using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.Database.GBI.Measures.Domain.Entities;
using System.Collections.Concurrent;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure
{
    public interface IMeasureRepository
    {
        Task SaveStage1Result(Stage1ValidationResultModel result);

        Task SaveAndUpdateStage2ErrorResults(List<StageValidationError> result);
        Task<Stage1ValidationResultResponse> GetStage1Result(Guid documentId);
        Task SaveMeasures(IEnumerable<MeasureModel> measureModels);
        Task<IEnumerable<PurposeOfNotification>> GetPurposeOfNotifications();
        Task<IEnumerable<MeasureType>> GetMeasureTypes();
        Task<IEnumerable<EligibilityType>> GetEligibilityTypes();
        Task<IEnumerable<TenureType>> GetTenureTypes();
        Task<IEnumerable<InnovationMeasure>> GetInnovationMeasuresAsync();
        Task<IEnumerable<VerificationMethodType>> GetVerificationMethodTypes();
        Task<IEnumerable<FlexReferralRoute>> GetFlexReferralRoutesAsync();
		Task<IEnumerable<ExistingMeasureDetailsForMeasureModel>> GetExistingMeasureData(IEnumerable<string> measureReferenceNumbers);
        Task<IEnumerable<AssociatedMeasureModelDto>> GetAssociatedMeasureData(IEnumerable<string> measureReferenceNumbers);
		Task<FilesWithErrorsMetadata> GetLatestFilesWithErrors(string supplierName);
        Task<FilesWithErrorsMetadata> GetAllFilesWithErrors(string supplierName);
        Task AddMeasureHistory(List<Measure> measures);
        Task<ConcurrentDictionary<string, string>> GetMeasureCategoriesByTypeAsync();
        Task<List<string>> GetMeasureTypesInnovationNumbers(string measureType);
        Task<IDictionary<string, AssociatedInFillMeasuresDto>> GetMeasureInfillsAsync(string measureReferenceNumber, string infillMeasureReferenceNumber);
        Task<FivePercentExtension?> GetFivePercentExtension(string supplierName, DateTime doci);
    }
}