using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class Stage1ProcessingService : IStage1ProcessingService
    {
        private readonly ILogger<Stage1ProcessingService> _logger;
        private readonly IMeasureStage1RulesService _measureRuleService;
        private readonly IPreProcessingService _preProcessingService;
        private readonly IMeasureRepository _measureRepository;
        private readonly ISendMessageService _sendMessageService;

        public Stage1ProcessingService(ILogger<Stage1ProcessingService> logger, IMeasureStage1RulesService measureRuleService,
            IPreProcessingService preProcessingService, IMeasureRepository measureRepository, ISendMessageService sendMessageService)
        {
            _logger = logger;
            _measureRuleService = measureRuleService;
            _preProcessingService = preProcessingService;
            _measureRepository = measureRepository;
            _sendMessageService = sendMessageService;
        }

        public async Task ProcessStage1Validation(MeasureDocumentDetails documentDetails)
        {
            try
            {
                _logger.LogInformation("Stage1 validation has started.");
                var measures = await _preProcessingService.GetMeasuresFromDocumentAndDatabase(documentDetails.DocumentId);

                var measureModels = measures.ToList();
                var validationResult = await _measureRuleService.ValidateMeasures(measureModels);

                validationResult.DocumentId = documentDetails.DocumentId;
                validationResult.FileName = measureModels.Any() ? measureModels.First().FileName : "";

                await _measureRepository.SaveStage1Result(validationResult);

                if (validationResult.PassedMeasures?.Count > 0)
                {
                    validationResult.PassedMeasures.ForEach(m => m.MeasureStatusId = (int)Types.MeasureStatus.NotifiedIncomplete);
                    await _measureRepository.SaveMeasures(validationResult.PassedMeasures!);

                    await _sendMessageService.SendMessageToTriggerStage2ValidationAsync(new MeasureDocumentDetails
                    {
                        DocumentId = documentDetails.DocumentId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in stage1 validation. {exMessage}", ex.Message);
                throw;
            }
        }

        public async Task<Stage1ValidationResultResponse> GetStage1ValidationResult(Guid documentId)
        {
            try
            {
                var result = await _measureRepository.GetStage1Result(documentId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot get stage1 result for {docId}. {exMessage}", documentId, ex.Message);
                throw;
            }
        }
    }
}
