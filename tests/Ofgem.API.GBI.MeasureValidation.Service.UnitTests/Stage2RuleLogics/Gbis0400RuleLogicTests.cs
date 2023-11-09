using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0400RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.LISocialHousing, TenureTypes.SocialHousing)]
        [InlineData(EligibilityTypes.LISocialHousing, "social housing (rsl)")]
        [InlineData("LI - Social Housing", TenureTypes.SocialHousing)]
        [InlineData("LI - Social Housing", "social housing (RSL)")]
        public void Gbis0400RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string? tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567890",
                EligibilityType = eligibilityType,
                TenureType = tenureType
            };
            var rule = new Gbis0400RuleLogic();
            var result = rule.FailureCondition(measureModel);
            
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.LISocialHousing, TenureTypes.OwnerOccupied)]
        [InlineData(EligibilityTypes.LISocialHousing, "")]
        public void Gbis0400RuleLogic_WithInvalidInput_FailsValidation(string eligibilityType, string? tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567890",
                EligibilityType = eligibilityType,
                TenureType = tenureType
            };
            var rule = new Gbis0400RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0400", rule.TestNumber);
            Assert.Equal(measureModel.TenureType, observedValue);
        }
    }
}
