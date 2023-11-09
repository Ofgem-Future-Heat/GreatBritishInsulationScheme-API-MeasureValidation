using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Service.Services;
using System.Collections.Concurrent;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class MeasureTypeToInnovationMeasureServiceTests
    {
        private readonly Mock<ILogger<MeasureTypeToInnovationMeasureService>> _logger;
        private Mock<IMeasureRepository> _mockMeasureRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMeasureTypeToInnovationMeasureService _measureTypeToInnovationMeasureService;
        private readonly Dictionary<string, List<string>> _testDictionary = new Dictionary<string, List<string>>(){
                { "CWI_123_ETC", new List<string>(){"001", "002" } },
                { "LI_123_ETC", new List<string>(){"001", "002" } },
                { "PHI", new List < string >() },
                { "TRV", new List < string >() }
        };
        private readonly ConcurrentDictionary<string, List<string>> _concTestDictionary;

        public MeasureTypeToInnovationMeasureServiceTests()
        {
            _logger = new Mock<ILogger<MeasureTypeToInnovationMeasureService>>();
            _mockMeasureRepository = new Mock<IMeasureRepository>();
            _concTestDictionary = new ConcurrentDictionary<string, List<string>>(_testDictionary);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IMeasureRepository>(provider => _mockMeasureRepository.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _measureTypeToInnovationMeasureService = new MeasureTypeToInnovationMeasureService(_logger.Object, _serviceProvider);

        }

        [Theory]
        [InlineData("CWI_123_ETC")]
        [InlineData("LI_123_ETC")]
        public async Task GetInnovationNumbersByType_GivenValidMeasureType_ReturnsListOfNumbers(string measureType)
        {
            _mockMeasureRepository.Setup(x => x.GetMeasureTypesInnovationNumbers(It.IsAny<string>())).Returns(
                Task.FromResult(_concTestDictionary[measureType]));
            var expectedResult = _concTestDictionary[measureType];
            var actualResult = await _measureTypeToInnovationMeasureService.GetMeasureTypeInnovationNumbers(measureType);
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("ABC_456_ETC")]
        public async Task GetInnovationNumbersByType_GivenValidMeasureType_ReturnsEmptyList(string measureType)
        {
            var actualResult = await _measureTypeToInnovationMeasureService.GetMeasureTypeInnovationNumbers(measureType);
            Assert.Equal(actualResult, new List<string>());
        }

    }
}
