using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IStage1ProcessingService
    {
        Task ProcessStage1Validation(MeasureDocumentDetails documentDetails);
        Task<Stage1ValidationResultResponse> GetStage1ValidationResult(Guid documentId);
    }
}
