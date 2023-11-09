using CsvHelper.Configuration.Attributes;

namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class ErrorReportResponse
    {
        [Name("Measure reference number")]
        public string? MeasureReferenceNumber { get; set; }
        [Name("Measure notification date")]
        public string? CreatedDate { get; set; }
        [Name("Current measure status")]
        public string? MeasureStatus { get; set; }
        [Name("Current measure stage")]
        public string? ErrorStage { get; set; }
        [Name("Field containing error in notification template")]
        public string? Field { get; set; }
        [Name("What was added to the notification template")]
        public string? WhatWasAddedToTheNotificationTemplate { get; set; }
        [Name("Reason for error with measure")]
        public string? ReasonForError { get; set; }
        [Name("How to fix this error")]
        public string? HowToFix { get; set; }
        [Name("Test Reference number")]
        public required string TestNumber { get; set; }
    }
}
