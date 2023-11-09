namespace Ofgem.API.GBI.MeasureValidation.Domain.Dtos
{
    public class ExistingMeasureDetailsForMeasureModel
    {   
        public string? MeasureReferenceNumber { get; set; }
        public string? ExistingSupplierReference { get; set; }
        public int? MeasureStatusId { get; set; }
		public DateTime? CreatedDate { get; set; }
    }
}
