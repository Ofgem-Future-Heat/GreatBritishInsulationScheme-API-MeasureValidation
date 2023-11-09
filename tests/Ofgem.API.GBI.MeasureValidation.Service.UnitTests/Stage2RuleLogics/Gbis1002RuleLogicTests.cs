using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.Database.GBI.Measures.Domain.Dtos;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis1002RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(Gbis1002ValidInputArguments))]
        public void Gbis1002Rule_WithValidInput_PassValidation(string purposeOfNotification, DateTime notificationDate, DateTime notificationEndDate)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                PurposeOfNotification = purposeOfNotification,
                CreatedDate = notificationDate,
                FivePercentExtensionDto = new FivePercentExtensionQuotaDto()
                {
                    NotificationEndDate = notificationEndDate
                }
            };
            var rule = new Gbis1002RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        public static TheoryData<string, DateTime, DateTime> Gbis1002ValidInputArguments() => new()
        {
            { PurposeOfNotificationConstants.ExtendedNotification, new DateTime(2023, 02, 15), new DateTime(2023, 02, 16) },
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2023, 12, 15), new DateTime(2024, 01, 01) },
            { "Extended Notification", new DateTime(2023, 01, 01), new DateTime(2023, 01, 02) },
            { "automatic late extension", new DateTime(2023, 01, 01), new DateTime(2025, 01, 01) }
        };

        [Theory]
        [MemberData(nameof(Gbis1002InvalidInputArguments))]
        public void Gbis1002Rule_WithInvalidInput_FailsValidation(string purposeOfNotification, DateTime notificationDate, DateTime notificationEndDate)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                PurposeOfNotification = purposeOfNotification,
                CreatedDate = notificationDate,
                FivePercentExtensionDto = new FivePercentExtensionQuotaDto()
                {
                    NotificationEndDate = notificationEndDate
                }
            };
            var rule = new Gbis1002RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS1002", rule.TestNumber);
            Assert.Equal(measureModel.PurposeOfNotification, observedValue);
        }

        public static TheoryData<string, DateTime, DateTime> Gbis1002InvalidInputArguments() => new()
        {
            { PurposeOfNotificationConstants.ExtendedNotification, new DateTime(2023, 02, 15), new DateTime(2022, 01, 01) },
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2023, 02, 15), new DateTime(2023, 01, 01) },
            { PurposeOfNotificationConstants.EditedNotification, new DateTime(2024, 01, 01), new DateTime(2023, 01, 01) },
            { "edited notification", new DateTime(2023, 10, 21), new DateTime(2023, 01, 21) },
            { "New Notification", new DateTime(2024, 02, 01), new DateTime(2024, 01, 01) },
            { "new notification", new DateTime(2024, 04, 15), new DateTime(2024, 04, 14) },
        };
    }
}
