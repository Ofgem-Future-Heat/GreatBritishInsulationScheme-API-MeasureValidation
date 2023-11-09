using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Service.Services;
using Ofgem.Database.GBI.Measures.Domain.Entities;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class ErrorsReportServiceTests
    {
        private readonly Mock<IErrorReportRepository> _errorReportRepository;
        private readonly ErrorsReportService _errorsReportService;

        public ErrorsReportServiceTests()
        {
            Mock<ILogger<ErrorsReportService>> logger = new();
            _errorReportRepository = new Mock<IErrorReportRepository>();
            Mock<IMapper> mapper = new();
            _errorsReportService = new ErrorsReportService(logger.Object, _errorReportRepository.Object, mapper.Object);
        }

        [Fact]
        public async Task GetErrorsReport_Returns_Result()
        {
            //Arrange
            var docId = Guid.NewGuid();
            _errorReportRepository.Setup(x => x.GetValidationErrors(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<List<ValidationError>>());

            //Act
            var result = await _errorsReportService.GetErrorsReport(docId, "Stage");

            //Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }
    }
}
