using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0502RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !(string.IsNullOrWhiteSpace(measureModel.StreetName) ||
              measureModel.StreetName.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable)) &&
            measureModel.StreetName.Length > 50;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StreetName;
        public string TestNumber => "GBIS0502";
    }
}
