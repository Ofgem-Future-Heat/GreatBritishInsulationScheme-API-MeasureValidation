using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0802RuleLogic : IRuleLogic
    {
        private static readonly List<string> IncludedMeasureTypes = new()
        {
            MeasureTypes.Trv,
            MeasureTypes.PAndRt
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            IncludedMeasureTypes.CaseInsensitiveContainsInList(measureModel.MeasureType) &&
            measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInsulationMrnForHeatingMeasures;

        public string TestNumber => "GBIS0802";
    }
}
