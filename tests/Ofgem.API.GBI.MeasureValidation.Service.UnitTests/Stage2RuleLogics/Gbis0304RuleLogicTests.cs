using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0304RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0304Rule_WithValidInput_PassValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0304RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "70")]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "38.3")]
        public void Gbis0304Rule_WithInvalidInput_FailsValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0304RuleLogic();
            
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0304", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
        }

            public static IEnumerable<object[]> InvalidInputArguments()
        {
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "38.5" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "68.4" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "39" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "50" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "68.3" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.Scotland, "15" };
            yield return new object[] { EligibilityTypes.LILADeclaration, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "39" };
            yield return new object[] { EligibilityTypes.LILADeclaration, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Wales, "20" };
            yield return new object[] { EligibilityTypes.LISocialHousing, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.Scotland, "1" };
            yield return new object[] { EligibilityTypes.GeneralGroup, TenureTypes.OwnerOccupied, ReferenceDataConstants.CountryCode.England, "70" };
        }
    }
}

