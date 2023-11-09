using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0910RuleLogic : IRuleLogic
    {
        private const decimal MinStartingSapRating = 0.0m;
        private const decimal MaxStartingSapRating = 54.4m;

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LISupplierEvidence) &&
            measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.OwnerOccupied) &&
            decimal.TryParse(measureModel.StartingSAPRating, out var startingSapRating) &&
            startingSapRating is < MinStartingSapRating or > MaxStartingSapRating;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;

        public string TestNumber => "GBIS0910";

    }
}