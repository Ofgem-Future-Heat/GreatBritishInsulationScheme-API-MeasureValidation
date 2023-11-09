using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class Stage2ProcessingService : IStage2ProcessingService
    {
        private readonly ILogger<Stage2ProcessingService> _logger;
        private readonly IAddressProcessingService _addressProcessingService;
        private readonly IMeasureStage2RulesService _measureRuleService;
        private readonly IPreProcessingService _preProcessingService;
        private readonly IMeasureRepository _measureRepository;
        private readonly IStage2PassedMeasuresProcessor _stage2PassedMeasuresProcessor;

        public Stage2ProcessingService(ILogger<Stage2ProcessingService> logger,
            IMeasureStage2RulesService measureRuleService, IPreProcessingService preProcessingService,
            IMeasureRepository measureRepository, IStage2PassedMeasuresProcessor stage2PassedMeasuresProcessor, IAddressProcessingService addressProcessingService)
        {
            _logger = logger;
            _measureRuleService = measureRuleService;
            _preProcessingService = preProcessingService;
            _measureRepository = measureRepository;
            _stage2PassedMeasuresProcessor = stage2PassedMeasuresProcessor;
            _addressProcessingService = addressProcessingService;
        }

        public async Task ProcessStage2Validation(MeasureDocumentDetails documentDetails)
        {
            try
            {
                _logger.LogInformation("Stage 2 validation has started.");
                var measures = await _preProcessingService.GetMeasuresFromDocumentAndDatabase(documentDetails.DocumentId);

                var measureModels = await _addressProcessingService.AddressVerificationAsync(measures.ToList());

                var result = await _measureRuleService.ValidateMeasures(measureModels);

                await _measureRepository.SaveAndUpdateStage2ErrorResults(result.FailedMeasureErrors);

                var updatedStage2Measures = await _stage2PassedMeasuresProcessor.Process(result.PassedMeasures!);

                await _measureRepository.SaveMeasures(updatedStage2Measures);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Stage 2 validation. {exMessage}", ex.Message);
                throw;
            }
        }
    }
}