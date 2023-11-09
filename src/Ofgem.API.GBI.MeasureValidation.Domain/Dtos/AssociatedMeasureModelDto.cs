namespace Ofgem.API.GBI.MeasureValidation.Domain.Dtos
{
    public class AssociatedMeasureModelDto
    {
        public string? MeasureReferenceNumber { get; set; }
        public string? MeasureType { get; set; }
        public int? MeasureCategoryId { get; set; }
        public string? MeasureCategory { get; set; }
        public AddressDto Address { get; set; } = new();
        public string? SupplierReference { get; set; }
        public DateTime? DateOfCompletedInstallation { get; set; }
        public string? EligibilityType { get; set; }
    }
}
