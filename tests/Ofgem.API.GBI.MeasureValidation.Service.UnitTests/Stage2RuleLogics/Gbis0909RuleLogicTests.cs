using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0909RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidTestData))]
        public void Gbis0909Rule_WithValidInput_PassesValidation(string eligibilityType, string privateDomesticPremises)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0909RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string?, string?> ValidTestData() => new()
        {
                { null, null},
                { null, "" },
                { "", null },
                { "", "" },
                { EligibilityTypes.LILADeclaration, CommonTypesConstants.Yes },
                { EligibilityTypes.LISupplierEvidence, CommonTypesConstants.Yes },
                { EligibilityTypes.InFill, CommonTypesConstants.No},
                { EligibilityTypes.InFill, CommonTypesConstants.Yes},
        };

        [Theory]
        [MemberData(nameof(InvalidTestData))]
        public void Gbis0909Rule_WithInvalidInput_FailsValidation(string eligibilityType, string privateDomesticPremises)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0909RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.PrivateDomesticPremises, errorFieldValue);
            Assert.Equal("GBIS0909", rule.TestNumber);
        }

        public static TheoryData<string, string> InvalidTestData() => new()
        {
                { EligibilityTypes.LILADeclaration , CommonTypesConstants.No },
                { EligibilityTypes.LISupplierEvidence, CommonTypesConstants.No },
                { "Li - LA declaration", "no"},
                { "li - supplier evidence ", "No"},
        };

    }
}