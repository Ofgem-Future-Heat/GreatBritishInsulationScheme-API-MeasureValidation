namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class StageValidationError
    {
        public string? MeasureReferenceNumber { get; set; }
        public string? SupplierName { get; set; }
        public string? ErrorStage { get; set; }
        public string? MeasureStatus { get; set; }
        public string? TestNumber { get; set; }
        public string? WhatWasAddedToTheNotificationTemplate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? DocumentId { get; set; }
    }
}
