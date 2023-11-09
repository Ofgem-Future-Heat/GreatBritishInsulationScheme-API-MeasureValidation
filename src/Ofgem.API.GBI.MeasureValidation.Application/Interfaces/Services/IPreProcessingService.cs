using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IPreProcessingService
    {
        Task<IEnumerable<MeasureModel>> GetMeasuresFromDocumentAndDatabase(Guid documentId);
    }
}
