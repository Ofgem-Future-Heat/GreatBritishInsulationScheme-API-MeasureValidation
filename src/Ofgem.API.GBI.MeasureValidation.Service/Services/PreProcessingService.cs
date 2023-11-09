using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using System.Globalization;
using System.Text;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class PreProcessingService : IPreProcessingService
    {
        private readonly ILogger<PreProcessingService> _logger;
        private readonly IDocumentApiClient _documentApiClient;
        private readonly ISupplierApiClient _supplierApiClient;
        private readonly IMeasureRepository _measureRepository;
        private readonly IMapper _mapper;

        public PreProcessingService(ILogger<PreProcessingService> logger, IDocumentApiClient documentApiClient,
            ISupplierApiClient supplierApiClient, IMeasureRepository measureRepository, IMapper mapper)
        {
            _logger = logger;
            _documentApiClient = documentApiClient;
            _supplierApiClient = supplierApiClient;
            _measureRepository = measureRepository;

            _mapper = mapper;
        }

        public async Task<IEnumerable<MeasureModel>> GetMeasuresFromDocumentAndDatabase(Guid documentId)
        {
            var records = new List<MeasureModel>();
            if (documentId != Guid.Empty)
            {
                records = await GetMeasuresFromDocument(documentId);
                records = await AddExistingMeasureDataToMeasureModels(records);
                records = await AddAssociatedMeasureDataToMeasureModels(records);
            }
            return records;
        }


        private async Task<List<MeasureModel>> GetMeasuresFromDocument(Guid documentId)
        {
            try
            {
                var fileData = await _documentApiClient.GetDocument(documentId);
                var measureData = Encoding.UTF8.GetString(fileData.Content!);
                var supplierLicenses = await _supplierApiClient.GetSupplierLicencesAsync();
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.Trim().Replace("�", ""),
                    TrimOptions = TrimOptions.Trim
                };

                using var textReader = new StringReader(measureData);
                using var csvReader = new CsvReader(textReader, config);
                var records = csvReader.GetRecords<MeasureModel>().ToList();

                if (!records.Any())
                {
                    return new List<MeasureModel>();
                }

                var referenceDataDetails = await GetMeasureReferenceData();
                records.ForEach(r =>
                {
                    r.DocumentId = fileData.DocumentId;
                    r.FileName = fileData.FileName;
                    r.SupplierName = fileData.SupplierName;
                    r.CreatedDate = fileData.CreatedDate;
                    r.SupplierLicences = supplierLicenses;
                    r.ReferenceDataDetails = referenceDataDetails;
                });

                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get document error for {documentId}. {exMessage}", documentId, ex.Message);
                throw;
            }
        }

        private async Task<ReferenceDataDetails> GetMeasureReferenceData()
        {
            try
            {
                var purposeOfNotifications = await _measureRepository.GetPurposeOfNotifications();
                var measureTypes = await _measureRepository.GetMeasureTypes();
                var eligibilityTypes = await _measureRepository.GetEligibilityTypes();
                var tenureTypes = await _measureRepository.GetTenureTypes();
                var verificationMethodTypes = await _measureRepository.GetVerificationMethodTypes();
                var innovationMeasures = await _measureRepository.GetInnovationMeasuresAsync();
                var flexReferralRoutes = await _measureRepository.GetFlexReferralRoutesAsync();

                var referenceDataDetails = new ReferenceDataDetails()
                {
                    PurposeOfNotificationsList = purposeOfNotifications.Select(x => x.Name)!,
                    MeasureTypesList = _mapper.Map<IEnumerable<MeasureTypeDto>>(measureTypes),
                    EligibilityTypesList = eligibilityTypes.Select(x => x.Name)!,
                    TenureTypesList = tenureTypes.Select(x => x.Name)!,
                    VerificationMethodTypesList = verificationMethodTypes.Select(x => x.Name)!,
                    InnovationMeasureList = _mapper.Map<IEnumerable<InnovationMeasureDto>>(innovationMeasures),
                    FlexReferralRouteList = flexReferralRoutes?.Select(x => x.Name!)
                };

                return referenceDataDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get reference data error. {exMessage}", ex.ToString());
                throw;
            }
        }

        public async Task<List<MeasureModel>> AddExistingMeasureDataToMeasureModels(List<MeasureModel> records)
        {
            try
            {
                if (!records.Any())
                {
                    return records;
                }

                var existingMeasuresData = await GetExistingMeasureDataAsync(records);
                var existingMeasureDataList = existingMeasuresData.ToList();

                foreach (var record in records)
                {
                    var existingMeasureData = existingMeasureDataList.FirstOrDefault(
                        m => m.MeasureReferenceNumber != null &&
                        m.MeasureReferenceNumber.CaseInsensitiveEquals(record.MeasureReferenceNumber));

                    if (existingMeasureData != null)
                    {
                        record.IsExistingMeasure = existingMeasureData.MeasureReferenceNumber != null;
                        record.ExistingSupplierReference = existingMeasureData.ExistingSupplierReference;
                        record.CreatedDate = existingMeasureData.CreatedDate;
                        record.MeasureStatusId = existingMeasureData.MeasureStatusId;
                    }

                    if (record.MeasureType != null)
                    {
                        var measureType = record.ReferenceDataDetails.MeasureTypesList?.FirstOrDefault(m => m.Name.CaseInsensitiveEquals(record.MeasureType));
                        record.MeasureCategoryId = measureType?.MeasureCategoryId;
                    }

                    if (PurposeOfNotificationConstants.AutomaticLateExtension.CaseInsensitiveEquals(record.PurposeOfNotification))
                    {
                        record.FivePercentExtensionDto = await GetFivePercentExtensionsQuota(record.SupplierName, record.DateOfCompletedInstallation);
                    }
                }
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get existing measure details error. {exMessage}", ex.Message);
                throw;
            }
        }

        public async Task<List<MeasureModel>> AddAssociatedMeasureDataToMeasureModels(List<MeasureModel> records)
        {
            try
            {
                if (!records.Any())
                {
                    return records;
                }

                var associatedMeasureData = await GetAssociatedMeasureDataAsync(records);
                var associatedMeasureDataList = associatedMeasureData.ToList();

                foreach (var record in records)
                {
                    if (record.AssociatedInfillMeasure1 != null)
                    {
                        record.AssociatedInfillMeasure1Details = associatedMeasureDataList
                            .FirstOrDefault(x => x.MeasureReferenceNumber == record.AssociatedInfillMeasure1);
                    }

                    if (record.AssociatedInfillMeasure2 != null)
                    {
                        record.AssociatedInfillMeasure2Details = associatedMeasureDataList
                            .FirstOrDefault(x => x.MeasureReferenceNumber == record.AssociatedInfillMeasure2);
                    }

                    if (record.AssociatedInfillMeasure3 != null)
                    {
                        record.AssociatedInfillMeasure3Details = associatedMeasureDataList
                            .FirstOrDefault(x => x.MeasureReferenceNumber == record.AssociatedInfillMeasure3);
                    }

                    if (record.AssociatedInsulationMrnForHeatingMeasures != null)
                    {
                        record.AssociatedInsulationMeasureForHeatingMeasureDetails = associatedMeasureDataList
                            .FirstOrDefault(x => x.MeasureReferenceNumber == record.AssociatedInsulationMrnForHeatingMeasures);
                    }

                }
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get associated measure data error. {Message}", ex.Message);
                throw;
            }
        }

        private async Task<IEnumerable<ExistingMeasureDetailsForMeasureModel>> GetExistingMeasureDataAsync(IEnumerable<MeasureModel> records)
        {
            var measureReferenceNumbers = records
                .Where(record => record.MeasureReferenceNumber != null)
                .Select(record => new string(record.MeasureReferenceNumber!.ToUpperInvariant()))
                .ToList();

            return await _measureRepository.GetExistingMeasureData(measureReferenceNumbers);
        }

        private async Task<IEnumerable<AssociatedMeasureModelDto>> GetAssociatedMeasureDataAsync(IEnumerable<MeasureModel> records)
        {
            var measureModels = records.ToList();
            var associatedMeasure1ReferenceNumbers = measureModels
                .Where(record => record.AssociatedInfillMeasure1 != null)
                .Select(record => record.AssociatedInfillMeasure1!);

            var associatedMeasure2ReferenceNumbers = measureModels
                .Where(record => record.AssociatedInfillMeasure2 != null)
                .Select(record => record.AssociatedInfillMeasure2!);

            var associatedMeasure3ReferenceNumbers = measureModels
                .Where(record => record.AssociatedInfillMeasure3 != null)
                .Select(record => record.AssociatedInfillMeasure3!);

			var associatedInsulationForHeatingMeasuresMrns = measureModels
				.Where(record => record.AssociatedInsulationMrnForHeatingMeasures != null)
				.Select(record => record.AssociatedInsulationMrnForHeatingMeasures!);

			var measureReferenceNumbers = associatedMeasure1ReferenceNumbers
                .Concat(associatedMeasure2ReferenceNumbers)
                .Concat(associatedMeasure3ReferenceNumbers)
                .Concat(associatedInsulationForHeatingMeasuresMrns);

            measureReferenceNumbers = measureReferenceNumbers.Distinct();

            return await _measureRepository.GetAssociatedMeasureData(measureReferenceNumbers);
        }

        private async Task<FivePercentExtensionQuotaDto> GetFivePercentExtensionsQuota(string? supplierName, string? doci)
        {
            if (!string.IsNullOrEmpty(supplierName) && DateTime.TryParse(doci, out DateTime result))
            {
                var fivePercentQuota = await _measureRepository.GetFivePercentExtension(supplierName, result);
                return _mapper.Map<FivePercentExtensionQuotaDto>(fivePercentQuota);
            }
            return new FivePercentExtensionQuotaDto();
        }
    }
}
