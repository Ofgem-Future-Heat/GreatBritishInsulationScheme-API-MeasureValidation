using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0611RuleLogic : IRuleLogic
    {
        private static readonly List<string> PropertyTypesToCheck = new()
        {
            PropertyTypes.House,
            PropertyTypes.Bungalow,
            PropertyTypes.ParkHome,
        };

        private const decimal MinStartingSapRating = 0.0m;
        private const decimal MaxStartingSapRating = 68.4m;

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
            PropertyTypesToCheck.CaseInsensitiveContainsInList(measureModel.Property) &&
            !measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
            decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
            startingSapRating is < MinStartingSapRating or > MaxStartingSapRating;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;

        public string TestNumber { get; } = "GBIS0611";

    }
}