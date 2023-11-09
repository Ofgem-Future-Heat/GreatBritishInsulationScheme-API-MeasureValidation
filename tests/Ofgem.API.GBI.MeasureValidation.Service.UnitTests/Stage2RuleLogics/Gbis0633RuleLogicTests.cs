using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0633RuleLogicTests
    {
        [Theory]
        [InlineData("ABC0123456781", "ABC123456789", CommonTypesConstants.NotApplicable, EligibilityTypes.InFill)]
        [InlineData("ABC0123456781", CommonTypesConstants.NotApplicable, "ABC123456789", EligibilityTypes.InFill)]
        [InlineData(CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, EligibilityTypes.InFill)]
        [InlineData(CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, EligibilityTypes.GeneralGroup)]
        [InlineData("ABC0123456781", "ABC123456789", CommonTypesConstants.NotApplicable, EligibilityTypes.LISupplierEvidence)]
        [InlineData("ABC0123456781", CommonTypesConstants.NotApplicable, "ABC123456789", EligibilityTypes.GeneralGroup)]
        public void Gbis0633Rule_WithValidInput_PassesValidation(string associatedInfillMeasure2, string associatedInfillMeasure1, string associatedInfillMeasure3, string eligibilityType)
        {
            var measureModel = new MeasureModel
            {
                AssociatedInfillMeasure1 = associatedInfillMeasure1,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                EligibilityType = eligibilityType
            };

            var rule = new Gbis0633RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("ABC0123456781", "ABC0123456781", CommonTypesConstants.NotApplicable, EligibilityTypes.InFill)]
        [InlineData("ABC0123456781", CommonTypesConstants.NotApplicable, "ABC0123456781", EligibilityTypes.InFill)]
      
        public void Gbis0633Rule_WithInvalidInput_FailsValidation(string associatedInfillMeasure2, string associatedInfillMeasure1, string associatedInfillMeasure3, string eligibilityType)
        {
            var measureModel = new MeasureModel
            {
                AssociatedInfillMeasure1 = associatedInfillMeasure1,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                EligibilityType = eligibilityType
            };
            var rule = new Gbis0633RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.Equal("GBIS0633", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure2, observedValue);
            Assert.True(result);
        }
    }
}