using DotNetRuleEngine.Interface;
using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0705RuleLogicTests
    {
        readonly IOptions<SchemeDetailsOptions> _mockSchemeDetailsOptions = Options.Create(new SchemeDetailsOptions()
        {
            StartDate = new DateTime(2023, 03, 30),
            BackdatedInstallationsPeriodEndDateTime = new DateTime(2023, 12, 31),
            BackdatedInstallationsNotificationEndDateTime = new DateTime(2024, 01, 31),
        });

        [Theory]
        [MemberData(nameof(Gbis0705ValidInputArguments))]
        public void Gbis0705Rule_WithValidInput_PassValidation(string purposeOfNotification, DateTime notificationDate)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                PurposeOfNotification = purposeOfNotification,
                DateOfCompletedInstallation = "30/10/2023",
                CreatedDate = notificationDate,
            };
            var rule = new Gbis0705RuleLogic(_mockSchemeDetailsOptions);
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        public static TheoryData<string, DateTime> Gbis0705ValidInputArguments() => new()
        {
            { "New Notification", new DateTime(2023, 01, 01) },
            { PurposeOfNotificationConstants.EditedNotification, new DateTime(2023, 01, 01) },
            { "edited notification", new DateTime(2023, 01, 01) },
            { PurposeOfNotificationConstants.ExtendedNotification, new DateTime(2023, 02, 15) },
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2023, 02, 15) }
        };

        [Theory]
        [MemberData(nameof(Gbis0705InvalidInputArguments))]
        public void Gbis0705Rule_WithInvalidInput_FailsValidation(string purposeOfNotification, DateTime notificationDate)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                PurposeOfNotification = purposeOfNotification,
                DateOfCompletedInstallation = "18/10/2023",
                CreatedDate = notificationDate,
            };
            var rule = new Gbis0705RuleLogic(_mockSchemeDetailsOptions);

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0705", rule.TestNumber);
            Assert.Equal(notificationDate.ToString("dd/MM/yyyy"), observedValue);
        }

        public static TheoryData<string, DateTime> Gbis0705InvalidInputArguments() => new()
        {
            { "New Notification", new DateTime(2024, 02, 01) },
            { "new notification", new DateTime(2024, 04, 15) },
        };
    }
}
