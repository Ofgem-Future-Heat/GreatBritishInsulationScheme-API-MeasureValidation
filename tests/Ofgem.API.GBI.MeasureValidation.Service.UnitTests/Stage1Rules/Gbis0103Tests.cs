using DotNetRuleEngine.Interface;
using Microsoft.Extensions.Options;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0103Tests
    {
        [Theory]
        [InlineData("30/03/2023")]
        [InlineData("31/03/2026")]
        [InlineData("16/07/2023")]
        public async Task Gbis0103_Pass(string dateOfCompletedInstallation)
        {
            var result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            Assert.Null(result);
        }

        public static TheoryData<string> FailureCaseDates =>
            new()
            {
                { "29/03/2023" },
                { "01/04/2026" },
                { "03/05/2022" },
                { "23/08/2028" },
            };

        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0103_Fail_MeasureReferenceNumber(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0103_Fail_TestReferenceNumber(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("GBIS0103", error.TestNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0103_Fail_WhatWasAddedToTheNotificationTemplate(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal(dateOfCompletedInstallation, error.WhatWasAddedToTheNotificationTemplate);
        }

        private static async Task<IRuleResult> CreateAndInvokeRule(string dateOfCompletedInstallation)
        {
            var schemeDetailsOptionsMock = new Mock<IOptions<SchemeDetailsOptions>>();
            schemeDetailsOptionsMock.Setup(x => x.Value).Returns(new SchemeDetailsOptions
                { StartDate = new(2023, 03, 30), EndDate = new(2026, 03, 31) });

            var rule = new Gbis0103(schemeDetailsOptionsMock.Object)
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
