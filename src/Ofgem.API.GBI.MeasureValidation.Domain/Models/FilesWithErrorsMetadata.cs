namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class FileWithErrorsMetadata
    {
        public Guid DocumentId { get; set; }
        public DateTime? DateTime { get; set; }
        public string? FileName { get; set; }
        public string? ErrorStage { get; set; }
    }

    public class FilesWithErrorsMetadata
    {
        public required IEnumerable<FileWithErrorsMetadata> FilesWithErrors { get; set; }
    }
}