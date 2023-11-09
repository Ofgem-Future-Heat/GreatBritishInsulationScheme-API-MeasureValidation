using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0210RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0210Rule_WithValidInput_PassValidation(string eligibilityType, string countryCode, string councilTaxBand)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                CountryCode = countryCode,
                CouncilTaxBand = councilTaxBand
            };
            var rule = new Gbis0210RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Scotland, ReferenceDataConstants.CouncilTaxBand.F)]
        [InlineData(EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Wales, ReferenceDataConstants.CouncilTaxBand.F)]
        public void Gbis0210Rule_WithInvalidInput_FailsValidation(string eligibilityType, string countryCode, string councilTaxBand)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                CountryCode = countryCode,
                CouncilTaxBand = councilTaxBand
            };
            var rule = new Gbis0210RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0210", rule.TestNumber);
            Assert.Equal(measureModel.CouncilTaxBand, observedValue);
        }

        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.A.ToLower() };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.D.ToLower() };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.E.ToLower() };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.A };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.D };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.E };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.B };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.C.ToLower() };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.E };
            yield return new object[] { EligibilityTypes.LILADeclaration, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.A };
            yield return new object[] { EligibilityTypes.LILADeclaration, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.D.ToLower()};
            yield return new object[] { EligibilityTypes.LILADeclaration, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.E };
            yield return new object[] { EligibilityTypes.LISocialHousing, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.A.ToLower() };
            yield return new object[] { EligibilityTypes.LISocialHousing, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.E };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.B };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.C.ToLower() };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, ReferenceDataConstants.CountryCode.England, ReferenceDataConstants.CouncilTaxBand.E };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Wales, ReferenceDataConstants.CouncilTaxBand.A };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Wales, ReferenceDataConstants.CouncilTaxBand.D.ToLower() };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Wales, ReferenceDataConstants.CouncilTaxBand.E };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Scotland, ReferenceDataConstants.CouncilTaxBand.A };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Scotland, ReferenceDataConstants.CouncilTaxBand.D.ToLower() };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CountryCode.Scotland, ReferenceDataConstants.CouncilTaxBand.E };
        }
    }
}