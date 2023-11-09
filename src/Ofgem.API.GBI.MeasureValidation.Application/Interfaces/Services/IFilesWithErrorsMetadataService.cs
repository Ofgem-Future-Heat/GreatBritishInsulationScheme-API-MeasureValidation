using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IFilesWithErrorsMetadataService
    {
        Task<FilesWithErrorsMetadata> GetLatestFilesWithErrorsMetadata(string supplierName);
        Task<FilesWithErrorsMetadata> GetAllFilesWithErrorsMetadata(string supplierName);
    };
}