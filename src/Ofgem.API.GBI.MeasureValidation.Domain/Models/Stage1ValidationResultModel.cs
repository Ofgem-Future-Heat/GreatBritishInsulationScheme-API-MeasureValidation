namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class Stage1ValidationResultModel
    {
        public Guid DocumentId { get; set; }
        public string? FileName { get; set; }
        public int TotalRowCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<StageValidationError> FailedMeasureErrors { get; set; } = new();
        public List<MeasureModel>? PassedMeasures { get; set; }
    }
}
