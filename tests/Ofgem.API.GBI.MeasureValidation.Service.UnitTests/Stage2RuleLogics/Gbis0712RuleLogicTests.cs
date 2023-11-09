using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using System.Runtime.Intrinsics.Arm;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0712RuleLogicTests
    {

        [Theory]
        [InlineData("dwp match", "AAA_A01234567890")]
        [InlineData("DWP Match", "AAA_AA01234567890")]
        [InlineData("ECO Eligible Referral", "AAA_A01234567890")]
        [InlineData("eco eligible referral", "AAA_AA01234567890")]
        [InlineData(VerificationMethods.DwpMatch, "AAA_A01234567890")]
        [InlineData(VerificationMethods.EcoEligibleReferral, "AAA_AA01234567890")]
        [InlineData("", "n/a")]
        [InlineData("n/a", "n/a")]
        [InlineData("an other", "n/a")]
        [InlineData("an other", "N/A")]
        public void Gbis0712Rule_WithValidInput_PassValidation(string verificationMethod, string dwpReferenceNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                VerificationMethod = verificationMethod,
                DwpReferenceNumber = dwpReferenceNumber
            };
            var rule = new Gbis0712RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null, "AAA_A0123456789")]
        [InlineData("", "AAA_A0123456789")]
        [InlineData("n/a", "AAA_A0123456789")]
        [InlineData("N/A", "AAA_AA0123456789")]
        [InlineData("\t", null)]
        [InlineData("an other", "")]
        [InlineData("an other", "\t")]
        [InlineData("an other", "NA")]
        [InlineData("an other", "aaa_aa0123456789")]
        [InlineData("an other", "AAA_AA0123456789")]
        public void Gbis0712Rule_WithInvalidInput_FailsValidation(string verificationMethod, string dwpReferenceNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                VerificationMethod = verificationMethod,
                DwpReferenceNumber = dwpReferenceNumber
            };
            var rule = new Gbis0712RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0712", rule.TestNumber);
            Assert.Equal(measureModel.DwpReferenceNumber, observedValue);
        }

    }
}