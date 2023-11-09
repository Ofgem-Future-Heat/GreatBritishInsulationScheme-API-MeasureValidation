using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis1002RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            (!PurposeOfNotificationConstants.AutomaticLateExtension.CaseInsensitiveEquals(measureModel
                .PurposeOfNotification) ||
            !PurposeOfNotificationConstants.ExtendedNotification.CaseInsensitiveEquals(measureModel
                .PurposeOfNotification)) &&
            measureModel.CreatedDate > measureModel.FivePercentExtensionDto?.NotificationEndDate;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PurposeOfNotification;

        public string TestNumber => "GBIS1002";
    }
}