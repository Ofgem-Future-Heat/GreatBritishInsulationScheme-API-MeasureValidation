using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0621RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781")]
        [InlineData(EligibilityTypes.InFill, "FOX0123456781")]
        public void Gbis0621Rule_WithValidInput_PassesValidation(string eligibilityType, string associatedInfillMeasure2)
        {
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
                AssociatedInfillMeasure2Details = new AssociatedMeasureModelDto
                {
                    MeasureReferenceNumber = associatedInfillMeasure2
                }
            };

            var rule = new Gbis0621RuleLogic();

            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781")]
        [InlineData(EligibilityTypes.InFill, "FOX0123456781")]
        public void Gbis0621Rule_WithInvalidInput_FailsValidation(string eligibilityType, string associatedInfillMeasure2)
        {
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                AssociatedInfillMeasure2 = associatedInfillMeasure2
            };

            var rule = new Gbis0621RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0621", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure2, observedValue);
        }
    }
}