namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class SupplierLicenceResponse
    {
        public required string SupplierLicenceReference { get; set; }
        public string? LicenceNo { get; set; }
        public string? LicenceName { get; set; }
        public string? SupplierName { get; set; }
    }
}
