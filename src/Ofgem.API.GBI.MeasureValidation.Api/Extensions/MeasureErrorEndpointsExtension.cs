using Microsoft.AspNetCore.Mvc;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;

namespace Ofgem.API.GBI.MeasureValidation.Api.Extensions
{
    public static class MeasureErrorEndpointsExtension
    {
        public static void MapGetErrorsEndpoints(this WebApplication app)
        {
            app.MapGet("/GetErrorsReport/{documentId}/{stage}", async (Guid documentId, string? stage, IErrorsReportService _errorReportService) =>
            {
                try
                {
                    if (documentId == Guid.Empty)
                    {
                        return Results.BadRequest();
                    }

                    var reportString = await _errorReportService.GetErrorsReport(documentId, stage);
                    return Results.Ok(reportString);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex);
                }
            });

            app.MapGet("/GetLatestFilesWithErrorsMetadata", async ([FromQuery] string supplierName, IFilesWithErrorsMetadataService _filesWithErrorsMetadataService, ILogger<Program> logger) =>
            {
                if (string.IsNullOrWhiteSpace(supplierName))
                {
                    return Results.BadRequest($"Invalid `{nameof(supplierName)}` parameter value provided: \'{supplierName}\'");
                }

                try
                {
                    var result = await _filesWithErrorsMetadataService.GetLatestFilesWithErrorsMetadata(supplierName);

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting latest files with errors: {Message}", ex.Message);
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            });

            app.MapGet("/GetAllFilesWithErrorsMetadata", async ([FromQuery] string supplierName, IFilesWithErrorsMetadataService _filesWithErrorsMetadataService, ILogger<Program> logger) =>
            {
                if (string.IsNullOrWhiteSpace(supplierName))
                {
                    return Results.BadRequest($"Invalid `{nameof(supplierName)}` parameter value provided: \'{supplierName}\'");
                }

                try
                {
                    var result = await _filesWithErrorsMetadataService.GetAllFilesWithErrorsMetadata(supplierName);

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting latest files with errors: {Message}", ex.Message);
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            });
        }
    }
}
