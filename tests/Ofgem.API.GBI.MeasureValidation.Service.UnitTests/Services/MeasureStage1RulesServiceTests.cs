using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class MeasureStage1RulesServiceTests
    {
        private readonly Mock<IEnumerable<IGeneralRule<MeasureModel>>> _stage1Rules;
        private readonly Mock<RuleAsync<MeasureModel>> _rule;
        private readonly MeasureStage1RulesService _measureStage1RulesService;

        public MeasureStage1RulesServiceTests()
        {
            _stage1Rules = new Mock<IEnumerable<IGeneralRule<MeasureModel>>>();
            _rule = new Mock<RuleAsync<MeasureModel>>();
            Mock<TimeProvider> mockTimeProvider = new();
            _measureStage1RulesService = new MeasureStage1RulesService(_stage1Rules.Object, mockTimeProvider.Object);
        }

        [Fact]
        public async Task ValidateMeasures_SuccessResult_Count()
        {
            //Arrange
            _rule.Setup(r => r.InvokeAsync()).ReturnsAsync(It.IsAny<RuleResult>());
            var rules = new List<IGeneralRule<MeasureModel>>() { _rule.Object };
            _stage1Rules.Setup(m => m.GetEnumerator()).Returns(rules.GetEnumerator());

            //Act
            var result = await _measureStage1RulesService.ValidateMeasures(new List<MeasureModel> { It.IsAny<MeasureModel>() });

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.SuccessCount);
        }

        [Fact]
        public async Task ValidateMeasures_FailedResult_Count()
        {
            //Arrange

            var test = new List<StageValidationError>();

            _rule.Setup(r => r.InvokeAsync()).ReturnsAsync(new RuleResult() { Result = test });
            var rules = new List<IGeneralRule<MeasureModel>>() { _rule.Object };
            _stage1Rules.Setup(m => m.GetEnumerator()).Returns(rules.GetEnumerator());

            //Act
            var result = await _measureStage1RulesService.ValidateMeasures(new List<MeasureModel> 
            { 
                new()
                {
                    FileName = "test.csv"
                } 
            });

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.FailedCount);
        }

        [Theory]
        [InlineData("BGTBG1111111")]
        public async Task ValidateMeasures_Returns_Result(string measureRefNumber)
        {
            //Arrange
            var measures = new List<MeasureModel>()
            {
                new() { MeasureReferenceNumber = measureRefNumber },
                new() { MeasureReferenceNumber = "BGTBG2222222" }
            };

            var stage1Errors = new List<StageValidationError>()
            {
                new()
                {
                    MeasureReferenceNumber = measureRefNumber
                },
                new()
                {
                    MeasureReferenceNumber = measureRefNumber
                }
            };

            _rule.Setup(r => r.InvokeAsync()).ReturnsAsync(new RuleResult() { Result = stage1Errors });
            var rules = new List<IGeneralRule<MeasureModel>>() { _rule.Object };
            _stage1Rules.Setup(m => m.GetEnumerator()).Returns(rules.GetEnumerator());

            //Act
            var result = await _measureStage1RulesService.ValidateMeasures(measures);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<Stage1ValidationResultModel>(result);
            Assert.Equal(1, result.SuccessCount);
            Assert.Equal(1, result.FailedCount);
            Assert.Equal(2, result.FailedMeasureErrors.Count);
        }
    }
}
