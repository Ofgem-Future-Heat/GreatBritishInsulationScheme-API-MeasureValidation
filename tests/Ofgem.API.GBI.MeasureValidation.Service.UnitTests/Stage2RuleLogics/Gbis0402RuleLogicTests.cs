using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0402RuleLogicTests
    {
        Mock<IMeasureTypeToInnovationMeasureService> _mockMeasureTypeToInnovationService = new Mock<IMeasureTypeToInnovationMeasureService>();

        [Theory]
        [InlineData(EligibilityTypes.LISocialHousing, "55.5", "EWI_cavity_0.45_0.21")]
        [InlineData(EligibilityTypes.LISocialHousing, "67.1", "EWI_solid_2.0_0.2")]
        public void Gbis0402RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string startingSAPRating, string measureType)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                StartingSAPRating = startingSAPRating,
                MeasureType = measureType
            };
            _mockMeasureTypeToInnovationService.Setup(m => m.GetMeasureTypeInnovationNumbers(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<string>() { "001" }));
            var rule = new Gbis0402RuleLogic(_mockMeasureTypeToInnovationService.Object);
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);

        }

        [Theory]
        [InlineData(EligibilityTypes.LISocialHousing, "55.5", "CWI_0.033")]
        [InlineData(EligibilityTypes.LISocialHousing, "67.1", "CWI_0.033")]
        [InlineData(EligibilityTypes.LISocialHousing, "54.5", "CWI_0.033")]
        [InlineData(EligibilityTypes.LISocialHousing, "68.4", "CWI_0.033")]
        public void Gbis0402RuleLogic_WithValidInput_FailedValidation(string eligibilityType, string startingSAPRating, string measureType)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                StartingSAPRating = startingSAPRating,
                MeasureType = measureType
            };

            _mockMeasureTypeToInnovationService.Setup(m => m.GetMeasureTypeInnovationNumbers(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<string>()));
            var rule = new Gbis0402RuleLogic(_mockMeasureTypeToInnovationService.Object);
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.True(result);

        }
    }
}
