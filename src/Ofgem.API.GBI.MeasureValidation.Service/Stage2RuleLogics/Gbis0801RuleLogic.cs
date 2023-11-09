using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0801RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
            !GbisRegexes.MeasureReferenceNumberRegex().IsMatch(measureModel.AssociatedInsulationMrnForHeatingMeasures!);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInsulationMrnForHeatingMeasures;
        public string TestNumber => "GBIS0801";
    }
}