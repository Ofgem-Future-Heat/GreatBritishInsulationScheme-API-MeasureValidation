using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0910RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidStartingSapRatings))]
        public void Gbis0910Rule_WithValidInput_PassValidation(string eligibilityType, string tenureType, string startingSapRating)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0910RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string, string, string> ValidStartingSapRatings() => new()
        {
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "0" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "1" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "25" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "54" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "54.4" },
            { EligibilityTypes.InFill, TenureTypes.OwnerOccupied , "100" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.PrivateRentedSector, "-100" },
        };

        [Theory]
        [MemberData(nameof(InvalidStartingSapRatings))]
        public void Gbis0910Rule_WithInvalidInput_FailsValidation(string eligibilityType, string tenureType, string startingSapRating)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0910RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
            Assert.Equal("GBIS0910", rule.TestNumber);
        }

        public static TheoryData<string, string, string?> InvalidStartingSapRatings() => new()
        {
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "-100" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "-1" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "-0.1" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "54.41" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "55" },
            { EligibilityTypes.LISupplierEvidence, TenureTypes.OwnerOccupied , "100" },
        };

    }
}