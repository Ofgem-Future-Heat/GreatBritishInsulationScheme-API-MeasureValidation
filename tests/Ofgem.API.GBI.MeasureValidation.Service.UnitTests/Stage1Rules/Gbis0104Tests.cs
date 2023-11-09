using DotNetRuleEngine.Interface;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0104Tests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("AAA0123456789")]
        [InlineData("ZZZ0000000000")]
        [InlineData("BGT9999999999")]
        public async Task Gbis0104_Pass(string measureReferenceNumber)
        {
            var rule = new Gbis0104()
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = measureReferenceNumber
                }
            };

            var result = await rule.InvokeAsync();
            Assert.Null(result);
        }

        public static TheoryData<string> FailureCaseMrns =>
            new()
            {
                { string.Empty },
                { "A1" },
                { "AAA012345678" },
                { "AA0123456789" },
                { "ABC01234567899" },
                { "ABCD012345678" },
                { "ABC0123456789A" },
            };

        [Theory]
        [MemberData(nameof(FailureCaseMrns))]
        public async Task Gbis0104_Fail_MeasureReferenceNumber(string measureReferenceNumber)
        {
            IRuleResult result = await CreateAndInvokeRule(measureReferenceNumber);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal(measureReferenceNumber, error.MeasureReferenceNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseMrns))]
        public async Task Gbis0104_Fail_TestReferenceNumber(string measureReferenceNumber)
        {
            IRuleResult result = await CreateAndInvokeRule(measureReferenceNumber);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("GBIS0104", error.TestNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseMrns))]
        public async Task Gbis0104_Fail_WhatWasAddedToTheNotificationTemplate(string measureReferenceNumber)
        {
            IRuleResult result = await CreateAndInvokeRule(measureReferenceNumber);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal(measureReferenceNumber, error.MeasureReferenceNumber);
        }
        
        private static async Task<IRuleResult> CreateAndInvokeRule(string measureReferenceNumber)
        {
            var rule = new Gbis0104()
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = measureReferenceNumber
                    
                }
            };

            var result = await rule.InvokeAsync();
            return result;
        }

    }
}