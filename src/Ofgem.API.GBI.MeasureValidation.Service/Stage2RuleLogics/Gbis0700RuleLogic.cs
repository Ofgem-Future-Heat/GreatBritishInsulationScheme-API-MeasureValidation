using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0700RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !measureModel.PrivateDomesticPremises.CaseInsensitiveEquals(CommonTypesConstants.Yes) &&
            !measureModel.PrivateDomesticPremises.CaseInsensitiveEquals(CommonTypesConstants.No);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PrivateDomesticPremises;
        public string TestNumber { get; } = "GBIS0700";
    }
}
