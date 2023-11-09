namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IErrorsReportService
    {
        Task<string> GetErrorsReport(Guid documentId, string? stage);
    }
}
