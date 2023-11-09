using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0501RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !(string.IsNullOrWhiteSpace(measureModel.FlatNameNumber) ||
              measureModel.FlatNameNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable)) &&
            measureModel.FlatNameNumber.Length > 50;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.FlatNameNumber;
        public string TestNumber { get; } = "GBIS0501";
    }
}
