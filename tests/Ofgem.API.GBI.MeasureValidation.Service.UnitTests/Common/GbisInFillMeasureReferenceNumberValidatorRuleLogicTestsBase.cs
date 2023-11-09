using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common
{
    public abstract class GbisInFillMeasureReferenceNumberValidatorRuleLogicTestsBase<T>
        where T : GbisInFillMeasureReferenceNumberValidatorRuleLogic
    {
        public abstract T RuleLogic { get; }

        public abstract string TestNumber { get; }

        public abstract Action<MeasureModel, string?> FailureFieldSetterFunction { get; }

        [Theory]
        [MemberData(nameof(GbisInFillMeasureReferenceNumberValidatorRuleLogicTests.SuccessCaseMeasureReferenceNumbers),
            MemberType = typeof(GbisInFillMeasureReferenceNumberValidatorRuleLogicTests))]

        public void GbisMeasureReferenceNumberRuleLogic_WithValidInput_PassValidation(string measureReferenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel();
            FailureFieldSetterFunction(measureModel, measureReferenceNumber);

            // Act
            var result = RuleLogic.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(GbisInFillMeasureReferenceNumberValidatorRuleLogicTests.FailureCaseMeasureReferenceNumbers),
            MemberType = typeof(GbisInFillMeasureReferenceNumberValidatorRuleLogicTests))]

        public void GbisMeasureReferenceNumberRuleLogic_WithInvalidInput_FailsValidation(string measureReferenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel();
            FailureFieldSetterFunction(measureModel, measureReferenceNumber);

            // Act
            var result = RuleLogic.FailureCondition(measureModel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GbisMeasureReferenceNumberRuleLogic_HasCorrectTestNumber()
        {
            // Arrange

            // Act

            // Assert
            Assert.Equal(TestNumber, RuleLogic.TestNumber);
        }

        [Fact]
        public void GbisMeasureReferenceNumberRuleLogic_WithInvalidInput_ErrorHasCorrectFailedValue()
        {
            // Arrange
            const string measureReferenceNumber = "AAA";
            var measureModel = new MeasureModel();
            FailureFieldSetterFunction(measureModel, measureReferenceNumber);

            // Act
            var observedValue = RuleLogic.FailureFieldValueFunction(measureModel);

            // Assert
            Assert.Equal(measureReferenceNumber, observedValue);
        }
    }
}