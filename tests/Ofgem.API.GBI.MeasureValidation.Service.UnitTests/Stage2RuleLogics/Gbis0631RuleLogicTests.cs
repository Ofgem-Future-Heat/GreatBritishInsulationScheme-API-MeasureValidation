using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0631RuleLogicTests
    {
        [Theory]
        [InlineData("ABC0123456781", EligibilityTypes.LISupplierEvidence)]
        [InlineData("ABC0123456781", EligibilityTypes.GeneralGroup)]
        [InlineData(CommonTypesConstants.NotApplicable, EligibilityTypes.InFill)]
        public void Gbis0631Rule_WithValidInput_PassesValidation(string associatedInfillMeasure3, string associatedEligibilityType)
        {
            var measureModel = new MeasureModel
            {
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                AssociatedInfillMeasure3Details = new AssociatedMeasureModelDto
                {
                    EligibilityType = associatedEligibilityType
                }
            };

            var rule = new Gbis0631RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("ABC0123456781", EligibilityTypes.InFill)]
        [InlineData("ABC0123456781", null)]
        public void Gbis0631Rule_WithInvalidInput_FailsValidation(string associatedInfillMeasure3, string associatedEligibilityType)
        {
            var measureModel = new MeasureModel
            {
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                AssociatedInfillMeasure3Details = new AssociatedMeasureModelDto
                {
                    EligibilityType = associatedEligibilityType
                }
            };
            var rule = new Gbis0631RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.Equal("GBIS0631", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure3, observedValue);
            Assert.True(result);
        }
    }
}