namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class ErrorReportRequest
    {
        public Guid DocumentId { get; set; }
        public string? Stage { get; set; }
    }
}
