using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0807RuleLogic : IRuleLogic
    {
        private static readonly List<string> HeatingMeasureTypes = new()
        {
            MeasureTypes.Trv,
            MeasureTypes.PAndRt
        };

        private static readonly List<string> AcceptedCategoryTypes = new()
        {
            MeasureCategories.CavityWallInsulation,
            MeasureCategories.ExternalInternalWallInsulation,
            MeasureCategories.LoftInsulation,
            MeasureCategories.OtherInsulation
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            HeatingMeasureTypes.CaseInsensitiveContainsInList(measureModel.MeasureType) &&
            !measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
            measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails?.MeasureCategory != null &&
            !AcceptedCategoryTypes.CaseInsensitiveContainsInList(measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails?.MeasureCategory);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInsulationMrnForHeatingMeasures;

        public string TestNumber { get; } = "GBIS0807";

    }
}