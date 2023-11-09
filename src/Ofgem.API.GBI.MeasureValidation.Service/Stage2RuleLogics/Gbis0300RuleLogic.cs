using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0300RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LIHelpToHeatGroup) &&
            measureModel.PrivateDomesticPremises.CaseInsensitiveEquals("No");

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PrivateDomesticPremises;

        public string TestNumber => "GBIS0300";
    }
}