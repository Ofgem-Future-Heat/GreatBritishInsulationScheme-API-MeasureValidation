using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0715RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !EligibilityTypes.GeneralGroup.CaseInsensitiveEquals(measureModel.EligibilityType) &&
            !measureModel.CouncilTaxBand.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);
  
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.CouncilTaxBand;

        public string TestNumber { get; } = "GBIS0715";
    }
}