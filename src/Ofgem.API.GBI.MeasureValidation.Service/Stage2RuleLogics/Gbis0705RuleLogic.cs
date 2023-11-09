using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0705RuleLogic : IRuleLogic
    {
        public Gbis0705RuleLogic(IOptions<SchemeDetailsOptions> schemeDetailsOptions) 
        {
            var startDate = schemeDetailsOptions.Value.StartDate;
            var backdatedInstallationsEndDate = schemeDetailsOptions.Value.BackdatedInstallationsPeriodEndDateTime;
            var backdatedInstallationsNotificationEndDate = schemeDetailsOptions.Value.BackdatedInstallationsNotificationEndDateTime;

            FailureCondition = measureModel => CheckPurposeOfNotifications(measureModel.PurposeOfNotification) &&
            DateTime.TryParseExact(measureModel.DateOfCompletedInstallation, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var doci) &&
            doci >= startDate && doci <= backdatedInstallationsEndDate && measureModel.CreatedDate > backdatedInstallationsNotificationEndDate;
        }

        private static readonly List<string> PurposeOfNotificationsToCheck = new()
        {
            PurposeOfNotificationConstants.EditedNotification,
            PurposeOfNotificationConstants.ExtendedNotification,
            PurposeOfNotificationConstants.AutomaticLateExtension
        };

        private static bool CheckPurposeOfNotifications(string? pon)
        {
            return !PurposeOfNotificationsToCheck.CaseInsensitiveContainsInList(pon);
        }

        public Predicate<MeasureModel> FailureCondition { get; init; } 
  
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.CreatedDate?.Date.ToString("dd/MM/yyyy");
        public string TestNumber { get; } = "GBIS0705";
    }
}