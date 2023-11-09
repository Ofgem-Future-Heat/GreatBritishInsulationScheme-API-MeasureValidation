namespace Ofgem.API.GBI.MeasureValidation.Application.Common
{
    public class SchemeDetailsOptions
    {
        public const string Name = "SchemeDetails";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime FinalNotificationDate { get; set; }
        public DateTime BackdatedInstallationsPeriodEndDateTime { get; set; }
        public DateTime BackdatedInstallationsNotificationEndDateTime { get; set; }
    }
}
