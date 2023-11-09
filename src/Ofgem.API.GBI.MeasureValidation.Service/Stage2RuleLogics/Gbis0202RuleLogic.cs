using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0202RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType == EligibilityTypes.GeneralGroup &&
            !string.IsNullOrWhiteSpace(measureModel.TenureType) &&
            measureModel.TenureType.Equals(TenureTypes.SocialHousing, StringComparison.OrdinalIgnoreCase);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TenureType;
        public string TestNumber => "GBIS0202";
    }
}
