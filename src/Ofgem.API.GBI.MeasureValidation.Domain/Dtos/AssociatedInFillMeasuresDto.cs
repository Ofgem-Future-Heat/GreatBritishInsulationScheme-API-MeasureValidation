namespace Ofgem.API.GBI.MeasureValidation.Domain.Dtos
{
    public class AssociatedInFillMeasuresDto
    {
        public AssociatedInFillMeasuresDto(string? associatedInFillMeasure1, string? associatedInFillMeasure2, string? associatedInFillMeasure3)
        {
            AssociatedInFillMeasure1 = associatedInFillMeasure1;
            AssociatedInFillMeasure2 = associatedInFillMeasure2;
            AssociatedInFillMeasure3 = associatedInFillMeasure3;
        }

        public string? AssociatedInFillMeasure1 { get; set; }
        public string? AssociatedInFillMeasure2 { get; set; }
        public string? AssociatedInFillMeasure3 { get; set; }
    }
}
