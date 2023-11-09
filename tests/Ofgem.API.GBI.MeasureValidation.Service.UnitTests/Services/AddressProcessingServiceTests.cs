using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class AddressProcessingServiceTests
    {
        private readonly Mock<IAddressApiClient> _addressApiClient;
        private readonly AddressProcessingService _addressProcessingService;

        public AddressProcessingServiceTests()
        {
            Mock<ILogger<AddressProcessingService>> logger = new();

            _addressApiClient = new Mock<IAddressApiClient>();

            _addressProcessingService = new AddressProcessingService(_addressApiClient.Object, logger.Object);
        }

        [Fact]
        public async Task ValidateAddressAsync_HasVerifiedAddresses_ReturnsExpectedResult()
        {
            //Arrange
            _addressApiClient.Setup(c => c.ValidateAddressAsync(It.IsAny<IEnumerable<AddressValidationModel>>()))
                .ReturnsAsync(CreateMockAddressValidationResponseList(true));

            //Act
            var result = await _addressProcessingService.AddressVerificationAsync(MockMeasureModelsList);
            var measureModels = result.ToList();
            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<MeasureModel>>(result);
            var mrn1 = measureModels.First(c => c.MeasureReferenceNumber == "ABC0123456782");
            Assert.True(mrn1.AddressIsVerified);
            Assert.Equal("12345678", mrn1.VerifiedUprn);
            Assert.Equal("GB-SCT", mrn1.CountryCode);
            var mrn2 = measureModels.First(c => c.MeasureReferenceNumber == "ABC0123456781");
            Assert.True(mrn2.AddressIsVerified);
            Assert.Equal("87654321", mrn2.VerifiedUprn);
            Assert.Equal("GB-ENG", mrn2.CountryCode);
        }


        [Fact]
        public async Task ValidateAddressAsync_HasNonVerifiedAddresses_ReturnsExpectedResult()
        {
            //Arrange
            _addressApiClient.Setup(c => c.ValidateAddressAsync(It.IsAny<IEnumerable<AddressValidationModel>>()))
                .ReturnsAsync(CreateMockAddressValidationResponseList());

            //Act
            var result = await _addressProcessingService.AddressVerificationAsync(MockMeasureModelsList);
            var measureModels = result.ToList();
            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<MeasureModel>>(result);
            var mrn1 = measureModels.First(c => c.MeasureReferenceNumber == "ABC0123456782");
            Assert.False(mrn1.AddressIsVerified);
            Assert.NotEqual("12345678", mrn1.VerifiedUprn);
            Assert.Equal("GB-SCT", mrn1.CountryCode);
            var mrn2 = measureModels.First(c => c.MeasureReferenceNumber == "ABC0123456781");
            Assert.False(mrn2.AddressIsVerified);
            Assert.NotEqual("87654321", mrn2.VerifiedUprn);
            Assert.Equal("GB-ENG", mrn2.CountryCode);
        }

        private static List<MeasureModel> MockMeasureModelsList => new()
        {
            new MeasureModel(){MeasureReferenceNumber = "ABC0123456782"},
            new MeasureModel(){MeasureReferenceNumber = "ABC0123456781"}
        };

        private static List<AddressValidationResponse> CreateMockAddressValidationResponseList(bool isValid = false)
        {
            return new List<AddressValidationResponse>() { new()
            {
                Address = new AddressValidationModel(){AddressReferenceNumber = "ABC0123456782" },
                Uprn = isValid ? "12345678":null,
                IsValid = isValid,
                CountryCode = "GB-SCT"
            }, new()
            {
                Address = new AddressValidationModel(){AddressReferenceNumber = "ABC0123456781" },
                Uprn = isValid ? "87654321":null,
                IsValid = isValid,
                CountryCode = "GB-ENG"
            } };
        }

    }
}
