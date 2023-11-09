using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure
{
    public interface IDocumentApiClient
    {
        Task<MeasureFileContent> GetDocument(Guid documentId);
        Task<IDictionary<Guid, string>> GetDocumentsNames(IEnumerable<Guid> documentIds);
    }
}
