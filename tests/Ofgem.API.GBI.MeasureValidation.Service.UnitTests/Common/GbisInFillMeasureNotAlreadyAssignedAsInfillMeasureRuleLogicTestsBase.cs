using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common
{
    public abstract class GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogicTestsBase<T>
        where T : GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic
    {
        public abstract Func<IInFillMeasureService, T> RuleLogicCreator { get; }

        public abstract string TestNumber { get; }

        public abstract Action<MeasureModel, string?> FailureFieldSetterFunction { get; }

        private readonly Mock<IInFillMeasureService> _mockInFillMeasureService = new();

        [Fact]
        public void GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic_HasCorrectTestNumber()
        {
            // Arrange
            var ruleLogic = RuleLogicCreator(_mockInFillMeasureService.Object);

            // Act

            // Assert
            Assert.Equal(TestNumber, ruleLogic.TestNumber);
        }

        [Fact]
        public void GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic_WithInvalidInput_ErrorHasCorrectFailedValue()
        {
            // Arrange
            const string associatedInfillMeasure1 = "AAA";
            var measureModel = CreateMeasureModel(associatedInfillMeasure1);
            var ruleLogic = RuleLogicCreator(_mockInFillMeasureService.Object);

            // Act
            var observedValue = ruleLogic.FailureFieldValueFunction(measureModel);

            // Assert
            Assert.Equal(associatedInfillMeasure1, observedValue);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "FOX0123456781")]
        public void GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic_WithValidInput_PassesValidation(string eligibilityType, string associatedInfillMeasure)
        {
            _mockInFillMeasureService.Setup(c => c.IsInfillMeasureAssigned(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "ABC12345678",
                EligibilityType = eligibilityType,
            };
            FailureFieldSetterFunction(measureModel, associatedInfillMeasure);

            var rule = RuleLogicCreator(_mockInFillMeasureService.Object);

            var result = rule.FailureCondition(measureModel);

            Assert.True(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "FOX0123456781", false)]
        [InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable, false)]
        [InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable, true)]
        [InlineData(EligibilityTypes.GeneralGroup, "FOX0123456781", true)]
        [InlineData(EligibilityTypes.LISupplierEvidence, "FOX0123456781", false)]
        public void GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic_WithInvalidInput_FailsValidation(string eligibilityType, string associatedInfillMeasure, bool infillMeasureDbCheck)
        {
            _mockInFillMeasureService.Setup(c => c.IsInfillMeasureAssigned(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(infillMeasureDbCheck);

            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
            };
            FailureFieldSetterFunction(measureModel, associatedInfillMeasure);

            var rule = RuleLogicCreator(_mockInFillMeasureService.Object);

            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        private MeasureModel CreateMeasureModel(string associatedInfillMeasure, string eligibilityType = EligibilityTypes.InFill)
        {
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
            };
            FailureFieldSetterFunction(measureModel, associatedInfillMeasure);

            return measureModel;
        }
    }
}
