using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.Database.GBI.Measures.Domain.Dtos.MeasureUpload;
using Ofgem.Database.GBI.Measures.Domain.Entities;
using Ofgem.Database.GBI.Measures.Domain.Persistence;
using System.Collections.Concurrent;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.Repositories
{
    public class MeasureRepository : IMeasureRepository
    {
        private readonly ILogger<MeasureRepository> _logger;
        private readonly MeasuresDbContext _context;
        private readonly IMapper _mapper;
        private readonly TimeProvider _timeProvider;
        private const string Stage2 = "Stage 2";

        public MeasureRepository(ILogger<MeasureRepository> logger, MeasuresDbContext context, IMapper mapper, TimeProvider timeProvider)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _timeProvider = timeProvider;
        }

        public async Task SaveStage1Result(Stage1ValidationResultModel result)
        {
            try
            {
                var stage1Result = _mapper.Map<Stage1ValidationResult>(result);
                var stage1Errors = _mapper.Map<List<ValidationError>>(result.FailedMeasureErrors);

                _context.Stage1ValidationResults.Add(stage1Result);
                await _context.AddRangeAsync(stage1Errors);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while saving stage1 result. {exMessage}", ex.Message);
                throw;
            }
        }

        public async Task SaveAndUpdateStage2ErrorResults(List<StageValidationError> result)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var stage2Errors = _mapper.Map<List<ValidationError>>(result);

                        await _context.AddRangeAsync(stage2Errors);

                        var errorMeasureReferenceIds = stage2Errors.Select(x => x.MeasureReferenceNumber!).ToList();

                        var existingStage2Errors = await GetExistingStage2Errors(errorMeasureReferenceIds);

                        if (existingStage2Errors.Any())
                        {
                            foreach (var existingValidationError in existingStage2Errors)
                            {
                                existingValidationError.IsDeleted = true;
                                existingValidationError.ModifiedDate = _timeProvider.GetLocalNow().UtcDateTime;
                            }

                            await _context.BulkUpdateAsync(existingStage2Errors, options =>
                            {
                                options.SetOutputIdentity = true;
                                options.PropertiesToIncludeOnUpdate = new List<string>
                            {
                             nameof(ValidationError.IsDeleted),
                             nameof(ValidationError.ModifiedDate),
                            };
                            });
                        }
                        await _context.BulkInsertAsync(stage2Errors, options => { options.SetOutputIdentity = true; });
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error while saving or updating stage 2 error result. {exMessage}", ex.Message);
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });
        }

        private Task<List<ValidationError>> GetExistingStage2Errors(List<string> errorsMeasureReferenceIds)
        {
            var existingErrorMatches = _context.ValidationErrors.Where(x => x.ErrorStage != null && errorsMeasureReferenceIds.Contains(x.MeasureReferenceNumber!) && x.ErrorStage.Equals(Stage2) && !x.IsDeleted).ToListAsync();
            return existingErrorMatches;
        }

        public async Task<Stage1ValidationResultResponse> GetStage1Result(Guid documentId)
        {
            try
            {
                var stage1Result = await _context.Stage1ValidationResults.Where(x => x.DocumentId == documentId).FirstOrDefaultAsync();
                var result = _mapper.Map<Stage1ValidationResultResponse>(stage1Result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting stage1 result. {exMessage}", ex.Message);
                throw;
            }
        }

        public async Task SaveMeasures(IEnumerable<MeasureModel> measureModels)
        {
            try
            {
                var measureDtos = await SetMeasureProperties(measureModels);
                var measures = _mapper.Map<List<Measure>>(measureDtos);
                await _context.AddRangeAsync(measures);
                await BulkInsertOrUpdateAsync(measures);

                await AddMeasureHistory(measures);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while saving measures after stage1 passed. {exMessage}", ex.Message);
                throw;
            }
        }

        protected virtual async Task BulkInsertOrUpdateAsync(List<Measure> measures)
        {
            await _context.BulkInsertOrUpdateAsync(measures, options =>
            {
                options.SetOutputIdentity = true;
                options.PropertiesToExcludeOnUpdate = new List<string>
                {
                    nameof(Measure.MeasureId),
                    nameof(Measure.MeasureReferenceNumber),
                    nameof(Measure.CreatedBy),
                    nameof(Measure.CreatedDate)
                };
                options.UpdateByProperties = new List<string> { nameof(Measure.MeasureReferenceNumber) };
            });
        }

        public async Task AddMeasureHistory(List<Measure> measures)
        {
            try
            {
                var measureHistory = _mapper.Map<List<MeasureHistory>>(measures);
                await _context.AddRangeAsync(measureHistory);
                await BulkInsertAsync(measureHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError("Add measure history error. {exMessage}", ex.Message);
                throw;
            }
        }

        protected virtual Task BulkInsertAsync(List<MeasureHistory> measureHistory)
        {
            return _context.BulkInsertAsync(measureHistory);
        }

        public async Task<IEnumerable<PurposeOfNotification>> GetPurposeOfNotifications()
        {
            return await _context.PurposeOfNotifications.ToListAsync();
        }

        public async Task<IEnumerable<MeasureType>> GetMeasureTypes()
        {
            return await _context.MeasureTypes
                .Include(mt => mt.MeasureCategory)
                .ToListAsync();
        }

        public async Task<IEnumerable<EligibilityType>> GetEligibilityTypes()
        {
            return await _context.EligibilityTypes.ToListAsync();
        }

        public async Task<IEnumerable<TenureType>> GetTenureTypes()
        {
            return await _context.TenureTypes.ToListAsync();
        }

        public async Task<IEnumerable<VerificationMethodType>> GetVerificationMethodTypes()
        {
            return await _context.VerificationMethodType.ToListAsync();
        }

        public async Task<IEnumerable<InnovationMeasure>> GetInnovationMeasuresAsync()
        {
            return await _context.InnovationMeasures.ToListAsync();
        }

        public async Task<IEnumerable<FlexReferralRoute>> GetFlexReferralRoutesAsync()
        {
            return await _context.FlexReferralRoutes.ToListAsync();
        }

        public async Task<IEnumerable<ExistingMeasureDetailsForMeasureModel>> GetExistingMeasureData(IEnumerable<string> measureReferenceNumbers)
        {
            var measureReferenceNumbersList = measureReferenceNumbers.ToList();
            if (!measureReferenceNumbersList.Any()) return new List<ExistingMeasureDetailsForMeasureModel>();

            var result = await _context.Measures
                .Where(m => m.MeasureReferenceNumber != null && measureReferenceNumbersList.Contains(m.MeasureReferenceNumber))
                .Select(m => new ExistingMeasureDetailsForMeasureModel
                {
                    MeasureReferenceNumber = m.MeasureReferenceNumber,
                    ExistingSupplierReference = m.SupplierReference,
                    MeasureStatusId = m.MeasureStatusId,
                    CreatedDate = m.CreatedDate,
                })
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<AssociatedMeasureModelDto>> GetAssociatedMeasureData(IEnumerable<string> measureReferenceNumbers)
        {
            var referenceNumbers = measureReferenceNumbers.ToList();
            if (!referenceNumbers.Any())
            {
                return new List<AssociatedMeasureModelDto>();
            }

            return await _context.Measures
                .Include(m => m.MeasureType)
                    .ThenInclude(mt => mt != null ? mt.MeasureCategory : null)
                .Include(m => m.EligibilityType)
                .Where(m => m.MeasureTypeId.HasValue && m.MeasureReferenceNumber != null && referenceNumbers.Contains(m.MeasureReferenceNumber))
                .Select(m => new AssociatedMeasureModelDto
                {
                    MeasureReferenceNumber = m.MeasureReferenceNumber,
                    MeasureType = m.MeasureType != null ? m.MeasureType.Name : null,
                    MeasureCategoryId = m.MeasureType != null ? m.MeasureType.MeasureCategoryId : null,
                    MeasureCategory = m.MeasureType != null && m.MeasureType.MeasureCategory != null ? m.MeasureType.MeasureCategory.Name : null,
                    SupplierReference = m.SupplierReference,
                    DateOfCompletedInstallation = m.DateOfCompletedInstallation,
                    EligibilityType = m.EligibilityType != null ? m.EligibilityType.Name : null,
                    Address = new AddressDto()
                    {
                        BuildingName = m.BuildingName,
                        BuildingNumber = m.BuildingNumber,
                        FlatNameNumber = m.FlatNameNumber,
                        PostCode = m.PostCode,
                        StreetName = m.StreetName,
                        Town = m.Town
                    }
                })
                .ToListAsync();
        }

        public async Task<FilesWithErrorsMetadata> GetLatestFilesWithErrors(string supplierName)
        {
            try
            {
                var filesWithErrors = await _context.ValidationErrors.Where(x => x.SupplierName == supplierName)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToArrayAsync();
                var filesWithErrorsGroupedByDocumentIdAndStage = filesWithErrors.GroupBy(x => new
                {
                    x.DocumentId,
                    x.ErrorStage
                }).Take(5);

                var filesMetadata = filesWithErrorsGroupedByDocumentIdAndStage.Select(x => x.First())
                    .Select(x => new FileWithErrorsMetadata
                    {
                        DocumentId = x.DocumentId,
                        ErrorStage = x.ErrorStage,
                        DateTime = x.CreatedDate,
                    });

                var result = new FilesWithErrorsMetadata { FilesWithErrors = filesMetadata };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting latest files with errors: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FilesWithErrorsMetadata> GetAllFilesWithErrors(string supplierName)
        {
            try
            {
                var filesWithErrors = await _context.ValidationErrors.Where(x => x.SupplierName == supplierName)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToArrayAsync();
                var filesWithErrorsGroupedByDocumentIdAndStage = filesWithErrors.GroupBy(x => new
                {
                    x.DocumentId,
                    x.ErrorStage
                });

                var filesMetadata = filesWithErrorsGroupedByDocumentIdAndStage.Select(x => x.First())
                    .Select(x => new FileWithErrorsMetadata
                    {
                        DocumentId = x.DocumentId,
                        ErrorStage = x.ErrorStage,
                        DateTime = x.CreatedDate,
                    });

                var result = new FilesWithErrorsMetadata { FilesWithErrors = filesMetadata };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting all files with errors: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<ConcurrentDictionary<string, string>> GetMeasureCategoriesByTypeAsync()
        {
            var task = Task.Run(() =>
            {
                var result =
                    _context.MeasureTypes
                    .Join(_context.MeasureCategories,
                    type => type.MeasureCategoryId,
                    category => category.MeasureCategoryId,
                    (type, joinObject) =>
                    new
                    {
                        key = type.Name ?? string.Empty,
                        value = joinObject.Name ?? string.Empty
                    })
                    .Where(item => item.key != string.Empty)
                    .ToDictionary(c => c.key.ToUpperInvariant(), c => c.value);

                ConcurrentDictionary<string, string> measureCategoryDictionary = new ConcurrentDictionary<string, string>(result);
                return measureCategoryDictionary;
            });

            return await task;
        }

        public async Task<List<string>> GetMeasureTypesInnovationNumbers(string measureType)
        {
            var innovationNumbers = await _context.InnovationMeasures.Where(x => x.MeasureTypeName == measureType).Select(x => x.InnovationMeasureNumber)
                .ToListAsync();

            return innovationNumbers;
        }

        private async Task<IEnumerable<MeasureDto>> SetMeasureProperties(IEnumerable<MeasureModel> passedMeasures)
        {
            try
            {
                var measures = _mapper.Map<List<MeasureDto>>(passedMeasures);
                var purposeOfNotifications = await GetPurposeOfNotifications();
                var measureTypes = await GetMeasureTypes();
                var eligibilityTypes = await GetEligibilityTypes();
                var tenureTypes = await GetTenureTypes();

                measures.ForEach(m =>
                {
                    m.PurposeOfNotificationId = purposeOfNotifications.FirstOrDefault(x => x.Name!.Equals(m.PurposeOfNotification, StringComparison.OrdinalIgnoreCase))?.PurposeOfNotificationId;
                    m.MeasureTypeId = measureTypes.FirstOrDefault(x => x.Name!.Equals(m.MeasureType, StringComparison.OrdinalIgnoreCase))?.MeasureTypeId;
                    m.EligibilityTypeId = eligibilityTypes.FirstOrDefault(x => x.Name!.Equals(m.EligibilityType, StringComparison.OrdinalIgnoreCase))?.EligibilityTypeId;
                    m.ModifiedDate = _timeProvider.GetLocalNow().UtcDateTime;
                    m.TenureTypeId = tenureTypes.FirstOrDefault(x => x.Name!.Equals(m.TenureType, StringComparison.OrdinalIgnoreCase))?.TenureTypeId;
                });
                return measures;
            }
            catch (Exception ex)
            {
                _logger.LogError("Set measure properties error. {exMessage}", ex.ToString());
                throw;
            }
        }

        public async Task<IDictionary<string, AssociatedInFillMeasuresDto>> GetMeasureInfillsAsync(string measureReferenceNumber, string infillMeasureReferenceNumber)
        {
            var task = Task.Run(() =>
            {
                var result = _context.Measures
                    .Where(c => c.MeasureReferenceNumber != measureReferenceNumber &&
                            (c.AssociatedInfillMeasure1 != null && c.AssociatedInfillMeasure1.Equals(infillMeasureReferenceNumber)) ||
                            (c.AssociatedInfillMeasure2 != null && c.AssociatedInfillMeasure2.Equals(infillMeasureReferenceNumber)) ||
                            (c.AssociatedInfillMeasure3 != null && c.AssociatedInfillMeasure3.Equals(infillMeasureReferenceNumber))
                         ).ToDictionary(measure => measure.MeasureReferenceNumber!,
                        measure => new AssociatedInFillMeasuresDto
                            (IfNotApplicable(measure.AssociatedInfillMeasure1), IfNotApplicable(measure.AssociatedInfillMeasure2), IfNotApplicable(measure.AssociatedInfillMeasure3)));

                return result;

                static string? IfNotApplicable(string? modelProperty) =>
                    modelProperty.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) ? null : modelProperty;
            });

            return await task;
        }

        public async Task<FivePercentExtension?> GetFivePercentExtension(string supplierName, DateTime doci)
        {
            return await _context.FivePercentExtensions.Where(x => x.SupplierName!.Equals(supplierName)
                                            && x.DOCIPeriodStartDate <= doci && x.DOCIPeriodEndDate >= doci).FirstOrDefaultAsync();
        }
    }
}