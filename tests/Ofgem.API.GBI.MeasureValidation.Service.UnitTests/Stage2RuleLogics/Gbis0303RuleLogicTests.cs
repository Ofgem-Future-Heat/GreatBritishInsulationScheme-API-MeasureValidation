using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0303RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputData))]
        public void Gbis0303Rule_WithValidInput_PassValidation(string eligibilityType, string? tenureType,string? countryCode, string? startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0303RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        public static TheoryData<string?, string?, string?, string?> ValidInputData()
        {
            var validInputData = new TheoryData<string?, string?, string?, string?>
            {
                { EligibilityTypes.LIHelpToHeatGroup.ToLowerInvariant(), TenureTypes.PrivateRentedSector,"GB-WLS", "1" },
                { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector,"GB-ENG", "1" },
                { "In-fill", TenureTypes.PrivateRentedSector,"0", "0" },
                { "In-fill", TenureTypes.PrivateRentedSector.ToUpperInvariant(),"GB-ENG", "0" },
                { EligibilityTypes.LIHelpToHeatGroup, null,null, null },
                { EligibilityTypes.LIHelpToHeatGroup, "","", "" },
                { EligibilityTypes.LIHelpToHeatGroup, " "," ", " " },
            };

            return validInputData;
        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, "GB-WLS", "68.5")]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, "GB-WLS", "-1")]
        public void Gbis0303Rule_WithInvalidInput_FailsValidation(string eligibilityType, string? tenureType,string? countryCode,  string? startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0303RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0303", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
        }
    }
}