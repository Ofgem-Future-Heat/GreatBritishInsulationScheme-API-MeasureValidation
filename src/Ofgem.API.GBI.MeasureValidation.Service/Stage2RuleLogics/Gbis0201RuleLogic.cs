using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0201RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType == EligibilityTypes.GeneralGroup &&
            (string.IsNullOrWhiteSpace(measureModel.PrivateDomesticPremises) ||
             !measureModel.PrivateDomesticPremises.CaseInsensitiveEquals("Yes"));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PrivateDomesticPremises;

        public string TestNumber { get; } = "GBIS0201";
    }
}
