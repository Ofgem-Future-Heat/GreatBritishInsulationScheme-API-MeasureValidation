using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Services
{
    public class Stage2PassedMeasuresProcessorTests
    {
        private readonly Mock<INotificationDateService> notificationDateService;
        private readonly Stage2PassedMeasuresProcessor stage2PassedMeasuresProcessor;

        public Stage2PassedMeasuresProcessorTests()
        {
            notificationDateService = new Mock<INotificationDateService>();
            stage2PassedMeasuresProcessor = new Stage2PassedMeasuresProcessor(notificationDateService.Object);
        }

        [Fact]
        public async Task Process_GivenNullMeasuresArgument_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(() => stage2PassedMeasuresProcessor.Process(null!));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Contains("measures", exception.Message);
        }

        [Theory]
        [MemberData(nameof(FutureNotificationEndDateUpdatesMeasureStatusToNotifiedPending))]
        [MemberData(nameof(PassedNotificationEndDateUpdatesMeasureStatusToMeasureAwaitingVerification))]
        public async Task Process_MeasuresWithNotificationEndDate_UpdatesMeasureStatus(bool notificationEndDatePassed, int measureStatusId)
        {
            // Arrange
            notificationDateService.Setup(x => x.HasNotificationEndDatePassed(It.IsAny<MeasureModel>())).Returns(notificationEndDatePassed);

            var measureModels = new[]
            {
                new MeasureModel(),
            };

            // Act
            var actual = await stage2PassedMeasuresProcessor.Process(measureModels);

            // Assert
            Assert.Equal(measureStatusId, actual.First().MeasureStatusId);
        }

        public static TheoryData<bool, int> FutureNotificationEndDateUpdatesMeasureStatusToNotifiedPending() => new()
        {
            { false, MeasureStatusConstants.NotifiedPending },
        };

        public static TheoryData<bool, int> PassedNotificationEndDateUpdatesMeasureStatusToMeasureAwaitingVerification() => new()
        {
            { true, MeasureStatusConstants.MeasureAwaitingVerification },
        };

        [Fact]
        public async Task Process_MeasuresWithVariousNotificationEndDate_UpdatesMeasureStatuses()
        {
            // Arrange
            var measureModel1 = new MeasureModel();
            var measureModel2 = new MeasureModel();
            notificationDateService.Setup(x => x.HasNotificationEndDatePassed(measureModel1)).Returns(true);
            notificationDateService.Setup(x => x.HasNotificationEndDatePassed(measureModel2)).Returns(false);
            
            var measureModels = new[]
            {
                measureModel1,
                measureModel2,
            };

            // Act
            var actual = await stage2PassedMeasuresProcessor.Process(measureModels);

            // Assert
            var actualList = actual.ToList();
            Assert.Equal(MeasureStatusConstants.MeasureAwaitingVerification, actualList.First().MeasureStatusId);
            Assert.Equal(MeasureStatusConstants.NotifiedPending, actualList.Last().MeasureStatusId);
        }
    }
}
