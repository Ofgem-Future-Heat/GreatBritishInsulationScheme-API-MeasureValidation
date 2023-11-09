using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis1001RuleLogic : IRuleLogic
    {
        public Gbis1001RuleLogic(IOptions<SchemeDetailsOptions> scheduleOptions)
        {
            var finalNotificationDate = scheduleOptions.Value.FinalNotificationDate;
            FailureCondition = measureModel =>
                measureModel.PurposeOfNotification.CaseInsensitiveEquals(PurposeOfNotificationConstants
                    .AutomaticLateExtension)
                && ((measureModel.CreatedDate > measureModel.FivePercentExtensionDto!.ThreeMonthEndDate)
                    || (measureModel.CreatedDate) > finalNotificationDate);
        }
        public Predicate<MeasureModel> FailureCondition { get; init; } 
        
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PurposeOfNotification;

        public string TestNumber { get; } = "GBIS1001";
    }
}
