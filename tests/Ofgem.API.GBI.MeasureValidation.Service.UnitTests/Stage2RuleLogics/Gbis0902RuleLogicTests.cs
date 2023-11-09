using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0902RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidTestData))]
        public void Gbis0902Rule_WithValidInput_PassesValidation(string eligibilityType, string laDeclarationReferenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                LaDeclarationReferenceNumber = laDeclarationReferenceNumber
            };
            var rule = new Gbis0902RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string, string> ValidTestData() => new()
        {
                { "", "N/A"},
                { EligibilityTypes.LILADeclaration, "0123" },
                { EligibilityTypes.LILADeclaration, "" },
                { EligibilityTypes.InFill, "N/A" },
                { EligibilityTypes.GeneralGroup, "N/A" },
        };

        [Theory]
        [MemberData(nameof(InvalidTestData))]
        public void Gbis0902Rule_WithInvalidInput_FailsValidation(string eligibilityType, string laDeclarationReferenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                LaDeclarationReferenceNumber = laDeclarationReferenceNumber

            };
            var rule = new Gbis0902RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.EligibilityType, errorFieldValue);
            Assert.Equal("GBIS0902", rule.TestNumber);
        }

        public static TheoryData<string?, string?> InvalidTestData() => new()
        {
                { null, null },
                { "", "" },
                { EligibilityTypes.InFill, "01234" },
                { EligibilityTypes.GeneralGroup, "012345" },
                { "N/A", "012345" },
        };

    }
}