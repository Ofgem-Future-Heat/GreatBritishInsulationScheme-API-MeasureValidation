namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class Stage2ValidationResultModel
    {
        public Guid DocumentId { get; set; }
        public string? FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<StageValidationError> FailedMeasureErrors { get; set; } = new();
        public List<MeasureModel>? PassedMeasures { get; set; }
    }
}
