using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0303RuleLogic : IRuleLogic
    {
        private const decimal MinStartingSapRating = 0.0m;
        private const decimal MaxStartingSapRating = 68.4m;

        private static readonly List<string> AcceptedCountryCode = new()
        {
            ReferenceDataConstants.CountryCode.England,
            ReferenceDataConstants.CountryCode.Wales
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel => !string.IsNullOrWhiteSpace(measureModel.EligibilityType) &&
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LIHelpToHeatGroup) &&
            !string.IsNullOrWhiteSpace(measureModel.TenureType) &&
            measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.PrivateRentedSector) && 
            !string.IsNullOrWhiteSpace(measureModel.CountryCode) 
            && AcceptedCountryCode.CaseInsensitiveContainsInList(measureModel.CountryCode) &&
            decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
            startingSapRating is < MinStartingSapRating or > MaxStartingSapRating;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;

        public string TestNumber => "GBIS0303";
    }
}
