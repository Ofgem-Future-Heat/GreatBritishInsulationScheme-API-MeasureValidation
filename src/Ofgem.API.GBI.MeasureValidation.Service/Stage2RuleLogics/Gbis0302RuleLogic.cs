using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0302RuleLogic : IRuleLogic
    {
        private const decimal MinStartingSapRating = 0.0m;
        private const decimal MaxStartingSapRating = 68.4m;

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel => !string.IsNullOrWhiteSpace(measureModel.EligibilityType) &&
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LIHelpToHeatGroup) &&
            !string.IsNullOrWhiteSpace(measureModel.TenureType) &&
            measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.OwnerOccupied) &&
            decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
            startingSapRating is < MinStartingSapRating or > MaxStartingSapRating;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;

        public string TestNumber => "GBIS0302";
    }
}
