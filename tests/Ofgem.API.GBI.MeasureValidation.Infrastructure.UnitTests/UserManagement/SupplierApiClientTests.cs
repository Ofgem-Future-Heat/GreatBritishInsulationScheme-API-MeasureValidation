using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.UserManagement;
using System.Net;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.UnitTests.UserManagement
{
    public class SupplierApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly SupplierApiClient _supplierApiClient;
        private const string BaseUrl = "http://user-management/";
        private static readonly Uri BaseUri = new(BaseUrl);
        private static readonly Uri RequestUri = new(BaseUri, "suppliers");

       
        public SupplierApiClientTests()
        {
            _handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = _handler.CreateClient();
            httpClient.BaseAddress = BaseUri;


            Mock<ILogger<SupplierApiClient>> loggerMock = new();
            _supplierApiClient = new SupplierApiClient(httpClient, loggerMock.Object);

        }

        [Theory]
        [MemberData(nameof(SupplierList))]
        public async Task GetSuppliersAsync_SuccesfulApiCall_ReturnsSuppliers(IEnumerable<SupplierResponse> expected)
        {
            _handler.SetupRequest(HttpMethod.Get, RequestUri)
                .ReturnsJsonResponse(expected);

            var actual = await _supplierApiClient.GetSuppliersAsync();

            Assert.Equivalent(actual, expected);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.NotImplemented)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.PreconditionFailed)]

        public async Task GetSuppliersAsync_NonSuccessStatusCode_ExceptionThrown(HttpStatusCode httpStatusCode)
        {
            _handler.SetupRequest(HttpMethod.Get, RequestUri)
                            .ReturnsResponse(httpStatusCode);

            var actual = async() => await _supplierApiClient.GetSuppliersAsync();

            await Assert.ThrowsAsync<HttpRequestException>(actual);
        }

        public static TheoryData<IEnumerable<SupplierResponse>> SupplierList()
        {
            return new TheoryData<IEnumerable<SupplierResponse>>
            {
                new List<SupplierResponse>
                {
                    new()
                    {
                        SupplierId = 1,
                        SupplierName = "EON"
                    },
                    new()
                    {
                    SupplierId = 2,
                    SupplierName = "BGT"
                    }
                },
                new List<SupplierResponse>
                {
                    new()
                    {
                        SupplierId = 3,
                        SupplierName = "SSE"
                    },
                    new()
                    {
                    SupplierId = 4,
                    SupplierName = "BLT"
                    }
                },
            };  
        }
    } 
}
