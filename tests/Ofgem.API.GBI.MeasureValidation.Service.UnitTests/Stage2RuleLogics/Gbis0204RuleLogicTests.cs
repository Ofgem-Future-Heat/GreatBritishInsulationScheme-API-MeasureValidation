using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0204RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.England, "0")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.England, "32")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.England, "68.4")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "0")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "32")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "68.4")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Scotland, "-1")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Scotland, "0")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Scotland, "32")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Scotland, "69")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, null)]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, null, "100")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, null, "32")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, null, "-100")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "sixty nine")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "100.0m")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, CountryCodes.Wales, "100")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, CountryCodes.England, "-100")]
        public void Gbis0204Rule_WithValidInput_PassValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0204RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.England, "-1")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.England, "68.41")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "-1")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "68.5")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "-0.1")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.England, "-10000000")]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, CountryCodes.Wales, "10000000")]
        [InlineData("general group", TenureTypes.PrivateRentedSector, CountryCodes.Wales, "100")]
        [InlineData(EligibilityTypes.GeneralGroup, "private rented sector", CountryCodes.England, "100")]
        public void Gbis0204Rule_WithInvalidInput_FailsValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0204RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0204", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
        }

    }
}