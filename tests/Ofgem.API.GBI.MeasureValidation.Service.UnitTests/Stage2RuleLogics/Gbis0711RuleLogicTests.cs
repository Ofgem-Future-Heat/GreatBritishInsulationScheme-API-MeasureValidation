using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0711RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidVerificationMethodsAndDwpReferenceNumbers))]
        public void Gbis0711Rule_WithValidInput_PassValidation(string verificationMethod, string dwpReferenceNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                VerificationMethod = verificationMethod,
                DwpReferenceNumber = dwpReferenceNumber
            };
            var rule = new Gbis0711RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        public static IEnumerable<object[]> ValidVerificationMethodsAndDwpReferenceNumbers()
        {
            yield return new object[] { "ECO Eligible Referral", "AAA_A0" };
            yield return new object[] { "eco eligible referral", "aaa_a0" };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" + new string('0', 44) };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" + new string('0', 45) };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA0" };
            yield return new object[] { "eco eligible referral", "aaa_aa0" };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" + new string('0', 43) };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" + new string('0', 44) };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "eer_a0" };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "eer_a1234567890" };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "EER_A1234567890" };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "EER_A12039475753984734567890" };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "eer_aa0" };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "eer_de1234567890" };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "EER_AB1234567890" };
            yield return new object[] { VerificationMethods.EcoEligibleReferral, "EER_AB123456432152342357890" };
        }

        [Theory]
        [MemberData(nameof(InvalidVerificationMethodsAndDwpReferenceNumbers))]
        public void Gbis0711Rule_WithInvalidInput_FailsValidation(string verificationMethod, string dwpReferenceNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                VerificationMethod = verificationMethod,
                DwpReferenceNumber = dwpReferenceNumber
            };
            var rule = new Gbis0711RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0711", rule.TestNumber);
            Assert.Equal(measureModel.DwpReferenceNumber, observedValue);
        }

        public static IEnumerable<object?[]> InvalidVerificationMethodsAndDwpReferenceNumbers()
        {
            yield return new object?[] { "ECO Eligible Referral", null };
            yield return new object[] { "ECO Eligible Referral", "" };
            yield return new object[] { "ECO Eligible Referral", "\t" };
            yield return new object[] { "ECO Eligible Referral", "n/a" };
            yield return new object[] { "ECO Eligible Referral", "N/A" };
            yield return new object[] { "ECO Eligible Referral", "err" };
            yield return new object[] { "ECO Eligible Referral", "ERR" };
            yield return new object[] { "ECO Eligible Referral", "aaa_a" };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" + new string('0', 46) };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" + new string('0', 47) };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" + new string('0', 100) };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" + new string('A', 44) };
            yield return new object[] { "ECO Eligible Referral", "AAA_A" + new string('A', 45) };
            yield return new object[] { "ECO Eligible Referral", "aaa_aa" };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" + new string('0', 45) };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" + new string('0', 46) };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" + new string('0', 100) };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" + new string('A', 43) };
            yield return new object[] { "ECO Eligible Referral", "AAA_AA" + new string('A', 44) };
        }

    }
}