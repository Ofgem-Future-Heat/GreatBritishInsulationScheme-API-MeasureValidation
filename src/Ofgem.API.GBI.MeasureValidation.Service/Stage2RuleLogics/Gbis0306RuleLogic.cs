using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0306RuleLogic : IRuleLogic
    {
        private const decimal MinStartingSapRating = 0.0m;
        private const decimal MaxStartingSapRating = 38.4m;

        private static readonly List<string> CountryCodes = new()
        {
            ReferenceDataConstants.CountryCode.England,
            ReferenceDataConstants.CountryCode.Wales
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LIHelpToHeatGroup) &&
            measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.PrivateRentedSector) &&
            CountryCodes.CaseInsensitiveContainsInList(measureModel.CountryCode) &&
            decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
            startingSapRating is >= MinStartingSapRating and <= MaxStartingSapRating &&
            !measureModel.PrsSapBandException.CaseInsensitiveEquals(CommonTypesConstants.Yes);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PrsSapBandException;
        public string TestNumber => "GBIS0306";
    }
}