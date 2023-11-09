using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0203RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputData))]
        public void Gbis0203Rule_WithValidInput_PassValidation(string eligibilityType, string? tenureType, string? startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0203RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        public static TheoryData<string?, string?, string?> ValidInputData()
        {
            var validInputData = new TheoryData<string?, string?, string?>
            {
                { EligibilityTypes.GeneralGroup.ToLowerInvariant(), TenureTypes.OwnerOccupied, "1" },
                { EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, "1" },
                { "In-fill", TenureTypes.SocialHousing, "0" },
                { "In-fill", TenureTypes.SocialHousing.ToUpperInvariant(), "0" },
                { EligibilityTypes.GeneralGroup, null, null },
                { EligibilityTypes.GeneralGroup, "", "" },
                { EligibilityTypes.GeneralGroup, " ", " " },
            };

            return validInputData;
        }

        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, "68.5")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, "-1")]
        public void Gbis0203Rule_WithInvalidInput_FailsValidation(string eligibilityType, string? tenureType, string? startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0203RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0203", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
        }
    }
}