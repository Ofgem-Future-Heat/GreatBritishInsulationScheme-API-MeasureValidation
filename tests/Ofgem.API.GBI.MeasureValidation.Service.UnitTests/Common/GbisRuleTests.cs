using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common
{
    public class GbisRuleTests
    {
        private const string MeasureReferenceNumber = "MeasureReferenceNumber";

        [Fact]
        public async Task InitializeAsync_SetsFailureConditionAsRuleConstraint()
        {
            // Arrange
            Predicate<MeasureModel> failureCondition = _ => true;

            var ruleLogicMock = new Mock<IRuleLogic>();
            ruleLogicMock.Setup(x => x.FailureCondition).Returns(failureCondition);
            var sut = new GbisRule(ruleLogicMock.Object);

            // Act
            await sut.InitializeAsync();

            // Assert
            Assert.Same(failureCondition, sut.Configuration.Constraint);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsRuleResult()
        {
            // Arrange
            var gbisRule = CreateGbisRule();

            // Act
            var ruleResult = await gbisRule.InvokeAsync();

            // Assert
            Assert.IsType<RuleResult>(ruleResult);
        }

        [Fact]
        public async Task InvokeAsync_SetsCorrectValueForWhatWasAddedToTheNotificationTemplate()
        {
            // Arrange
            const string failedValue = "FailedValue";

            var gbisRule = CreateGbisRule(FailedValueFunction);

            // Act
            var ruleResult = await gbisRule.InvokeAsync();

            // Assert
            var stageValidationError = ExtractStageValidationError(ruleResult);
            Assert.Equal(failedValue, stageValidationError.WhatWasAddedToTheNotificationTemplate);
            string? FailedValueFunction(MeasureModel _) => failedValue;
        }

        [Fact]
        public async Task InvokeAsync_SetsCorrectMeasureReferenceNumber()
        {
            // Arrange
            var gbisRule = CreateGbisRule();

            // Act
            var ruleResult = await gbisRule.InvokeAsync();

            // Assert
            var stageValidationError = ExtractStageValidationError(ruleResult);
            Assert.Equal(MeasureReferenceNumber, stageValidationError.MeasureReferenceNumber);
        }

        [Fact]
        public async Task InvokeAsync_SetsCorrectTestNumber()
        {
            // Arrange
            const string testNumber = "TestNumber";

            var gbisRule = CreateGbisRule(testNumber:testNumber);

            // Act
            var ruleResult = await gbisRule.InvokeAsync();

            // Assert
            var stageValidationError = ExtractStageValidationError(ruleResult);
            Assert.Equal(testNumber, stageValidationError.TestNumber);
        }

        private static GbisRule CreateGbisRule(Func<MeasureModel, string?>? failedValueFunction = null, string? testNumber = null)
        {
            var ruleLogicMock = new Mock<IRuleLogic>();
            ruleLogicMock.Setup(x => x.FailureFieldValueFunction).Returns(failedValueFunction ?? DefaultFailedValueFunction);
            ruleLogicMock.Setup(x => x.TestNumber).Returns(testNumber ?? string.Empty);

            var sut = new GbisRule(ruleLogicMock.Object)
            {
                Model = new MeasureModel { MeasureReferenceNumber = MeasureReferenceNumber }
            };

            return sut;

            string? DefaultFailedValueFunction(MeasureModel _) => string.Empty;
        }

        private static StageValidationError ExtractStageValidationError(IRuleResult ruleResult)
        {
            var actualRuleResult = ruleResult as RuleResult;
            var stageValidationError = ((IEnumerable<StageValidationError>)actualRuleResult!.Result).First();
            return stageValidationError;
        }
    }
}
