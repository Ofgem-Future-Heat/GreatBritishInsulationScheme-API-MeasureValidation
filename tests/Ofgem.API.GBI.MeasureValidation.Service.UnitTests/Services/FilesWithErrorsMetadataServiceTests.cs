using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class FilesWithErrorsMetadataServiceTests
    {
        private readonly FilesWithErrorsMetadataService _filesWithErrorsMetadataService;
        private readonly Mock<IMeasureRepository> _measureRepositoryMock;
        private readonly Mock<IDocumentApiClient> _documentApiClientMock;

        public FilesWithErrorsMetadataServiceTests()
        {
            _measureRepositoryMock = new Mock<IMeasureRepository>();
            _documentApiClientMock = new Mock<IDocumentApiClient>();
            Mock<ILogger<FilesWithErrorsMetadataService>> loggerMock = new();
            _filesWithErrorsMetadataService = new FilesWithErrorsMetadataService(
                _measureRepositoryMock.Object, loggerMock.Object, _documentApiClientMock.Object);
        }

        [Fact]
        public async void GetLatestFilesWithErrorsMetadata_Returns_Result()
        {
            //Arrange
            var supplierName = "SupplierName";
            _measureRepositoryMock.Setup(x => x.GetLatestFilesWithErrors(supplierName)).ReturnsAsync(new FilesWithErrorsMetadata { FilesWithErrors = Array.Empty<FileWithErrorsMetadata>() });

            //Act
            var actual = await _filesWithErrorsMetadataService.GetLatestFilesWithErrorsMetadata(supplierName);

            //Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.FilesWithErrors);
        }

        [Theory]
        [MemberData(nameof(InvalidSupplierNameTestParameters))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Use type parameter to infer/allow specific exception assertion")]
        public async void GetLatestFilesWithErrorsMetadata_InvalidSupplierName_ThrowsArgumentException<T>(string? supplierName, T exceptionType) where T:Exception
        {
            //Arrange

            //Act
            var act = async () => await _filesWithErrorsMetadataService.GetLatestFilesWithErrorsMetadata(supplierName);

            //Assert
            await Assert.ThrowsAsync<T>(act);
        }

        public static TheoryData<string?, ArgumentException> InvalidSupplierNameTestParameters()
        {
            return new TheoryData<string?, ArgumentException>
            {
                {null, new ArgumentNullException() },
                {string.Empty, new ArgumentException() },
                {"   ", new ArgumentException() },
            };
        }

        [Fact]
        public async void GetLatestFilesWithErrorsMetadata_ReturnsResultWithFileName()
        {
            //Arrange
            var supplierName = "SupplierName";
            var documentId = Guid.NewGuid();
            var fileName = "filename.csv";

            _measureRepositoryMock.Setup(x => x.GetLatestFilesWithErrors(supplierName)).ReturnsAsync(
                new FilesWithErrorsMetadata
                {
                    FilesWithErrors = new[] { new FileWithErrorsMetadata{ DocumentId = documentId },
                    },
            });

            _documentApiClientMock.Setup(x => x.GetDocumentsNames(new[] { documentId }))
                .ReturnsAsync(new Dictionary<Guid, string> { { documentId, fileName } });

            //Act
            var actual = await _filesWithErrorsMetadataService.GetLatestFilesWithErrorsMetadata(supplierName);

            //Assert
            Assert.Equal(fileName, actual.FilesWithErrors.Single().FileName);
        }

        [Fact]
        public async void GetLatestFilesWithErrorsMetadata_WhenExceptionThrown_ExceptionIsRethrown()
        {
            //Arrange
            var supplierName = "SupplierName";

            _measureRepositoryMock.Setup(x => x.GetLatestFilesWithErrors(supplierName)).Throws<Exception>();

            //Act
            var act = async () => await _filesWithErrorsMetadataService.GetLatestFilesWithErrorsMetadata(supplierName);

            //Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async void GetAllFilesWithErrorsMetadata_Returns_Result()
        {
            //Arrange
            var supplierName = "SupplierName";
            _measureRepositoryMock.Setup(x => x.GetAllFilesWithErrors(supplierName)).ReturnsAsync(new FilesWithErrorsMetadata { FilesWithErrors = Array.Empty<FileWithErrorsMetadata>() });

            //Act
            var actual = await _filesWithErrorsMetadataService.GetAllFilesWithErrorsMetadata(supplierName);

            //Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.FilesWithErrors);
        }

        [Fact]
        public async void GetAllFilesWithErrorsMetadata_ReturnsMultipleResultsWithFileNameAndStage()
        {
            //Arrange
            var supplierName = "SupplierName";
            var document1Id = Guid.NewGuid();
            var document2Id = Guid.NewGuid();
            var file1Name = "filename1.csv";
            var file2Name = "filename2.csv";

            _measureRepositoryMock.Setup(x => x.GetAllFilesWithErrors(supplierName)).ReturnsAsync(
                new FilesWithErrorsMetadata
                {
                    FilesWithErrors = new[] { 
                        new FileWithErrorsMetadata{ DocumentId = document1Id },
                        new FileWithErrorsMetadata{ DocumentId = document2Id },
                    },
                });

               _documentApiClientMock.Setup(x => x.GetDocumentsNames(new[] { document1Id, document2Id }))
                    .ReturnsAsync(new Dictionary<Guid, string>() { { document1Id, file1Name }, { document2Id, file2Name } });

            //Act
            var actual = await _filesWithErrorsMetadataService.GetAllFilesWithErrorsMetadata(supplierName);

            //Assert
            Assert.Equal(file1Name, actual.FilesWithErrors.ElementAt(0).FileName);
            Assert.Equal(file2Name, actual.FilesWithErrors.ElementAt(1).FileName);
        }

        [Fact]
        public async Task GetAllFilesWithErrorsMetadata_ReturnsResultsSameDocumentNameAndDifferentStages()
        {
            //Arrange
            var supplierName = "SupplierName";
            var document1Id = Guid.NewGuid();
            var file1Name = "filename1.csv";
            var errorStage1 = "Stage 1";
            var errorStage2 = "Stage 2";

            _measureRepositoryMock.Setup(x => x.GetAllFilesWithErrors(supplierName)).ReturnsAsync(
                new FilesWithErrorsMetadata
                {
                    FilesWithErrors = new[] {
                        new FileWithErrorsMetadata{ DocumentId = document1Id, ErrorStage = errorStage1},
                        new FileWithErrorsMetadata{ DocumentId = document1Id, ErrorStage = errorStage2},
                    },
                });

            _documentApiClientMock.Setup(x => x.GetDocumentsNames(new[] { document1Id, document1Id }))
                .ReturnsAsync(new Dictionary<Guid, string>() { { document1Id, file1Name } });

            //Act
            var actual = await _filesWithErrorsMetadataService.GetAllFilesWithErrorsMetadata(supplierName);

            //Assert
            Assert.Equal(file1Name, actual.FilesWithErrors.ElementAt(0).FileName);
            Assert.Equal(errorStage1, actual.FilesWithErrors.ElementAt(0).ErrorStage);
            Assert.Equal(file1Name, actual.FilesWithErrors.ElementAt(1).FileName);
            Assert.Equal(errorStage2, actual.FilesWithErrors.ElementAt(1).ErrorStage);
        }

        [Fact]
        public async void GetAllFilesWithErrorsMetadata_WhenExceptionThrown_ExceptionIsRethrown()
        {
            //Arrange
            var supplierName = "SupplierName";

            _measureRepositoryMock.Setup(x => x.GetAllFilesWithErrors(supplierName)).Throws<Exception>();

            //Act
            var act = async () => await _filesWithErrorsMetadataService.GetAllFilesWithErrorsMetadata(supplierName);

            //Assert
            await Assert.ThrowsAsync<Exception>(act);
        }
    }
}
