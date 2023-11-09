using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0714RuleLogicTests
    {

        public static IEnumerable<object[]> PercentageTreatedValues
        {
            get
            {
                for (int i = 1; i <= 66; i++)
                {
                    yield return new object[] { i.ToString() };

                }
                yield return new object[] { "67+" };
            }
        }

        [Theory]
        [MemberData(nameof(PercentageTreatedValues))]
        public void Gbis0714Rule_WithValidInput_PassValidation(string percentageTreated)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                PercentageOfPropertyTreated = percentageTreated
            };
            var rule = new Gbis0714RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\n")]
        [InlineData("-10000000")]
        [InlineData("-1")]
        [InlineData("0")]
        [InlineData("1.1")]
        [InlineData("25.0")]
        [InlineData("66.5")]
        [InlineData("67")]
        [InlineData("68")]
        [InlineData("69")]
        [InlineData("1+")]
        [InlineData("66+")]
        [InlineData("68+")]
        [InlineData("100+")]
        [InlineData("100000000")]
        public void Gbis0714Rule_WithInvalidInput_FailsValidation(string percentageTreated)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                PercentageOfPropertyTreated = percentageTreated
            };
            var rule = new Gbis0714RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0714", rule.TestNumber);
            Assert.Equal(measureModel.PercentageOfPropertyTreated, observedValue);
        }

    }
}