using DotNetRuleEngine.Interface;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0105Tests
    {
        private readonly Mock<ISupplierApiClient> _supplierApiClient;
        public Gbis0105Tests()
        {
            _supplierApiClient = new Mock<ISupplierApiClient>();
            _supplierApiClient.Setup(x => x.GetSuppliersAsync())
                .ReturnsAsync(new[]
            {
                new SupplierResponse { SupplierId = 1, SupplierName = "ABC" }, 
                new SupplierResponse { SupplierId = 2, SupplierName = "DEF" }
            });

        }

        [Theory]
        [InlineData("ABC0123456789")]
        [InlineData("abc0123456789")]
        [InlineData("Abc9999999999")]
        public async Task Gbis0105_ValidMrn_Pass(string measureReferenceNumber)
        {   
            var rule = new Gbis0105(_supplierApiClient.Object)
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
                { "    "},
                { string.Empty },
                { "A1"},
                { "XYZ0123456789" },
                { "0123456789ABC" },
            };

        [Theory]
        [MemberData(nameof(FailureCaseMrns))]
        public async Task Gbis0105_InvalidMrn_ReturnsMrn(string measureReferenceNumber)
        {
            IRuleResult result = await CreateAndInvokeRule(measureReferenceNumber);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal(measureReferenceNumber, error.MeasureReferenceNumber);
        }

        [Theory]
        [MemberData((nameof(FailureCaseMrns)))]
        public async Task Gbis0105_InvalidMrn_ReturnsTestReferenceNumber(string measureReferenceNumber)
        {
            IRuleResult result = await CreateAndInvokeRule(measureReferenceNumber);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("GBIS0105", error.TestNumber);
        }

        [Theory]
        [MemberData((nameof(FailureCaseMrns)))]
        public async Task Gbis0105_InvalidMrn_ReturnsWhatWasAdded(string measureReferenceNumber)
        {
            IRuleResult result = await CreateAndInvokeRule(measureReferenceNumber);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal(measureReferenceNumber, error.WhatWasAddedToTheNotificationTemplate);
        }



        private async Task<IRuleResult> CreateAndInvokeRule(string measureReferenceNumber)
        {
            var rule = new Gbis0105(_supplierApiClient.Object)
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
