using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class Stage2ProcessingServiceTests
    {
        private readonly Mock<IMeasureStage2RulesService> _measureStage2RuleService;
        private readonly Stage2ProcessingService _stage2ProcessingService;
        private readonly Mock<IPreProcessingService> _preProcessingService;
        private readonly Mock<IMeasureRepository> _measureRepository;
        private readonly Mock<IStage2PassedMeasuresProcessor> _stage2PassedMeasuresProcessor;
        private readonly Mock<IAddressProcessingService> _addressProcessingService;

        public Stage2ProcessingServiceTests()
        {
            Mock<ILogger<Stage2ProcessingService>> logger = new();
            _measureStage2RuleService = new Mock<IMeasureStage2RulesService>();
            _preProcessingService = new Mock<IPreProcessingService>();
            _measureRepository = new Mock<IMeasureRepository>();
            _addressProcessingService = new Mock<IAddressProcessingService>();
            _stage2PassedMeasuresProcessor = new Mock<IStage2PassedMeasuresProcessor>();
            _stage2ProcessingService = new Stage2ProcessingService(logger.Object,
                _measureStage2RuleService.Object, _preProcessingService.Object, _measureRepository.Object, _stage2PassedMeasuresProcessor.Object, _addressProcessingService.Object);
        }

        [Fact]
        public async Task ProcessStage2Validation_ValidatesMeasures()
        {
            //Arrange
             _measureStage2RuleService.Setup(x => x.ValidateMeasures(It.IsAny<List<MeasureModel>>())).ReturnsAsync(new Stage2ValidationResultModel());
            _preProcessingService.Setup(d => d.GetMeasuresFromDocumentAndDatabase(It.IsAny<Guid>())).ReturnsAsync(new List<MeasureModel>());
            _addressProcessingService.Setup(c => c.AddressVerificationAsync(It.IsAny<List<MeasureModel>>())).ReturnsAsync(new List<MeasureModel>());

            //Act
            await _stage2ProcessingService.ProcessStage2Validation(new MeasureDocumentDetails());

            //Assert
            _measureStage2RuleService.Verify(d => d.ValidateMeasures(It.IsAny<List<MeasureModel>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessStage2Validation_SavesOnlyPassedMeasures()
        {
            //Arrange
            _preProcessingService.Setup(d => d.GetMeasuresFromDocumentAndDatabase(It.IsAny<Guid>()))
                .ReturnsAsync(Array.Empty<MeasureModel>());
            _addressProcessingService.Setup(c => c.AddressVerificationAsync(It.IsAny<List<MeasureModel>>()));

            var measureDocumentId = new Guid("4BCD6CBF-FBE7-411A-8C87-11D8A88F6BCF");
            var measureDocumentIdString = measureDocumentId.ToString();
            var passedMeasures = new List<MeasureModel>
            {
                new() { DocumentId = measureDocumentIdString },
            };
            _measureStage2RuleService.Setup(x => x.ValidateMeasures(It.IsAny<IEnumerable<MeasureModel>>()))
                .ReturnsAsync(new Stage2ValidationResultModel
                {
                    PassedMeasures = passedMeasures,
                });
            _stage2PassedMeasuresProcessor.Setup(x => x.Process(passedMeasures)).ReturnsAsync(passedMeasures);

            //Act
            await _stage2ProcessingService.ProcessStage2Validation(new MeasureDocumentDetails());

            //Assert
            _measureRepository.Verify(
                x => x.SaveMeasures(It.Is<IEnumerable<MeasureModel>>(y => y.Single().DocumentId == measureDocumentIdString)),
                Times.Once);
        }
    }
}
