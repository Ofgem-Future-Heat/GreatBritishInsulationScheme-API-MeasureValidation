using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class InfillMeasureServiceTests
    {
        private readonly Mock<IMeasureRepository> _measureRepositoryMock;
        private readonly IInFillMeasureService _infillMeasureService;
        private readonly ConcurrentDictionary<string, AssociatedInFillMeasuresDto> _associatedInFillMeasures = new();

        public InfillMeasureServiceTests()
        {
            _measureRepositoryMock = new Mock<IMeasureRepository>();
            Mock<ILogger<InFillMeasureService>> loggerMock = new();
            _infillMeasureService = new InFillMeasureService(_measureRepositoryMock.Object, loggerMock.Object);
        }

        [Theory]
        [InlineData("ABC123", "ABC234", "ABC234", null, null)]
        [InlineData("ABC123", "ABC234", null, "ABC234", null)]
        [InlineData("ABC123", "ABC234", null, null, "ABC234")]
        [InlineData("ABC123", "ABC234", "ABC234", "ABC345", null)]
        [InlineData("ABC123", "ABC234", null, "ABC234", "ABC345")]
        [InlineData("ABC123", "ABC234", "ABC345", null, "ABC234")]
        [InlineData("ABC123", "ABC234", "ABC345", "ABC567", "ABC234")]
        public async Task IsInfillMeasureAssigned_WhenAssociatedInFillMeasureValuesExistInDb_ReturnsTrue(string mrn,
            string infillMeasureToMatch, string? dbInfillMeasure1, string? dbInfillMeasure2, string? dbInfillMeasure3)
        {
            _associatedInFillMeasures.TryAdd("ABC890",
                new AssociatedInFillMeasuresDto(dbInfillMeasure1, dbInfillMeasure2, dbInfillMeasure3));

            _measureRepositoryMock.Setup(c => c.GetMeasureInfillsAsync(mrn, infillMeasureToMatch)).ReturnsAsync(_associatedInFillMeasures);

            var result = await _infillMeasureService.IsInfillMeasureAssigned(mrn, infillMeasureToMatch);

            Assert.True(result);
        }
    }
}
