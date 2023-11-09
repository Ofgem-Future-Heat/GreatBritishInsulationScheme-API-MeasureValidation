using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common
{
    public class GbisInFillMeasureReferenceNumberValidatorRuleLogicTests
    {
        private const string TestNumber = "TestNumber";
        private readonly GbisInFillMeasureReferenceNumberValidatorRuleLogic ruleLogic;

        public GbisInFillMeasureReferenceNumberValidatorRuleLogicTests()
        {
            ruleLogic = new GbisInFillMeasureReferenceNumberValidatorRuleLogic(TestNumber,
                measureModel => measureModel.MeasureReferenceNumber);
        }

        private static MeasureModel CreateMeasureModel(string measureReferenceNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = measureReferenceNumber,
            };
            return measureModel;
        }

        public static TheoryData<string?> SuccessCaseMeasureReferenceNumbers =>
            new()
            {
                null,
                "N/A",
                "AAA0123456789",
                "ZZZ0000000000",
                "BGT9999999999",
            };

        [Theory]
        [MemberData(nameof(SuccessCaseMeasureReferenceNumbers))]
        public void GbisMeasureReferenceValidatorRuleLogic_WithValidInput_PassValidation(string measureReferenceNumber)
        {
            // Arrange
            var measureModel = CreateMeasureModel(measureReferenceNumber);

            // Act
            var result = ruleLogic.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
        }

        public static TheoryData<string> FailureCaseMeasureReferenceNumbers =>
            new()
            {
                string.Empty,
                "A1",
                "AAA012345678",
                "AA0123456789",
                "ABC01234567899",
                "ABCD012345678",
                "ABC0123456789A",
            };

        [Theory]
        [MemberData(nameof(FailureCaseMeasureReferenceNumbers))]
        public void GbisMeasureReferenceValidatorRuleLogic_WithInvalidInput_FailsValidation(
            string measureReferenceNumber)
        {
            // Arrange
            var measureModel = CreateMeasureModel(measureReferenceNumber);

            // Act
            var result = ruleLogic.FailureCondition(measureModel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GbisMeasureReferenceValidatorRuleLogic_HasCorrectTestNumber()
        {
            // Arrange

            // Act

            // Assert
            Assert.Equal(TestNumber, ruleLogic.TestNumber);
        }

        [Fact]
        public void GbisMeasureReferenceValidatorRuleLogic_WithInvalidInput_ErrorHasCorrectFailedValue()
        {
            // Arrange
            const string measureReferenceNumber = "AAA";
            var measureModel = CreateMeasureModel(measureReferenceNumber);

            // Act
            var observedValue = ruleLogic.FailureFieldValueFunction(measureModel);

            // Assert
            Assert.Equal(measureReferenceNumber, observedValue);
        }
    }
}