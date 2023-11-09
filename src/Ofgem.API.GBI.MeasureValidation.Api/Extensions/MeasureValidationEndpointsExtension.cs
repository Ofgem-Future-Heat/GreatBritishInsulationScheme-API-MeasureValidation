using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Api.Extensions
{
    public static class MeasureValidationEndpointsExtension
    {
        public static void MapMeasureValidationEndpoints(this WebApplication app)
        {
            app.MapPost("/ProcessStage1Validation", async (MeasureDocumentDetails? documentDetails, 
                IStage1ProcessingService _stage1ProcessingService, ILogger<Program> _logger) =>
            {
                if (documentDetails == null)
                {
                    return Results.BadRequest();
                }
                try
                {
                    await _stage1ProcessingService.ProcessStage1Validation(documentDetails);
                    return Results.Ok();

                }
                catch (Exception ex)
                {
                    _logger.LogError("Stage1 processing error. {exMessage}", ex.Message);
                    return Results.BadRequest();
                }
            });

            app.MapPost("/ProcessStage2Validation", async (MeasureDocumentDetails? documentDetails, IStage2ProcessingService stage2ProcessingService, ILogger<Program> _logger) =>
            {
                if (documentDetails == null)
                {
                    return Results.BadRequest();
                }
                try
                {
                    await stage2ProcessingService.ProcessStage2Validation(documentDetails);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Stage2 processing error. {exMessage}", ex.Message);
                    return Results.BadRequest();
                }
            });

            app.MapGet("/GetStage1ChecksResult/{documentId}", async (string documentId, IStage1ProcessingService stage1ProcessingService) =>
            {
                try
                {
                    var result = await stage1ProcessingService.GetStage1ValidationResult(Guid.Parse(documentId));

                    if (result == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });
        }
    }
}
