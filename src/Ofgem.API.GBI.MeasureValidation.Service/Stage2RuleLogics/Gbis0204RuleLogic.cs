using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0204RuleLogic : IRuleLogic
    {
        private const decimal minStartingSapRating = 0.0m;
        private const decimal maxStartingSapRating = 68.4m;

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.GeneralGroup) &&
            measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.PrivateRentedSector) &&
            (measureModel.CountryCode == CountryCodes.England || measureModel.CountryCode == CountryCodes.Wales) &&
            decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
            (startingSapRating < minStartingSapRating || startingSapRating > maxStartingSapRating);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;

        public string TestNumber { get; } = "GBIS0204";
    }
}

