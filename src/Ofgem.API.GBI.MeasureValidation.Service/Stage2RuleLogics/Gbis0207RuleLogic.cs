using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0207RuleLogic : IRuleLogic
    {
        private static readonly List<string> AcceptedMeasureTypes = new()
        {
            MeasureTypes.Trv,
            MeasureTypes.PAndRt
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType != null &&
            EligibilityTypes.GeneralGroup.CaseInsensitiveEquals(measureModel.EligibilityType) &&
            AcceptedMeasureTypes.CaseInsensitiveContainsInList(measureModel.MeasureType);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.MeasureType;
        public string TestNumber { get; } = "GBIS0207";
    }
}
