using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0710RuleLogicTests
    {

        [Theory]
        [InlineData("DWP Match", "0000000000")]
        [InlineData("dwp match", "8888888888")]
        [InlineData(VerificationMethods.DwpMatch, "1234567890")]
        [InlineData(VerificationMethods.DwpMatch, "0987654321")]
        [InlineData("an other", "1234567890")]
        [InlineData("an other", "12345678901")]
        [InlineData("an other", "123456789")]
        [InlineData("an other", null)]
        [InlineData("an other", "")]
        [InlineData("an other", "\t")]
        public void Gbis0710Rule_WithValidInput_PassValidation(string verificationMethod, string dwpReferenceNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                VerificationMethod = verificationMethod,
                DwpReferenceNumber = dwpReferenceNumber
            };
            var rule = new Gbis0710RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("DWP Match", "000000000")]
        [InlineData("DWP Match", "00000000001")]
        [InlineData("DWP Match", "----------")]
        [InlineData("dwp match", "---0000000")]
        [InlineData(VerificationMethods.DwpMatch, "a0000000000")]
        [InlineData(VerificationMethods.DwpMatch, "aaa00000000")]
        [InlineData("DWP Match", "00000000aaa")]
        [InlineData("DWP Match", "aaaaabbbbb")]
        [InlineData("DWP Match", null)]
        [InlineData("DWP Match", "")]
        [InlineData("DWP Match", "\t")]
        public void Gbis0710Rule_WithInvalidInput_FailsValidation(string verificationMethod, string dwpReferenceNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                VerificationMethod = verificationMethod,
                DwpReferenceNumber = dwpReferenceNumber
            };
            var rule = new Gbis0710RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0710", rule.TestNumber);
            Assert.Equal(measureModel.DwpReferenceNumber, observedValue);
        }

    }
}