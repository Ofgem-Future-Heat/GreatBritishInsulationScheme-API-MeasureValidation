using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common
{
    public class GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogicTests
    {
        private const string TestNumber = "TestNumber";
        private readonly GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic ruleLogic;
        private readonly Mock<IInFillMeasureService> _mockInFillMeasureService =  new();

        public GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogicTests()
        {
            ruleLogic = new GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic(TestNumber,
                measureModel => measureModel.AssociatedInfillMeasure1, _mockInFillMeasureService.Object);
        }

        private static MeasureModel CreateMeasureModel(string associatedInfillMeasure1, string eligibilityType = EligibilityTypes.InFill)
        {
            var measureModel = new MeasureModel
            {
                AssociatedInfillMeasure1 = associatedInfillMeasure1,
                EligibilityType = eligibilityType,
            };
            return measureModel;
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
            const string associatedInfillMeasure1 = "AAA";
            var measureModel = CreateMeasureModel(associatedInfillMeasure1);

            // Act
            var observedValue = ruleLogic.FailureFieldValueFunction(measureModel);

            // Assert
            Assert.Equal(associatedInfillMeasure1, observedValue);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "FOX0123456781")]
        public void GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic_WithValidInput_PassesValidation(string eligibilityType, string associatedInfillMeasure1)
        {
            _mockInFillMeasureService.Setup(c => c.IsInfillMeasureAssigned(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var measureModel = CreateMeasureModel(associatedInfillMeasure1, eligibilityType);
            measureModel.MeasureReferenceNumber = "ABC12345678";


            var result = ruleLogic.FailureCondition(measureModel);

            Assert.True(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "FOX0123456781", false)]
        [InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable, false)]
        [InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable, true)]
        [InlineData(EligibilityTypes.GeneralGroup, "FOX0123456781", true)]
        [InlineData(EligibilityTypes.LISupplierEvidence, "FOX0123456781", false)]
        public void GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic_WithInvalidInput_FailsValidation(string eligibilityType, string associatedInfillMeasure1, bool infillMeasureDbCheck)
        {
            _mockInFillMeasureService.Setup(c => c.IsInfillMeasureAssigned(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(infillMeasureDbCheck);

            var measureModel = CreateMeasureModel(associatedInfillMeasure1, eligibilityType);

            var result = ruleLogic.FailureCondition(measureModel);

            Assert.False(result);
        }
    }
}
