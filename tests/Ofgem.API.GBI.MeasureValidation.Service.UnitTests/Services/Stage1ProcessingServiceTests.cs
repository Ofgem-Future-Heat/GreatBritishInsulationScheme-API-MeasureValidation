using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Services;
using Ofgem.Database.GBI.Measures.Domain.Entities;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class Stage1ProcessingServiceTests
    {
        private readonly Mock<IMeasureStage1RulesService> _measureStage1RuleService;
        private readonly Mock<IMeasureRepository> _measureRepository;
        private readonly Mock<IPreProcessingService> _preProcessingService;
        private readonly Stage1ProcessingService _stage1ProcessingService;

        public Stage1ProcessingServiceTests()
        {
            Mock<ILogger<Stage1ProcessingService>> logger = new();
            _measureStage1RuleService = new Mock<IMeasureStage1RulesService>();
            _measureRepository = new Mock<IMeasureRepository>();
            _preProcessingService = new Mock<IPreProcessingService>();
            Mock<ISendMessageService> sendMessageService = new();

			_stage1ProcessingService = new Stage1ProcessingService(logger.Object,
				_measureStage1RuleService.Object, _preProcessingService.Object, _measureRepository.Object, sendMessageService.Object);
		}

        [Fact]
        public async Task ProcessStage1Validation_Returns_Result()
        {
            //Arrange
            _measureStage1RuleService.Setup(x => x.ValidateMeasures(It.IsAny<List<MeasureModel>>())).ReturnsAsync(new Stage1ValidationResultModel());
            _preProcessingService.Setup(d => d.GetMeasuresFromDocumentAndDatabase(It.IsAny<Guid>())).ReturnsAsync(new List<MeasureModel>());

            //Act
            await _stage1ProcessingService.ProcessStage1Validation(new MeasureDocumentDetails());

            //Assert
            _measureRepository.Verify(m => m.SaveStage1Result(It.IsAny<Stage1ValidationResultModel>()), Times.Once);
        }

        [Fact]
        public async Task ProcessStage1Validation_WithPassedMeasures_SavesMeasures()
        {
            //Arrange
            var validationResult = new Stage1ValidationResultModel
            {
                PassedMeasures = new List<MeasureModel>
                {
                    new MeasureModel()
                }
            };

            _preProcessingService.Setup(d => d.GetMeasuresFromDocumentAndDatabase(It.IsAny<Guid>())).ReturnsAsync(new List<MeasureModel>());
            _measureStage1RuleService.Setup(x => x.ValidateMeasures(It.IsAny<List<MeasureModel>>())).ReturnsAsync(validationResult);

            _measureRepository.Setup(r => r.GetPurposeOfNotifications()).ReturnsAsync(new List<PurposeOfNotification>());
            _measureRepository.Setup(r => r.GetMeasureTypes()).ReturnsAsync(new List<MeasureType>());
            _measureRepository.Setup(r => r.GetEligibilityTypes()).ReturnsAsync(new List<EligibilityType>());
            _measureRepository.Setup(r => r.GetTenureTypes()).ReturnsAsync(new List<TenureType>());

            //Act
            await _stage1ProcessingService.ProcessStage1Validation(new MeasureDocumentDetails());

            //Assert
            _measureRepository.Verify(m => m.SaveMeasures(It.IsAny<IEnumerable<MeasureModel>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessStage1Validation_WithPassedMeasures_SavesMeasuresWithStatusNotifiedPending()
        {
            //Arrange
            var validationResult = new Stage1ValidationResultModel
            {
                PassedMeasures = new List<MeasureModel> { new() }
            };

            _preProcessingService.Setup(d => d.GetMeasuresFromDocumentAndDatabase(It.IsAny<Guid>())).ReturnsAsync(Array.Empty<MeasureModel>());
            _measureStage1RuleService.Setup(x => x.ValidateMeasures(It.IsAny<IEnumerable<MeasureModel>>())).ReturnsAsync(validationResult);

            _measureRepository.Setup(r => r.GetPurposeOfNotifications()).ReturnsAsync(Array.Empty<PurposeOfNotification>());
            _measureRepository.Setup(r => r.GetMeasureTypes()).ReturnsAsync(Array.Empty<MeasureType>());
            _measureRepository.Setup(r => r.GetEligibilityTypes()).ReturnsAsync(Array.Empty<EligibilityType>());
            _measureRepository.Setup(r => r.GetTenureTypes()).ReturnsAsync(Array.Empty<TenureType>());

            IEnumerable<MeasureModel>? measures = null;
            _measureRepository.Setup(r => r.SaveMeasures(It.IsAny<IEnumerable<MeasureModel>>()))
                .Callback<IEnumerable<MeasureModel>>(x => measures = x);

            //Act
            await _stage1ProcessingService.ProcessStage1Validation(new MeasureDocumentDetails());

            //Assert
            _measureRepository.Verify(m => m.SaveMeasures(It.IsAny<IEnumerable<MeasureModel>>()), Times.Once);
            Assert.NotNull(measures);
            Assert.Collection(measures, x => Assert.True(x.MeasureStatusId == MeasureStatusConstants.NotifiedIncomplete));
        }
    }
}
