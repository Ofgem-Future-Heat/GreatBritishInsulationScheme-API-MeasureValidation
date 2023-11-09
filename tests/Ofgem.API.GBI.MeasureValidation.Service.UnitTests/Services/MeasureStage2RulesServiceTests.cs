using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class MeasureStage2RulesServiceTests
    {
        private readonly Mock<IEnumerable<IGeneralRule<MeasureModel>>> _stage2Rules;
        private readonly Mock<RuleAsync<MeasureModel>> _rule;
        private readonly MeasureStage2RulesService _measureStage2RulesService;
        private readonly Mock<TimeProvider> _mockTimeProvider;

        public MeasureStage2RulesServiceTests()
        {
            _stage2Rules = new Mock<IEnumerable<IGeneralRule<MeasureModel>>>();
            _rule = new Mock<RuleAsync<MeasureModel>>();
            _mockTimeProvider = new Mock<TimeProvider>();
            _measureStage2RulesService = new MeasureStage2RulesService(_stage2Rules.Object, _mockTimeProvider.Object);
        }

        [Fact]
        public async Task ValidateMeasures_SuccessResult()
        {
            //Arrange
            _rule.Setup(r => r.InvokeAsync()).ReturnsAsync(It.IsAny<RuleResult>());
            var rules = new List<IGeneralRule<MeasureModel>>() { _rule.Object };
            _stage2Rules.Setup(m => m.GetEnumerator()).Returns(rules.GetEnumerator());

            //Act
            var result = await _measureStage2RulesService.ValidateMeasures(new List<MeasureModel> { It.IsAny<MeasureModel>() });

            //Assert
            Assert.IsType<Stage2ValidationResultModel>(result);
            Assert.NotEmpty(result.PassedMeasures!);
            Assert.Empty(result.FailedMeasureErrors);

        }

        [Fact]
        public async Task ValidateMeasures_FailedResult()
        {
            //Arrange
            var stage2Errors = new List<StageValidationError>()
            {
                new()
                {
                    MeasureReferenceNumber = "BGTBG1111111"
                }
            };

            _rule.Setup(r => r.InvokeAsync()).ReturnsAsync(new RuleResult() { Result = stage2Errors });
            var rules = new List<IGeneralRule<MeasureModel>>() { _rule.Object };
            _stage2Rules.Setup(m => m.GetEnumerator()).Returns(rules.GetEnumerator());

            //Act
            const string supplierName = "SUP";
            var result = await _measureStage2RulesService.ValidateMeasures(new List<MeasureModel> 
            { 
                new()
                {
                    FileName = "test.csv",
                    SupplierName = supplierName
                } 
            });

            //Assert
            Assert.IsType<Stage2ValidationResultModel>(result);
            Assert.NotEmpty(result.FailedMeasureErrors);
            Assert.Empty(result.PassedMeasures!);
        }


        [Fact]
        public async Task ValidateMeasures_FailedResult_CommonPropertiesValuesSet()
        {
            //Arrange
            const string supplierName = "SUP";
            const string documentId = "DocumentID";
            var createdDate = new DateTime(2023, 09, 25, 16, 40, 00, DateTimeKind.Unspecified);

            var stage2Errors = new[]
            {
                new StageValidationError
                {
                    MeasureReferenceNumber = "BGTBG1111111",
                }
            };

            _rule.Setup(r => r.InvokeAsync()).ReturnsAsync(new RuleResult { Result = stage2Errors });
            var rules = new List<IGeneralRule<MeasureModel>>() { _rule.Object };
            _stage2Rules.Setup(m => m.GetEnumerator()).Returns(rules.GetEnumerator());
            _mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(createdDate);

            //Act
            var result = await _measureStage2RulesService.ValidateMeasures(new []
            {
                new MeasureModel()
                {
                    FileName = "test.csv",
                    SupplierName = supplierName,
                    DocumentId = documentId,
                    CreatedDate = createdDate,
                }
            });

            //Assert
            var actualStageValidationError = result.FailedMeasureErrors.Single();
            Assert.Equal("Stage 2", actualStageValidationError.ErrorStage);
            Assert.Equal("Notified Incomplete", actualStageValidationError.MeasureStatus);
            Assert.Equal(documentId, actualStageValidationError.DocumentId);
            Assert.Equal(createdDate, actualStageValidationError.CreatedDate);
            Assert.Equal(supplierName, actualStageValidationError.SupplierName);
        }
    }
}
