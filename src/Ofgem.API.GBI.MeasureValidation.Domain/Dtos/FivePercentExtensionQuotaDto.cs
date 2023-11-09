namespace Ofgem.API.GBI.MeasureValidation.Domain.Dtos
{
    public class FivePercentExtensionQuotaDto
    {
        public string? SupplierName { get; set; }
        public DateTime? NotificationEndDate { get; set; }
        public DateTime? ThreeMonthEndDate { get; set; }
        public int? RemainingQuota { get; set; }
    }
}
