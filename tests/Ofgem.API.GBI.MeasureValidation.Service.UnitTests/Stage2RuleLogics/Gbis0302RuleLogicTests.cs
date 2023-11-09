using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0302RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputData))]
        public void Gbis0302Rule_WithValidInput_PassValidation(string eligibilityType, string? tenureType, string? startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0302RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        public static TheoryData<string?, string?, string?> ValidInputData()
        {
            var validInputData = new TheoryData<string?, string?, string?>
            {
                { EligibilityTypes.LIHelpToHeatGroup.ToLowerInvariant(), TenureTypes.OwnerOccupied, "1" },
                { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, "1" },
                { "In-fill", TenureTypes.SocialHousing, "0" },
                { "In-fill", TenureTypes.SocialHousing.ToUpperInvariant(), "0" },
                { EligibilityTypes.LIHelpToHeatGroup, null, null },
                { EligibilityTypes.LIHelpToHeatGroup, "", "" },
                { EligibilityTypes.LIHelpToHeatGroup, " ", " " },
            };

            return validInputData;
        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, "68.5")]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, "-1")]
        public void Gbis0302Rule_WithInvalidInput_FailsValidation(string eligibilityType, string? tenureType, string? startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0302RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0302", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
        }
    }
}