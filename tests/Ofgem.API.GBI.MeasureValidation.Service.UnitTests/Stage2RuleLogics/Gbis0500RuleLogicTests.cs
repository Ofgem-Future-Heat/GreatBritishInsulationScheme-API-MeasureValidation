using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0500RuleLogicTests
    {
        [Fact]
        public void Gbis0500Rule_WithValidInput_PassValidation()
        {
            var measureModel = new MeasureModel
            {
                AddressIsVerified = true
            };
            var rule = new Gbis0500RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Fact]
        public void Gbis0500Rule_WithInvalidInput_FailsValidation()
        {
            var measureModel = new MeasureModel
            {
                AddressIsVerified = false,
                BuildingNumber = "123",
                StreetName = "ABC Street",
                PostCode = "AB3 CD5"
            };
            var rule = new Gbis0500RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0500", rule.TestNumber);
            Assert.Equal("123,ABC Street,AB3 CD5", observedValue);
        }
    }
}