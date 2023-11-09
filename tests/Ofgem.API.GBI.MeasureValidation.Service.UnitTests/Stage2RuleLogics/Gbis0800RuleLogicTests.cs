using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0800RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0800Rule_WithValidInput_PassValidation(string measureType, string eligibilityType, string tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                MeasureType = measureType,
                TenureType = tenureType,
            };
            var rule = new Gbis0800RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0800Rule_WithInvalidInput_FailsValidation(string measureType, string eligibilityType, string tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                MeasureType = measureType,
                TenureType = tenureType,
            };
            var rule = new Gbis0800RuleLogic();

            var result = rule.FailureCondition(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0800", rule.TestNumber);
        }
        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { MeasureTypes.Trv, EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied };
            yield return new object[] { MeasureTypes.PAndRt, EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied };
        }
        public static IEnumerable<object[]> InvalidInputArguments()
        {
            yield return new object[] { MeasureTypes.Trv, EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing };
            yield return new object[] { MeasureTypes.PAndRt, EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing };
        }
    }
}
