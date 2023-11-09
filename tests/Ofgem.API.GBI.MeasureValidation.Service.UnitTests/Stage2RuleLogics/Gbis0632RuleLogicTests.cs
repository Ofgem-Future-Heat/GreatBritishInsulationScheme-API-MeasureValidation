using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0632RuleLogicTests
    {
        [Theory]
        [InlineData("ABC0123456781", "ABC123456789", CommonTypesConstants.NotApplicable, EligibilityTypes.InFill)]
        [InlineData("ABC0123456781", CommonTypesConstants.NotApplicable, "ABC123456789", EligibilityTypes.InFill)]
        [InlineData(CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, EligibilityTypes.InFill)]
        [InlineData(CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, EligibilityTypes.GeneralGroup)]
        [InlineData("ABC0123456781", "ABC123456789", CommonTypesConstants.NotApplicable, EligibilityTypes.LISupplierEvidence)]
        [InlineData("ABC0123456781", CommonTypesConstants.NotApplicable, "ABC123456789", EligibilityTypes.GeneralGroup)]
        public void Gbis0632Rule_WithValidInput_PassesValidation(string associatedInfillMeasure1, string associatedInfillMeasure2, string associatedInfillMeasure3, string eligibilityType)
        {
            var measureModel = new MeasureModel
            {
                AssociatedInfillMeasure1 = associatedInfillMeasure1,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                EligibilityType = eligibilityType
            };

            var rule = new Gbis0632RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("ABC0123456781", "ABC0123456781", CommonTypesConstants.NotApplicable, EligibilityTypes.InFill)]
        [InlineData("ABC0123456781", CommonTypesConstants.NotApplicable, "ABC0123456781", EligibilityTypes.InFill)]
      
        public void Gbis0632Rule_WithInvalidInput_FailsValidation(string associatedInfillMeasure1, string associatedInfillMeasure2, string associatedInfillMeasure3, string eligibilityType)
        {
            var measureModel = new MeasureModel
            {
                AssociatedInfillMeasure1 = associatedInfillMeasure1,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                EligibilityType = eligibilityType
            };
            var rule = new Gbis0632RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.Equal("GBIS0632", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure1, observedValue);
            Assert.True(result);
        }
    }
}