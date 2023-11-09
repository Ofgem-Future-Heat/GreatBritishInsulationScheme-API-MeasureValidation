using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0210RuleLogic : IRuleLogic
    {
        private static readonly List<string> CouncilTaxBands = new()
        {
            ReferenceDataConstants.CouncilTaxBand.A,
            ReferenceDataConstants.CouncilTaxBand.B,
            ReferenceDataConstants.CouncilTaxBand.C,
            ReferenceDataConstants.CouncilTaxBand.D,
            ReferenceDataConstants.CouncilTaxBand.E
        };

        private static readonly List<string> CountryCodes = new()
        {
            ReferenceDataConstants.CountryCode.Scotland,
            ReferenceDataConstants.CountryCode.Wales
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel is { CouncilTaxBand: not null, EligibilityType: not null, CountryCode:not null } &&
            EligibilityTypes.GeneralGroup.CaseInsensitiveEquals(measureModel.EligibilityType) &&
            CountryCodes.CaseInsensitiveContainsInList(measureModel.CountryCode) &&
            !CouncilTaxBands.CaseInsensitiveContainsInList(measureModel.CouncilTaxBand);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.CouncilTaxBand;
        public string TestNumber { get; } = "GBIS0210";
    }
}
