using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0202RuleLogicTests
    {
        [Theory]
        [InlineData("General Group", TenureTypes.OwnerOccupied)]
        [InlineData("In-fill", TenureTypes.SocialHousing)]
        [InlineData("general group", null)]
        [InlineData("General group", "")]
        [InlineData("GENERAL Group", " ")]
        public void Gbis0202Rule_WithValidInput_PassValidation(string eligibilityType, string? tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567890",
                EligibilityType = eligibilityType,
                TenureType = tenureType
            };
            var rule = new Gbis0202RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing)]
        public void Gbis0202Rule_WithInvalidInput_FailsValidation(string eligibilityType, string? tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567890",
                EligibilityType = eligibilityType,
                TenureType = tenureType
            };
            var rule = new Gbis0202RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0202", rule.TestNumber);
            Assert.Equal(measureModel.TenureType, observedValue);
        }
    }
}