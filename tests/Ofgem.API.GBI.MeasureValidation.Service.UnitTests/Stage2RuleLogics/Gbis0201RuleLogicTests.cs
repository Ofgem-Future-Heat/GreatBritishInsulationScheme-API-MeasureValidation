using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0201RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, "Yes")]
        [InlineData("In-fill", "No")]
        public void Gbis0201Rule_WithValidInput_PassValidation(string eligibilityType, string privateDomesticPremises)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567890",
                EligibilityType = eligibilityType,
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0201RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, "No")]
        [InlineData(EligibilityTypes.GeneralGroup, null)]
        [InlineData(EligibilityTypes.GeneralGroup, "")]
        [InlineData(EligibilityTypes.GeneralGroup, " ")]
        public void Gbis0201Rule_WithInvalidInput_FailsValidation(string eligibilityType, string privateDomesticPremises)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567890",
                EligibilityType = eligibilityType,
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0201RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0201", rule.TestNumber);
            Assert.Equal(measureModel.PrivateDomesticPremises, observedValue);
        }
    }
}
