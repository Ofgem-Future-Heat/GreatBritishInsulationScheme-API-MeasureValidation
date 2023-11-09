using DotNetRuleEngine.Interface;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0102Tests
    {

        [Theory]
        [InlineData("01/04/2023")]
        [InlineData("01/01/2021")]
        [InlineData("16/05/2023")]
        public async Task Gbis0102_Pass_Other_Dates(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            Assert.Null(result);
        }

        public static TheoryData<string> FailureCaseDates =>
            new()
            {
                { "26/05/2023" },
                { "27/05/2023" },
                { "01/04/2026" },
                { "05/06/2027" },
            };

        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0102_Fail_MeasureReferenceNumber(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0102_Fail_TestReferenceNumber(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("GBIS0102", error.TestNumber);
        }

        private static async Task<IRuleResult> CreateAndInvokeRule(string dateOfCompletedInstallation)
        {
            var mockTimeProvider = new Mock<TimeProviderImplementation>();
            mockTimeProvider.Setup(timeProvider => timeProvider.GetLocalNow()).Returns(new DateTime(2023, 05, 25));

            var rule = new Gbis0102(mockTimeProvider.Object)
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    DateOfCompletedInstallation = dateOfCompletedInstallation
                }
            };

            var result = await rule.InvokeAsync();
            return result;
        }
    }
}
