using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0400RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
        measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LISocialHousing) &&
        !measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.SocialHousing);
       
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TenureType;

        public string TestNumber => "GBIS0400";
    }
}
