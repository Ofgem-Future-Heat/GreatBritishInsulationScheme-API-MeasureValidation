namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class AddressValidationResponse
    {
        public AddressValidationModel? Address { get; set; }
        public bool IsValid { get; set; }
        public string? Uprn { get; set; } = null;
        public string? ErrorMessage { get; set; } = null;
        public string? CountryCode { get; set; } = null;
    }
}