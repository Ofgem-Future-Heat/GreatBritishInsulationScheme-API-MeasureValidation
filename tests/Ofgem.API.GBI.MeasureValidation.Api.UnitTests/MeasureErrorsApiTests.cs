using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using System.Net;

namespace Ofgem.API.GBI.MeasureValidation.Api.UnitTests
{
    public class MeasureErrorsApiTests
    {
        private readonly Mock<IErrorsReportService> _errorsReportService;

        public MeasureErrorsApiTests()
        {
            _errorsReportService = new Mock<IErrorsReportService>();
            _errorsReportService.Setup(x => x.GetErrorsReport(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());
        }

        [Fact]
        public async void GetErrorsReport_Returns_StatusCodeOk()
        {
            await using var application = new TestApplicationFactory(x => 
            {
                x.AddSingleton(_errorsReportService.Object);
            });

            var client = application.CreateClient();
            var documentId = Guid.NewGuid();
            var response = await client.GetAsync($"/GetErrorsReport/{documentId}/Stage");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void GetErrorsReport_Returns_BadRequest()
        {
            await using var application = new TestApplicationFactory(x =>
            {
                x.AddSingleton(_errorsReportService.Object);
            });

            var client = application.CreateClient();
            var documentId = Guid.Empty;
            var response = await client.GetAsync($"/GetErrorsReport/{documentId}/Stage");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #region GetLatestFilesWithErrorsMetadata API Endpoint Tests

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetLatestFilesWithErrorsMetadata_EmptyOrWhitespaceSupplierIdParameterValue_BadRequestStatusCode(string supplierId)
        {
            // Arrange
            await using var application = new TestApplicationFactory(_ => { });
            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/GetLatestFilesWithErrorsMetadata?supplierName={supplierId}&");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_ExceptionThrown_StatusCodeIsInternalServerError()
        {
            // Arrange
            const string supplierId = "SupplierID";
            var filesWithErrorsMetadataServiceMock = new Mock<IFilesWithErrorsMetadataService>();
            filesWithErrorsMetadataServiceMock.Setup(x => x.GetLatestFilesWithErrorsMetadata(supplierId)).ThrowsAsync(new Exception());

            await using var application = new TestApplicationFactory(x =>
            {
                x.AddTransient(_ => filesWithErrorsMetadataServiceMock.Object);
            });

            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/GetLatestFilesWithErrorsMetadata?supplierName={supplierId}&");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_ExceptionThrown_ErrorLogged()
        {
            // Arrange
            const string supplierId = "SupplierID";
            var filesWithErrorsMetadataServiceMock = new Mock<IFilesWithErrorsMetadataService>();
            filesWithErrorsMetadataServiceMock.Setup(x => x.GetLatestFilesWithErrorsMetadata(supplierId)).ThrowsAsync(new Exception());
            var loggerMock = new Mock<ILogger<Program>>();

            await using var application = new TestApplicationFactory(x =>
            {
                x.AddTransient(_ => filesWithErrorsMetadataServiceMock.Object);
                x.AddTransient(_ => loggerMock.Object);
            });

            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/GetLatestFilesWithErrorsMetadata?supplierName={supplierId}&");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_ErrorFilesFound_StatusCodeIsOk()
        {
            // Arrange
            const string supplierId = "SupplierID";
            var filesWithErrorsMetadataServiceMock = new Mock<IFilesWithErrorsMetadataService>();
            filesWithErrorsMetadataServiceMock.Setup(x => x.GetLatestFilesWithErrorsMetadata(supplierId))
                .ReturnsAsync(new FilesWithErrorsMetadata { FilesWithErrors = new[] { new FileWithErrorsMetadata() } });

            await using var application = new TestApplicationFactory(x =>
            {
                x.AddTransient(_ => filesWithErrorsMetadataServiceMock.Object);
            });

            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/GetLatestFilesWithErrorsMetadata?supplierName={supplierId}&");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_ErrorFilesNotFound_StatusCodeIsOk()
        {
            // Arrange
            const string supplierId = "SupplierID";
            var filesWithErrorsMetadataServiceMock = new Mock<IFilesWithErrorsMetadataService>();
            filesWithErrorsMetadataServiceMock.Setup(x => x.GetLatestFilesWithErrorsMetadata(supplierId))
                .ReturnsAsync(new FilesWithErrorsMetadata { FilesWithErrors = Array.Empty<FileWithErrorsMetadata>() });

            await using var application = new TestApplicationFactory(x =>
            {
                x.AddTransient(_ => filesWithErrorsMetadataServiceMock.Object);
            });

            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/GetLatestFilesWithErrorsMetadata?supplierName={supplierId}&");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion
    }
}
