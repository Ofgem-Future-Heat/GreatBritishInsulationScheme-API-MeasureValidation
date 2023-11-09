using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.ApiClients;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.UnitTests.ApiClients
{
    public class DocumentApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly Mock<ILogger<DocumentApiClient>> _loggerMock;
        private readonly DocumentApiClient _documentApiClient;
        private const string BaseUrl = "http://document.api/";
        private static readonly Uri BaseUri = new(BaseUrl);
        private static readonly Uri RequestUri = new(BaseUri, "GetDocumentsNames");

        public DocumentApiClientTests()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection("DocumentServiceApiUrl").Value).Returns(BaseUrl);

            _handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = _handler.CreateClient();
            httpClient.BaseAddress = BaseUri;

            _loggerMock = new Mock<ILogger<DocumentApiClient>>();
            _documentApiClient = new DocumentApiClient(configurationMock.Object, httpClient, _loggerMock.Object);
        }

        [Fact]
        public async Task GetDocumentsNames_GivenNoDocumentIds_ReturnsNoMappings()
        {
            // Arrange

            // Act
            var actual = await _documentApiClient.GetDocumentsNames(Enumerable.Empty<Guid>());

            // Assert
            Assert.Empty(actual);
        }

        [Theory]
        [MemberData(nameof(DocumentIdsAndMappings))]
        public async Task GetDocumentsNames_GivenDocumentIds_ReturnsAppropriateMappings(IEnumerable<Guid> documentIds, IDictionary<Guid, string> expected)
        {
            // Arrange
            _handler.SetupRequest(HttpMethod.Get, RequestUri)
                .ReturnsJsonResponse(expected);

            // Act
            var actual = await _documentApiClient.GetDocumentsNames(documentIds);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        public static TheoryData<IEnumerable<Guid>, IDictionary<Guid, string>> DocumentIdsAndMappings()
        {
            return new TheoryData<IEnumerable<Guid>, IDictionary<Guid, string>>
            {
                {
                    new[] {
                        new Guid("00000000-0000-0000-0000-000000000001")
                    },
                    new Dictionary<Guid, string>{
                        { new Guid("00000000-0000-0000-0000-000000000001"), "file.csv" },
                    }
                },
                {
                    new[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002") },
                    new Dictionary<Guid, string>{ 
                        { new Guid("00000000-0000-0000-0000-000000000001"), "file.csv" },
                        { new Guid("00000000-0000-0000-0000-000000000002"), "file2.csv" },
                    }
                },
            };
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.NotImplemented)]
        [InlineData(HttpStatusCode.BadGateway)]
        public async Task GetDocumentsNames_NonSuccessStatusCode_ExceptionThrown(HttpStatusCode httpStatusCode)
        {
            // Arrange
            _handler.SetupRequest(HttpMethod.Get, RequestUri)
                .ReturnsResponse(httpStatusCode);

            var documentIds = new[] { new Guid("00000000-0000-0000-0000-000000000001") };

            // Act
            var act = async () => await _documentApiClient.GetDocumentsNames(documentIds);

            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(act);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Tests do not validate log message")]
        public async Task GetDocumentsNames_NonSuccessStatusCode_ErrorLogged()
        {
            // Arrange
            _handler.SetupRequest(HttpMethod.Get, RequestUri)
                .ReturnsResponse(HttpStatusCode.InternalServerError);

            var documentIds = new[] { new Guid("00000000-0000-0000-0000-000000000001") };

            // Act
            var act = async () => await _documentApiClient.GetDocumentsNames(documentIds);

            // Assert
            _ = await Assert.ThrowsAsync<HttpRequestException>(act);
            _loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetDocumentsNames_ExceptionThrown_ExceptionRethrown()
        {
            // Arrange
            _handler.SetupRequest(HttpMethod.Get, RequestUri)
                .ThrowsAsync(new Exception());

            var documentIds = new[] { new Guid("00000000-0000-0000-0000-000000000001") };

            // Act
            var act = async () => await _documentApiClient.GetDocumentsNames(documentIds);

            // Assert
            _ = await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Tests do not validate log message")]
        public async Task GetDocumentsNames_ExceptionThrown_ErrorLogged()
        {
            // Arrange
            _handler.SetupRequest(HttpMethod.Get, RequestUri)
                .ThrowsAsync(new Exception());

            var documentIds = new[] { new Guid("00000000-0000-0000-0000-000000000001") };

            // Act
            var act = async () => await _documentApiClient.GetDocumentsNames(documentIds);

            // Assert
            _ = await Assert.ThrowsAsync<Exception>(act);
            _loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<string>()), Times.Once);
        }
    }
}
