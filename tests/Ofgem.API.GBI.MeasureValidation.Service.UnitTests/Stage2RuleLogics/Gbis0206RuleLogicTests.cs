using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0206RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0206Rule_WithValidInput_PassValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating, string prsSapBandExeption)
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
            var rule = new Gbis0206RuleLogic();
            
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.England, "38.3", "No")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "6", "No")]
        public void Gbis0206Rule_WithInvalidInput_FailsValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating, string prsSapBandExeption)
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
            var rule = new Gbis0206RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0206", rule.TestNumber);
            Assert.Equal(measureModel.PrsSapBandException, observedValue);
        }

        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "60", "Yes" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "40.4", "No" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.Wales, "0", "No" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.Wales, "-1", "No" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.England, "38.5", "Yes" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.England, "45.6", "No" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "38.4", "No" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "55.8", "Yes" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.England, "70", "Yes" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.England, "70", "Yes" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "15.8", "No" };
            yield return new object[] { EligibilityTypes.LILADeclaration, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "7", "No" };
            yield return new object[] { EligibilityTypes.LISocialHousing, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "38.4", "No" };

        }
    }
}
