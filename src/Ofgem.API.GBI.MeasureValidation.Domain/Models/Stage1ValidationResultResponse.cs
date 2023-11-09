namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class Stage1ValidationResultResponse
    {
        public Guid DocumentId { get; set; }
        public string? FileName { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
