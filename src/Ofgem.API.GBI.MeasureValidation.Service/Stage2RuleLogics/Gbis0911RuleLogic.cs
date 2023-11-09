using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0911RuleLogic : IRuleLogic
    {
        private const decimal MinStartingSapRating = 0.0m;
        private const decimal MaxStartingSapRating = 54.4m;

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LISupplierEvidence) &&
            measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.PrivateRentedSector) &&
            (measureModel.CountryCode == ReferenceDataConstants.CountryCode.England ||
            measureModel.CountryCode == ReferenceDataConstants.CountryCode.Wales) &&
            decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
            startingSapRating is < MinStartingSapRating or > MaxStartingSapRating;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;
        public string TestNumber => "GBIS0911";
    }
}
