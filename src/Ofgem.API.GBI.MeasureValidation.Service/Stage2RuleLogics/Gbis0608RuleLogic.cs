using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0608RuleLogic : IRuleLogic
    {
        private static readonly List<string> PropertyTypesToCheck = new()
        {
            PropertyTypes.House,
            PropertyTypes.Bungalow,
            PropertyTypes.ParkHome,
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
            PropertyTypesToCheck.CaseInsensitiveContainsInList(measureModel.Property) &&
            (measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) ||
                measureModel.AssociatedInfillMeasure2.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) ||
                measureModel.AssociatedInfillMeasure3.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel =>
            $"{measureModel.AssociatedInfillMeasure1} | {measureModel.AssociatedInfillMeasure2} | {measureModel.AssociatedInfillMeasure3}";

        public string TestNumber { get; } = "GBIS0608";

    }
}