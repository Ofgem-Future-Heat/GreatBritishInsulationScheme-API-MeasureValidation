using Ofgem.Database.GBI.Measures.Domain.Entities;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure
{
    public interface IErrorReportRepository
    {
        Task<IEnumerable<ValidationError>> GetValidationErrors(Guid documentId, string? stage);
    }
}
