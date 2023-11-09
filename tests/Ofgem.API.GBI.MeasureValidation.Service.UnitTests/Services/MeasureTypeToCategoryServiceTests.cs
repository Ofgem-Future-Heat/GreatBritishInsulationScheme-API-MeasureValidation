
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Service.Services;
using System.Collections.Concurrent;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class MeasureTypeToCategoryServiceTests
    {
        private readonly Mock<ILogger<MeasureTypeToCategoryService>> _logger;
        private Mock<IMeasureRepository> _mockMeasureRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMeasureTypeToCategoryService _measureTypeToCategoryService;
        private readonly Dictionary<string, string> _testDictionary = new Dictionary<string, string>(){
                { "CWI_123_ETC", "Cavity Wall Insulation" },
                { "LI_123_ETC", "Loft Insulation" },
                { "PHI", "Other Insulation" },
                { "TRV", "Other heating" }
            };
        private readonly ConcurrentDictionary<string, string> _concTestDictionary; 
        
        public MeasureTypeToCategoryServiceTests()
        {
            _logger = new Mock<ILogger<MeasureTypeToCategoryService>>();
            _mockMeasureRepository = new Mock<IMeasureRepository>();
            _concTestDictionary = new ConcurrentDictionary<string, string>(_testDictionary);
            _mockMeasureRepository.Setup(x => x.GetMeasureCategoriesByTypeAsync()).Returns(
                Task.FromResult(_concTestDictionary));
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IMeasureRepository>(provider => _mockMeasureRepository.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _measureTypeToCategoryService = new MeasureTypeToCategoryService(_logger.Object, _serviceProvider);

        }

        [Theory]
        [InlineData("CWI_123_ETC")]
        [InlineData("LI_123_ETC")]
        public async Task GetMeasureCategoryByType_GivenValidMeasureType_ReturnsCategory(string measureType)
        {
            var expectedResult = _testDictionary[measureType];
            var actualResult = await _measureTypeToCategoryService.GetMeasureCategoryByType(measureType);
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("ABC_456_ETC")]
        public async Task GetMeasureCategoryByType_GivenInvalidType_ReturnsEmptyString(string measureType)
        {
            var actualResult = await _measureTypeToCategoryService.GetMeasureCategoryByType(measureType);
            Assert.Equal(actualResult, string.Empty);
        }
    }
}
