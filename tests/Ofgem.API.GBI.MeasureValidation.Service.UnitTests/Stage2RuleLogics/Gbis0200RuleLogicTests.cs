using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0200RuleLogicTests
    {
        private readonly ReferenceDataDetails _referenceDataDetails = new()
        {
            TenureTypesList = new List<string>
            {
                "Owner Occupied",
                "Private Rented Sector",
                "Social Housing (RSL)"
            }
        };

        [Theory]
        [InlineData("Owner Occupied")]
        [InlineData("owner occupied")]
        [InlineData("Private Rented Sector")]
        [InlineData("private rented sector")]
        [InlineData("Social Housing (RSL)")]
        [InlineData("social housing (Rsl)")]
        public void Gbis0200Rule_WithValidInput_PassValidation(string tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                TenureType = tenureType,
                ReferenceDataDetails = _referenceDataDetails
            };
            var rule = new Gbis0200RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("N/A")]
        [InlineData("Invalid Tenure Type")]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void Gbis0200Rule_WithInvalidInput_FailsValidation(string tenureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT1234567",
                TenureType = tenureType,
                ReferenceDataDetails = _referenceDataDetails
            };
            var rule = new Gbis0200RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0200", rule.TestNumber);
            Assert.Equal(measureModel.TenureType, observedValue);
        }
    }
}
