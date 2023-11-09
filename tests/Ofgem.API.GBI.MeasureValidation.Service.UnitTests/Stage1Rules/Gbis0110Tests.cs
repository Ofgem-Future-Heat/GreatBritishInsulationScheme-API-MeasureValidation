using DotNetRuleEngine.Interface;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0110Tests
    {
        [Theory]
        [InlineData("General Group")]
        [InlineData("LI - LA Declaration")]
        [InlineData("In-fill")]
        public async Task Gbis0110_Pass(string eligibilityType)
        {
            var rule = new Gbis0110
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    EligibilityType = eligibilityType,
                    ReferenceDataDetails = new ReferenceDataDetails()
                    {
                        EligibilityTypesList = GetEligibilityTypes()
                    }
                }
            };

            var result = await rule.InvokeAsync();
            Assert.Null(result);
        }

        public static TheoryData<string?> FailureCaseValues =>
           new TheoryData<string?>
           {
                { null },
                { string.Empty },
                {"Not a valid Eligibility Type" },
                {"IWI_solid_1.7_0.55" },
                {"5. In-fill" }
           };

        [Theory]
        [MemberData(nameof(FailureCaseValues))]
        public async Task Gbis0110_Fail_MeasureReferenceNumber(string eligibilityType)
        {
            var result = await CreateAndInvokeRule(eligibilityType);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.Single((IEnumerable<StageValidationError>)result.Result);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseValues))]
        public async Task Gbis0110_Fail_TestReferenceNumber(string eligibilityType)
        {
            var result = await CreateAndInvokeRule(eligibilityType);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.Single((IEnumerable<StageValidationError>)result.Result);
            Assert.Equal("GBIS0110", error.TestNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseValues))]
        public async Task Gbis0110_Fail_WhatWasAddedToTheNotificationTemplate(string eligibilityType)
        {
            var result = await CreateAndInvokeRule(eligibilityType);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.Single((IEnumerable<StageValidationError>)result.Result);
            Assert.Equal(eligibilityType, error.WhatWasAddedToTheNotificationTemplate);
        }

        private static async Task<IRuleResult> CreateAndInvokeRule(string eligibilityType)
        {
            var rule = new Gbis0110
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    EligibilityType = eligibilityType,
                    ReferenceDataDetails = new ReferenceDataDetails()
                    {
                        EligibilityTypesList = GetEligibilityTypes()
                    }
                }
            };

            var result = await rule.InvokeAsync();
            return result;
        }

        private static IEnumerable<string> GetEligibilityTypes()
        {
            return new List<string>
            {
                "General Group",
                "LI - Help to Heat Group",
                "LI - LA Declaration",
                "LI - Supplier Evidence",
                "LI - Social Housing",
                "In-fill"
            };
        }
    }
}

