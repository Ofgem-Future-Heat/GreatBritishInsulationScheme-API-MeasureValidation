using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.ApiClients;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Constants;
using System.Net;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.UnitTests.ApiClients
{
    public class AddressApiClientTests
    {
        private readonly IAddressApiClient _addressApiClient;
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private const string BaseUrl = "http://address-verification/";
        private static readonly Uri BaseUri = new(BaseUrl);
        private static readonly Uri RequestUri = new(BaseUri, InfrastructureConstants.AddressApiClient.AddressValidationUrl);

        public AddressApiClientTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = _handlerMock.CreateClient();
            httpClient.BaseAddress = BaseUri;
            Mock<ILogger<AddressApiClient>> loggerMock = new();

            _addressApiClient = new AddressApiClient(httpClient, loggerMock.Object);
        }

        [Fact]
        public async Task ValidateAddress_GivenAddressValidationModel_ReturnsAddressValidationModel()
        {
            // Arrange
            _handlerMock.SetupRequest(HttpMethod.Post, RequestUri)
                .ReturnsJsonResponse(new List<AddressValidationResponse>() { new() { IsValid = true } });

            // Act
            var actual = await _addressApiClient.ValidateAddressAsync(new List<AddressValidationModel>());

            // Assert
            Assert.True(actual != null);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.NotImplemented)]
        [InlineData(HttpStatusCode.BadGateway)]
        public async Task ValidateAddress_NonSuccessStatusCode_ExceptionThrown(HttpStatusCode httpStatusCode)
        {
            // Arrange
            _handlerMock.SetupRequest(HttpMethod.Post, RequestUri)
                .ReturnsResponse(httpStatusCode);

            // Act
            var actual = async () => await _addressApiClient.ValidateAddressAsync(new List<AddressValidationModel>());

            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(actual);
        }
    }
}
