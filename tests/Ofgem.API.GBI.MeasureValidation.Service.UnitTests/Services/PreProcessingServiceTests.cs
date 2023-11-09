using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Mappings;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Services;
using Ofgem.Database.GBI.Measures.Domain.Entities;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class PreProcessingServiceTests
    {
        private readonly Mock<ILogger<PreProcessingService>> _logger;
        private readonly Mock<IAddressApiClient> _addressApiClient;
        private readonly Mock<IDocumentApiClient> _documentApiClient;
        private readonly Mock<ISupplierApiClient> _supplierApiClient;
        private readonly Mock<IMeasureRepository> _measureRepository;
        private readonly PreProcessingService _preProcessingService;
        private readonly string _testFilesLocation;

        public PreProcessingServiceTests()
        {
            _logger = new Mock<ILogger<PreProcessingService>>();

            _addressApiClient = new Mock<IAddressApiClient>();
            _documentApiClient = new Mock<IDocumentApiClient>();
            _supplierApiClient = new Mock<ISupplierApiClient>();
            _measureRepository = new Mock<IMeasureRepository>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mapperConfig.CreateMapper();

            _preProcessingService = new PreProcessingService(_logger.Object, _documentApiClient.Object, _supplierApiClient.Object, _measureRepository.Object, mapper);
            var separator = Path.DirectorySeparatorChar;
            _testFilesLocation = $"..{separator}..{separator}..{separator}TestFiles";
        }

        [Fact]
        public async Task GetMeasuresFromDocumentAndDatabase_Returns_Result()
        {
            //Arrange
            var fileContent = File.ReadAllBytes("Data/get-document.csv");

            var fileData = new MeasureFileContent()
            {
                DocumentId = "test",
                Content = fileContent,
                FileName = "test.csv",
            };

            _addressApiClient.Setup(c => c.ValidateAddressAsync(It.IsAny<IEnumerable<AddressValidationModel>>()))
                .ReturnsAsync(new List<AddressValidationResponse>());
            _documentApiClient.Setup(d => d.GetDocument(It.IsAny<Guid>())).ReturnsAsync(fileData);
            _supplierApiClient.Setup(s => s.GetSupplierLicencesAsync()).ReturnsAsync(new List<SupplierLicenceResponse>());

            _measureRepository.Setup(r => r.GetPurposeOfNotifications()).ReturnsAsync(new List<PurposeOfNotification>());
            _measureRepository.Setup(r => r.GetMeasureTypes()).ReturnsAsync(new List<MeasureType>());
            _measureRepository.Setup(r => r.GetEligibilityTypes()).ReturnsAsync(new List<EligibilityType>());
            _measureRepository.Setup(r => r.GetTenureTypes()).ReturnsAsync(new List<TenureType>());
            _measureRepository.Setup(r => r.GetInnovationMeasuresAsync()).ReturnsAsync(new List<InnovationMeasure>());
            _measureRepository.Setup(r => r.GetExistingMeasureData(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<ExistingMeasureDetailsForMeasureModel>());
            _measureRepository.Setup(r => r.GetFlexReferralRoutesAsync()).ReturnsAsync(new List<FlexReferralRoute>());

            //Act
            var result = await _preProcessingService.GetMeasuresFromDocumentAndDatabase(Guid.NewGuid());

            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<MeasureModel>>(result);
        }

		[Fact]
		public async Task GetMeasuresFromDocumentAndDatabase_WithNoRecords_ReturnsEmptyList()
		{
			//Arrange
			var fileData = new MeasureFileContent()
			{
				DocumentId = "test",
				Content = Array.Empty<byte>(),
				FileName = "test.csv",
			};

			_documentApiClient.Setup(d => d.GetDocument(It.IsAny<Guid>())).ReturnsAsync(fileData);
			_supplierApiClient.Setup(s => s.GetSupplierLicencesAsync()).ReturnsAsync(new List<SupplierLicenceResponse>());

			//Act
			var result = await _preProcessingService.GetMeasuresFromDocumentAndDatabase(Guid.NewGuid());

			//Assert
			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
        public async Task GetMeasuresFromDocumentAndDatabase_Throws_Exception()
        {
            var documentId = Guid.NewGuid();
            var actual = await Assert.ThrowsAsync<NullReferenceException>(() => _preProcessingService.GetMeasuresFromDocumentAndDatabase(documentId));

            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Get document error for {documentId}. {actual.Message}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                    Times.Once);
        }

        [Fact]
        public async Task AddExistingMeasureDataToMeasureModels_GivenMeasuresInDatabase_AddsStatusIdsToMeasureModel()
        {
            //Arrange

            var existingMeasureData = new List<ExistingMeasureDetailsForMeasureModel>()
            {
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456781",
                       MeasureStatusId = 2,
                       ExistingSupplierReference = "AAA"
                    },
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456782",
                       MeasureStatusId = 3,
                       ExistingSupplierReference = "BBB"
                   },
            };

            var testMeasures = new List<MeasureModel>()
            {
                new MeasureModel()
                {
                    MeasureReferenceNumber = "ABC0123456781"

                },
                new MeasureModel()
                {
                    MeasureReferenceNumber = "ABC0123456782"
                }
            };

            _measureRepository.Setup(r => r.GetExistingMeasureData(It.IsAny<List<string>>())).ReturnsAsync(existingMeasureData);

            //Act
            var result = await _preProcessingService.AddExistingMeasureDataToMeasureModels(testMeasures);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<MeasureModel>>(result);
            Assert.Equal(result.First().ExistingSupplierReference, existingMeasureData.First().ExistingSupplierReference);
            Assert.Equal(result[1].ExistingSupplierReference, existingMeasureData[1].ExistingSupplierReference);
        }

        [Fact]
        public async Task AddExistingMeasureDataToMeasureModels_GivenMeasuresInDatabaseWithNullStatusId_AddsNothingToMeasureModelStatusIdField()
        {
            //Arrange

            var testMeasures = new List<MeasureModel>()
            {
                new MeasureModel()
                {
                    MeasureReferenceNumber = "ABC0123456781"

                },
                new MeasureModel()
                {
                    MeasureReferenceNumber = "ABC0123456782"

                }
            };

            var existingMeasureData = new List<ExistingMeasureDetailsForMeasureModel>()
            {
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456781",
                       MeasureStatusId = 2,

                    },
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456782",
                       MeasureStatusId = 3,
                       ExistingSupplierReference = "BBB"
                   },
            };

            _measureRepository.Setup(r => r.GetExistingMeasureData(It.IsAny<List<string>>())).ReturnsAsync(existingMeasureData);

            //Act
            var result = await _preProcessingService.AddExistingMeasureDataToMeasureModels(testMeasures);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<MeasureModel>>(result);
            Assert.Null(result.First().ExistingSupplierReference);
            Assert.Equal(result[1].ExistingSupplierReference, existingMeasureData[1].ExistingSupplierReference);
        }

		[Fact]
		public async Task AddExistingMeasureDataToMeasureModels_GivenMeasuresInDatabase_AddsMeasureCategoryIdToMeasureModel()
		{
            //Arrange
            int measureCategoryId = 1;

            var referenceDataDetails = new ReferenceDataDetails
            {
                MeasureTypesList = new List<MeasureTypeDto>
                {
                    new MeasureTypeDto
                    {
                        Name = "CWI_0.027",
                        MeasureCategoryId = measureCategoryId
                    }
                }
            };

			var testMeasures = new List<MeasureModel>()
			{
				new MeasureModel()
				{
					MeasureReferenceNumber = "ABC0123456781",
                    MeasureType = "CWI_0.027",
                    ReferenceDataDetails = referenceDataDetails
				}
			};

			//Act
			var result = await _preProcessingService.AddExistingMeasureDataToMeasureModels(testMeasures);

			//Assert
			Assert.NotNull(result);
			Assert.Equal(measureCategoryId, result.First().MeasureCategoryId);
		}

		[Fact]
        public async Task GetMeasuresFromDocumentAndDatabase_GivenMeasuresWithValidAddresses_AddsExistingDataAndReturns()
        {
            var streamFile = ReadCsvFile("two_valid_mrns.csv");

            var mockDocumentApiClientReturnObject = new List<ExistingMeasureDetailsForMeasureModel>()
            {
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456782",
                       ExistingSupplierReference = "AAA",
                       MeasureStatusId = 3,

                    },
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456781",
                       MeasureStatusId = 2,
                       ExistingSupplierReference = "BBB"
                   },
            };

            var mockDocumentClientApiReturnObject = new MeasureFileContent()
            {
                Content = streamFile
            };

            _documentApiClient.Setup(d => d.GetDocument(It.IsAny<Guid>())).ReturnsAsync(mockDocumentClientApiReturnObject);
            _measureRepository.Setup(r => r.GetExistingMeasureData(It.IsAny<List<string>>())).ReturnsAsync(mockDocumentApiClientReturnObject);

            var actualResult = await _preProcessingService.GetMeasuresFromDocumentAndDatabase(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
            var measureModels = actualResult.ToList();
            Assert.Equal(measureModels.First().MeasureStatusId, mockDocumentApiClientReturnObject[1].MeasureStatusId);
            Assert.Equal(measureModels.First().ExistingSupplierReference, mockDocumentApiClientReturnObject[1].ExistingSupplierReference);
            Assert.Equal(measureModels.ElementAt(1).MeasureStatusId, mockDocumentApiClientReturnObject[0].MeasureStatusId);
            Assert.Equal(measureModels.ElementAt(1).ExistingSupplierReference, mockDocumentApiClientReturnObject[0].ExistingSupplierReference);
        }

        [Fact]
        public async Task GetMeasuresFromDocumentAndDatabase_GivenMeasuresWithInvalidAddresses_AddsExistingDataAndReturns()
        {
            var streamFile = ReadCsvFile("two_valid_mrns.csv");

            var mockDocumentApiClientReturnObject = new List<ExistingMeasureDetailsForMeasureModel>()
            {
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456782",
                       ExistingSupplierReference = "AAA",
                       MeasureStatusId = 3,

                    },
                   new ExistingMeasureDetailsForMeasureModel()
                   {
                       MeasureReferenceNumber = "ABC0123456781",
                       MeasureStatusId = 2,
                       ExistingSupplierReference = "BBB"
                   },
            };

			var mockDocumentClientApiReturnObject = new MeasureFileContent()
            {
                Content = streamFile
            };

            _documentApiClient.Setup(d => d.GetDocument(It.IsAny<Guid>())).ReturnsAsync(mockDocumentClientApiReturnObject);
            _measureRepository.Setup(r => r.GetExistingMeasureData(It.IsAny<List<string>>())).ReturnsAsync(mockDocumentApiClientReturnObject);

            var actualResult = await _preProcessingService.GetMeasuresFromDocumentAndDatabase(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
            var measureModels = actualResult.ToList();
            Assert.Equal(measureModels.First().MeasureStatusId, mockDocumentApiClientReturnObject[1].MeasureStatusId);
            Assert.Equal(measureModels.First().ExistingSupplierReference, mockDocumentApiClientReturnObject[1].ExistingSupplierReference);
            Assert.Equal(measureModels.ElementAt(1).MeasureStatusId, mockDocumentApiClientReturnObject[0].MeasureStatusId);
            Assert.Equal(measureModels.ElementAt(1).ExistingSupplierReference, mockDocumentApiClientReturnObject[0].ExistingSupplierReference);

        }

		[Fact]
		public async Task AddAssociatedMeasureDataToMeasureModels_GivenMeasureWithAssociatedMeasure_AddsDataToMeasureModel()
		{
			//Arrange
			var testMeasures = new List<MeasureModel>()
			{
				new MeasureModel()
				{
					MeasureReferenceNumber = "ABC0123456782",
                    AssociatedInfillMeasure1 = "ABC0123456781"
				},
				new MeasureModel()
				{
					MeasureReferenceNumber = "ABC0123456783",
					AssociatedInfillMeasure2 = "ABC0123456781"
				},
				new MeasureModel()
				{
					MeasureReferenceNumber = "ABC0123456784",
					AssociatedInfillMeasure3 = "ABC0123456781"
				},
                new MeasureModel()
                {
                    MeasureReferenceNumber = "ABC0123456785",
                    AssociatedInsulationMrnForHeatingMeasures = "ABC0123456781"
				}
			};

            var associatedMeasureModels = new List<AssociatedMeasureModelDto>
            {
                new AssociatedMeasureModelDto
                {
                    MeasureReferenceNumber = "ABC0123456781",
                    MeasureCategoryId = 1
				}
            };

            _measureRepository.Setup(x => x.GetAssociatedMeasureData(It.IsAny<IEnumerable<string>>())).ReturnsAsync(associatedMeasureModels);

            // Act
            var result = await _preProcessingService.AddAssociatedMeasureDataToMeasureModels(testMeasures);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result[0].AssociatedInfillMeasure1Details);
            Assert.NotNull(result[1].AssociatedInfillMeasure2Details);
            Assert.NotNull(result[2].AssociatedInfillMeasure3Details);
            Assert.NotNull(result[3].AssociatedInsulationMeasureForHeatingMeasureDetails);
		}


        private byte[] ReadCsvFile(string testFileName)
        {
            string file = Path.Combine(_testFilesLocation, testFileName);
            return (File.ReadAllBytes(file).ToArray());
        }

    }
}
