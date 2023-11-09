using DotNetRuleEngine.Interface;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0107Tests
    {
        [Theory]
        [InlineData("CWI_0.027")]
        [InlineData("IWI_solid_1.7_0.55")]
        [InlineData("P&RT")]
        public async Task Gbis0107_Pass(string measureType)
        {
            var rule = new Gbis0107
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    MeasureType = measureType,
                    ReferenceDataDetails = new ReferenceDataDetails()
                    {
                        MeasureTypesList = GetMeasureTypes()
                    }
                }
            };

            var result = await rule.InvokeAsync();
            Assert.Null(result);
        }

        public static TheoryData<string?> FailureCaseValues =>
           new()
           {
                { null },
                { string.Empty },
                {"Not a valid MeasureType" },
                {"IWI_solid__1.7_0.55" },
           };

        [Theory]
        [MemberData(nameof(FailureCaseValues))]
        public async Task Gbis0107_Fail_MeasureReferenceNumber(string measureType)
        {
            var result = await CreateAndInvokeRule(measureType);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.Single((IEnumerable<StageValidationError>)result.Result);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseValues))]
        public async Task Gbis0107_Fail_TestReferenceNumber(string measureType)
        {
            var result = await CreateAndInvokeRule(measureType);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.Single((IEnumerable<StageValidationError>)result.Result);
            Assert.Equal("GBIS0107", error.TestNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseValues))]
        public async Task Gbis0107_Fail_WhatWasAddedToTheNotificationTemplate(string measureType)
        {
            var result = await CreateAndInvokeRule(measureType);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.Single((IEnumerable<StageValidationError>)result.Result);
            Assert.Equal(measureType, error.WhatWasAddedToTheNotificationTemplate);
        }

        private static async Task<IRuleResult> CreateAndInvokeRule(string measureType)
        {
            var rule = new Gbis0107
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    MeasureType = measureType,
                    ReferenceDataDetails = new ReferenceDataDetails()
                    {
                        MeasureTypesList = GetMeasureTypes()
                    }
                }
            };

            var result = await rule.InvokeAsync();
            return result;
        }

        private static IEnumerable<MeasureTypeDto> GetMeasureTypes()
        {
            return new List<MeasureTypeDto>
            {
                new MeasureTypeDto { Name = "CWI_0.027" },
				new MeasureTypeDto { Name = "IWI_solid_1.7_0.55" },
				new MeasureTypeDto { Name = "P&RT" }
			};
        }
    }
}

