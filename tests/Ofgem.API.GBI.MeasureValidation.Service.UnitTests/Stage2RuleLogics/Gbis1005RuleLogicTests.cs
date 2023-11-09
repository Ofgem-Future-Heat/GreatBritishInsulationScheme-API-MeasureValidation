using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis1005RuleLogicTests
    {
        private readonly IOptions<SchemeDetailsOptions> _mockSchemeDetailsOptions = Options.Create(new SchemeDetailsOptions()
        {
            FinalNotificationDate = new DateTime(2026, 6, 30, 23, 59, 59),
        });

        [Theory]
        [MemberData(nameof(ValidNotificationDates))]
        public void Gbis1005Rule_WithValidInput_PassesValidation(DateTime notificationDate)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                CreatedDate = notificationDate
            };
            var rule = new Gbis1005RuleLogic(_mockSchemeDetailsOptions);
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<DateTime?> ValidNotificationDates() => new()
        {
            { null },
            { new DateTime(2026, 6, 30) },
            { new DateTime(2026, 6, 30, 23, 59, 59) },
            { new DateTime(2024, 11, 7) },
            { new DateTime(1985, 10, 26, 9, 0, 0) },
        };

        [Theory]
        [MemberData(nameof(InvalidNotificationDates))]
        public void Gbis1005Rule_WithInvalidInput_FailsValidation(DateTime notificationDate)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                CreatedDate = notificationDate
            };
            var rule = new Gbis1005RuleLogic(_mockSchemeDetailsOptions);
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.CreatedDate.Value.ToShortDateString(), errorFieldValue);
            Assert.Equal("GBIS1005", rule.TestNumber);
        }

        public static TheoryData<DateTime> InvalidNotificationDates() => new()
        {
            { new DateTime( 2026, 7, 1) },
            { new DateTime( 2026, 7, 1, 0, 0, 1) },
            { new DateTime( 2027, 6, 30, 9, 10, 21) },
            { new DateTime( 2050, 1, 1) },
        };

    }
}