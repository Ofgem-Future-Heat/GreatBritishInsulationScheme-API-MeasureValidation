using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0306RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0306Rule_WithValidInput_PassValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating, string prsSapBandExeption)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating,
                PrsSapBandException = prsSapBandExeption
            };
            var rule = new Gbis0306RuleLogic();

            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.England, "38.3", "No")]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "6", "No")]
        public void Gbis0306Rule_WithInvalidInput_FailsValidation(string elibilityType, string tenureType, string countryCode, string startingSapRating, string prsSapBandExeption)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = elibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating,
                PrsSapBandException = prsSapBandExeption
            };
            var rule = new Gbis0306RuleLogic();
            
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);
            
            Assert.True(result);
            Assert.Equal("GBIS0306", rule.TestNumber);
            Assert.Equal(measureModel.PrsSapBandException, observedValue);
        }

        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "60", "Yes" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "40.4", "No" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.Wales, "0", "No" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.Wales, "-1", "Yes" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.Wales, "40", "No" };
            yield return new object[] { EligibilityTypes.LISocialHousing, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.Wales, "35.6", "No" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.England, "38.5", "Yes" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.England, "38.5", "No" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "38.4", "Yes" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "55.8", "No" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.England, "70", "No" };
            yield return new object[] { EligibilityTypes.LILADeclaration, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.England, "18.4", "No" };
            yield return new object[] { EligibilityTypes.LISocialHousing, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.England, "31.2", "No" };
        }
    }
}