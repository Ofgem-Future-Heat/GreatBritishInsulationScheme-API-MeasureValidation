using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0722RuleLogic : IRuleLogic
    {
        private static readonly List<string> ValidResponses = new()
        {
            CommonTypesConstants.Yes,
            CommonTypesConstants.No,
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !ValidResponses.CaseInsensitiveContainsInList(measureModel.Rural);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.Rural;

        public string TestNumber { get; } = "GBIS0722";
    }
}
