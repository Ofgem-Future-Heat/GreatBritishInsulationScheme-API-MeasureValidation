namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class AddressValidationModel
    {
        public string? AddressReferenceNumber { get; set; }
        public string? FlatNumberOrName { get; set; }
        public string? BuildingNumber { get; set; }
        public string? BuildingName { get; set; }
        public string? Street { get; set; }
        public string? Town { get; set; }
        public string? Postcode { get; set; }
        public string? Uprn { get; set; }
    }
}