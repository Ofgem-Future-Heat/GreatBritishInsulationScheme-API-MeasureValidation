using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class FilesWithErrorsMetadataService : IFilesWithErrorsMetadataService
    {
        private readonly IMeasureRepository _measureRepository;
        private readonly ILogger<FilesWithErrorsMetadataService> _logger;
        private readonly IDocumentApiClient _documentApiClient;

        public FilesWithErrorsMetadataService(IMeasureRepository measureRepository, ILogger<FilesWithErrorsMetadataService> logger, IDocumentApiClient documentApiClient)
        {
            _measureRepository = measureRepository;
            _logger = logger;
            _documentApiClient = documentApiClient;
        }

        public async Task<FilesWithErrorsMetadata> GetLatestFilesWithErrorsMetadata(string? supplierName)
        {
            Guard.Against.NullOrWhiteSpace(supplierName, nameof(supplierName));

            try
            {
                var result = await _measureRepository.GetLatestFilesWithErrors(supplierName);
                
                if (result.FilesWithErrors.Any())
                {
                    var filesWithErrors = result.FilesWithErrors;
                    result.FilesWithErrors = await GetFileNamesForErrors(filesWithErrors);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest files with errors for supplier name '{supplierName}': {ex.Message}", supplierName, ex.Message);
                throw;
            }
        }
        public async Task<FilesWithErrorsMetadata> GetAllFilesWithErrorsMetadata(string? supplierName)
        {
            Guard.Against.NullOrWhiteSpace(supplierName, nameof(supplierName));

            try
            {
                var result = await _measureRepository.GetAllFilesWithErrors(supplierName);

                if (result.FilesWithErrors.Any())
                {
                    var filesWithErrors = result.FilesWithErrors;
                    result.FilesWithErrors = await GetFileNamesForErrors(filesWithErrors);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all files with errors for supplier name '{supplierName}': {ex.Message}", supplierName, ex.Message);
                throw;
            }
        }

        private async Task<IEnumerable<FileWithErrorsMetadata>> GetFileNamesForErrors(IEnumerable<FileWithErrorsMetadata> filesWithErrors)
        {
            var fileWithErrorsMetadata = filesWithErrors.ToList();
            var filesWithErrorsDocumentIds = fileWithErrorsMetadata.Select(x => x.DocumentId);
            var documentIdsAndNames = await _documentApiClient.GetDocumentsNames(filesWithErrorsDocumentIds);
            var filesWithErrorsWithFoundFileName = FilterToDocumentsWithFoundFilenames(fileWithErrorsMetadata, documentIdsAndNames.Keys);
            return AddFilenameToFileWithErrorsMetadata(filesWithErrorsWithFoundFileName, documentIdsAndNames);
        }

        private static IEnumerable<FileWithErrorsMetadata> FilterToDocumentsWithFoundFilenames(IEnumerable<FileWithErrorsMetadata> filesWithErrors, IEnumerable<Guid> foundDocumentIds)
        {
            var filesWithErrorsWithFoundFileName = filesWithErrors.Where(x => foundDocumentIds.Contains(x.DocumentId));

            return filesWithErrorsWithFoundFileName;
        }

        private static IEnumerable<FileWithErrorsMetadata> AddFilenameToFileWithErrorsMetadata(IEnumerable<FileWithErrorsMetadata> filesWithErrorsMetadata, IDictionary<Guid, string> documentIdsAndNames)
        {
            var filesWithErrors = filesWithErrorsMetadata.ToArray();

            foreach (var fileWithErrorsMetadata in filesWithErrors)
            {
                if (documentIdsAndNames.TryGetValue(fileWithErrorsMetadata.DocumentId, out var fileName))
                {
                    fileWithErrorsMetadata.FileName = fileName;
                }
            }

            return filesWithErrors;
        }
    }
}