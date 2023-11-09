using Microsoft.Extensions.Options;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common
{
    public class NotificationDateServiceTests
    {
        private readonly Mock<TimeProvider> timeProviderMock = new();
        private Mock<IOptions<SchemeDetailsOptions>> schemeDetailsOptionsMock = new();
        private readonly NotificationDateService notificationDateService;

        public NotificationDateServiceTests()
        {
            schemeDetailsOptionsMock.Setup(x => x.Value).Returns(
                new SchemeDetailsOptions
                {
                    BackdatedInstallationsPeriodEndDateTime = new DateOnly(2023, 12, 31).ToDateTime(TimeOnly.MaxValue),
                    BackdatedInstallationsNotificationEndDateTime = new DateOnly(2024, 01, 31).ToDateTime(TimeOnly.MaxValue),
                }
            );
            notificationDateService = new NotificationDateService(schemeDetailsOptionsMock.Object, timeProviderMock.Object);
        }

        [Theory]
        [MemberData(nameof(CalculateNotificationEndDateTestData))]
        public void CalculateNotificationEndDate_ReturnsNotificationEndDate(DateOnly date, DateTime expected)
        {
            // Arrange

            // Act
            var actual = notificationDateService.CalculateNotificationEndDateTime(date);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(DatesOnly))]
        public void GetLastDateOfFollowingMonth_ReturnsLastDayOfFollowingMonth(DateOnly date, DateOnly expected)
        {
            // Arrange

            // Act
            var actual = NotificationDateService.GetLastDateOfFollowingMonth(date);

            // Assert
            Assert.Equal(expected, actual);
        }

        public static TheoryData<DateOnly, DateTime> CalculateNotificationEndDateTestData()
        {
            var endTime = TimeOnly.MaxValue;

            return new TheoryData<DateOnly, DateTime>
            {
                { new DateOnly(2024, 10, 01), new DateOnly(2024, 11, 30).ToDateTime(endTime) },
                { new DateOnly(2024, 10, 11), new DateOnly(2024, 11, 30).ToDateTime(endTime) },
                { new DateOnly(2024, 10, 31), new DateOnly(2024, 11, 30).ToDateTime(endTime) },
                { new DateOnly(2024, 11, 01), new DateOnly(2024, 12, 31).ToDateTime(endTime) },
                { new DateOnly(2024, 11, 30), new DateOnly(2024, 12, 31).ToDateTime(endTime) },
                { new DateOnly(2024, 11, 11), new DateOnly(2024, 12, 31).ToDateTime(endTime) },
                { new DateOnly(2025, 01, 11), new DateOnly(2025, 02, 28).ToDateTime(endTime) },
                { new DateOnly(2024, 01, 11), new DateOnly(2024, 02, 29).ToDateTime(endTime) },
                { new DateOnly(2024, 12, 11), new DateOnly(2025, 01, 31).ToDateTime(endTime) },
                { new DateOnly(2023, 01, 01), new DateOnly(2024, 01, 31).ToDateTime(endTime) },
                { new DateOnly(2023, 12, 11), new DateOnly(2024, 01, 31).ToDateTime(endTime) },
                { new DateOnly(2024, 01, 01), new DateOnly(2024, 02, 29).ToDateTime(endTime) },
            };
        }

        public static TheoryData<DateOnly, DateOnly> DatesOnly() => new()
        {
            { new DateOnly(2023, 10, 01), new DateOnly(2023, 11, 30) },
            { new DateOnly(2023, 10, 11), new DateOnly(2023, 11, 30) },
            { new DateOnly(2023, 10, 31), new DateOnly(2023, 11, 30) },
            { new DateOnly(2023, 11, 01), new DateOnly(2023, 12, 31) },
            { new DateOnly(2023, 11, 30), new DateOnly(2023, 12, 31) },
            { new DateOnly(2023, 11, 11), new DateOnly(2023, 12, 31) },
            { new DateOnly(2021, 01, 11), new DateOnly(2021, 02, 28) },
            { new DateOnly(2020, 01, 11), new DateOnly(2020, 02, 29) },
            { new DateOnly(2020, 12, 11), new DateOnly(2021, 01, 31) },
        };

        [Fact]
        public void HasNotificationEndDatePassed_ReturnsFalseWhenNotificationEndDateHasNotPassed()
        {
            // Arrange
            timeProviderMock.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 10, 10, 0, 0, 0, TimeSpan.Zero));
            MeasureModel measure = new MeasureModel
            {
                DateOfCompletedInstallation = "01/10/2023"
            };

            // Act
            var actual = notificationDateService.HasNotificationEndDatePassed(measure);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void HasNotificationEndDatePassed_ReturnsTrueWhenNotificationEndDateHasPassed()
        {
            // Arrange
            timeProviderMock.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2024, 02, 01, 0, 0, 0, TimeSpan.Zero));
            MeasureModel measure = new MeasureModel
            {
                DateOfCompletedInstallation = "10/10/2023"
            };

            // Act
            var actual = notificationDateService.HasNotificationEndDatePassed(measure);

            // Assert
            Assert.True(actual);
        }
    }
}
