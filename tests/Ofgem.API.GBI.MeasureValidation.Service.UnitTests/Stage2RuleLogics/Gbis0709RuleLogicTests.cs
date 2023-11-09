using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0709RuleLogicTests
    {
        private readonly ReferenceDataDetails _referenceDataDetails = new()
        {
            VerificationMethodTypesList = new List<string>
                {
                    "WHD Core Group",
                    "DWP Match",
                    "ECO Eligible Referral",
                    "Benefit Letter",
                    "Self Declaration",
                    "PPM",
                    "Non-PPM",
                    "Other",
                    "N/A"
                }
        };

        [Theory]
        [InlineData("WHD Core Group")]
        [InlineData("DWP Match")]
        [InlineData("ECO Eligible Referral")]
        [InlineData("Benefit Letter")]
        [InlineData("Self Declaration")]
        [InlineData("PPM")]
        [InlineData("Non-PPM")]
        [InlineData("Other")]
        [InlineData("N/A")]
        public void Gbis0709Rule_WithValidInput_PassValidation(string verificationMethodType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                VerificationMethod = verificationMethodType,
                ReferenceDataDetails = _referenceDataDetails
            };
            var rule = new Gbis0709RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("1")]
        [InlineData("0")]
        [InlineData("Y")]
        [InlineData("N")]
        [InlineData("  ")]
        public void Gbis0709Rule_WithInvalidInput_FailsValidation(string verificationMethodType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT123457",
                VerificationMethod = verificationMethodType,
                ReferenceDataDetails = _referenceDataDetails
            };
            var rule = new Gbis0709RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0709", rule.TestNumber);
            Assert.Equal(measureModel.VerificationMethod, observedValue);
        }
    };
}
