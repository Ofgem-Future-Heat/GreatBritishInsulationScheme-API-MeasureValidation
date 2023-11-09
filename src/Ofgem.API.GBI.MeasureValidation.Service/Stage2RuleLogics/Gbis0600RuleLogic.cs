using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0600RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } =
            measureModel => !measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
                            (!measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) ||
                              !measureModel.AssociatedInfillMeasure2.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) ||
                              !measureModel.AssociatedInfillMeasure3.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel =>
        {
            if (measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill)) return string.Empty;
            return $"{measureModel.AssociatedInfillMeasure1} | {measureModel.AssociatedInfillMeasure2} | {measureModel.AssociatedInfillMeasure3}";
        };

        public string TestNumber => "GBIS0600";
    }
}
