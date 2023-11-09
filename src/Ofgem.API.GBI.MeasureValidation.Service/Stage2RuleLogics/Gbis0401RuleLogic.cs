using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0401RuleLogic : IRuleLogic
    {
        private static readonly List<string> AcceptedEligibilityTypes = new() { EligibilityTypes.LISocialHousing, EligibilityTypes.LISupplierEvidence, EligibilityTypes.LIHelpToHeatGroup, EligibilityTypes.LILADeclaration };
        private const decimal MinStartingSapRating = 0.0m;
        private const decimal MaxStartingSapRating = 68.4m;
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
                AcceptedEligibilityTypes.CaseInsensitiveContainsInList(measureModel.EligibilityType)
        && decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
            startingSapRating is < MinStartingSapRating or > MaxStartingSapRating;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;
        public string TestNumber => "GBIS0401";
    }
}
