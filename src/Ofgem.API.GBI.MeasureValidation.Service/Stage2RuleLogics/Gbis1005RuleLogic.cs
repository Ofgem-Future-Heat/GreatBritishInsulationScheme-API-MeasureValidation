using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis1005RuleLogic : IRuleLogic
    {

        public Gbis1005RuleLogic(IOptions<SchemeDetailsOptions> scheduleOptions)
        {
            var finalNotificationDate = scheduleOptions.Value.FinalNotificationDate;
            FailureCondition = measureModel => measureModel.CreatedDate > finalNotificationDate;
        }

        public Predicate<MeasureModel> FailureCondition { get; init; }

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.CreatedDate!.Value.ToShortDateString();

        public string TestNumber => "GBIS1005";

    }
}