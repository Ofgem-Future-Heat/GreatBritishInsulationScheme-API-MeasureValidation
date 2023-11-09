namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class MeasureFileContent
    {
        public string? DocumentId { get; set; }
        public byte[]? Content { get; set; }
        public string? FileName { get; set; }
        public string? SupplierName { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
