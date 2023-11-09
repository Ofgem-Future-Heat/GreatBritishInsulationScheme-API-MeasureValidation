using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using System.Globalization;
using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Common
{
    public class NotificationDateService : INotificationDateService
    {
        private readonly DateTime backdatedInstallationsEndDate;
        private readonly DateTime backdatedInstallationsNotificationEndDate;
        private readonly TimeProvider timeProvider;

        public NotificationDateService(IOptions<SchemeDetailsOptions> schemeDetailsOptions, TimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
            backdatedInstallationsEndDate = schemeDetailsOptions.Value.BackdatedInstallationsPeriodEndDateTime;
            backdatedInstallationsNotificationEndDate = schemeDetailsOptions.Value.BackdatedInstallationsNotificationEndDateTime;
        }

        public bool HasNotificationEndDatePassed(MeasureModel measure)
        {
            var dateOfCompletedInstallationDate =
                DateOnly.ParseExact(measure.DateOfCompletedInstallation!, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var notificationEndDateTime = CalculateNotificationEndDateTime(dateOfCompletedInstallationDate);

            return timeProvider.GetUtcNow() > notificationEndDateTime;
        }

        internal DateTime CalculateNotificationEndDateTime(DateOnly date)
        {
            var notificationEndDateTime = IsBackdatedInstallation(date)
                ? backdatedInstallationsNotificationEndDate
                : GetLastDateOfFollowingMonth(date).ToDateTime(TimeOnly.MaxValue);

            return notificationEndDateTime;

            bool IsBackdatedInstallation(DateOnly d) => d.ToDateTime(TimeOnly.MinValue) <= backdatedInstallationsEndDate;
        }

        internal static DateOnly GetLastDateOfFollowingMonth(DateOnly date)
        {
            var dayOfMonth = date.Day;
            var twoMonthsAdded = date.AddMonths(2);
            var lastDayOfNextMonth = twoMonthsAdded.AddDays(-dayOfMonth);

            return lastDayOfNextMonth;
        }
    }
}
