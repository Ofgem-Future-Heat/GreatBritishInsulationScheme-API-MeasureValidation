using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0703RuleLogicTests
    {
        [Theory]
        [InlineData("Installation Co.")]
        [InlineData("#1 Best Installation Company")]
        [InlineData("WeAreACompanyOperatingInTheUnitedKingdomWhichInstallsInsulationToCustomersInTheUnitedKingdomWithACompanyNameContainingUnderOneHundredFiftyCharacters")] //148 characters
        [InlineData("WeAreACompanyOperatingInTheUnitedKingdomWhichInstallsInsulationToCustomersInEnglandScotlandAndWalesWithACompanyNameContainingOneHundredFiftyCharacters")] //150 characters
        public void Gbis0703Rule_WithValidInput_PassValidation(string installerName)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                InstallerName = installerName,
            };
            var rule = new Gbis0703RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("WeAreACompanyOperatingInTheUnitedKingdomThatInstallsInsulationToCustomersInTheUnitedKingdomWithACompanyNameThatContainsOverOneHundredAndFiftyCharacters")]  //151 characters
        [InlineData("WeAreACompanyOperatingInTheUnitedKingdomThatInstallsInsulationToOurCustomersInEnglandScotlandAndWalesWithACompanyNameThatIsOverOneHundredAndFiftyCharacters")]  //155 characters
        public void Gbis0703Rule_WithInvalidIput_FailsValidation(string installerName)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                InstallerName = installerName,
            };
            var rule = new Gbis0703RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0703", rule.TestNumber);
            Assert.Equal(measureModel.InstallerName, observedValue);
        }
    }
}
