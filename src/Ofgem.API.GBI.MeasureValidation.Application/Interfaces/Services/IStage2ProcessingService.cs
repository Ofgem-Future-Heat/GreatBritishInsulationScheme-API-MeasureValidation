using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IStage2ProcessingService
    {
        Task ProcessStage2Validation(MeasureDocumentDetails documentDetails);
    }
}
