using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0209RuleLogic : IRuleLogic
    {
        private static readonly List<string> CouncilTaxBands = new()
        {
            ReferenceDataConstants.CouncilTaxBand.A,
            ReferenceDataConstants.CouncilTaxBand.B,
            ReferenceDataConstants.CouncilTaxBand.C,
            ReferenceDataConstants.CouncilTaxBand.D
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel is { CouncilTaxBand: not null, EligibilityType: not null } &&
            EligibilityTypes.GeneralGroup.CaseInsensitiveEquals(measureModel.EligibilityType) &&
            measureModel.CountryCode == ReferenceDataConstants.CountryCode.England &&
            !CouncilTaxBands.CaseInsensitiveContainsInList(measureModel.CouncilTaxBand);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.CouncilTaxBand;
        public string TestNumber { get; } = "GBIS0209";
    }
}
