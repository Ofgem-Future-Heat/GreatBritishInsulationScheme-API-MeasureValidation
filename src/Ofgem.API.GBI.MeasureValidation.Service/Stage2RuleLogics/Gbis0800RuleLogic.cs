using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0800RuleLogic : IRuleLogic
    {
        private static readonly List<string> MeasureTypes = new() { Application.Constants.MeasureTypes.Trv, Application.Constants.MeasureTypes.PAndRt };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.MeasureType != null &&
            MeasureTypes.Contains(measureModel.MeasureType) &&
               (!EligibilityTypes.LIHelpToHeatGroup.CaseInsensitiveEquals(measureModel.EligibilityType) ||
                    !TenureTypes.OwnerOccupied.CaseInsensitiveEquals(measureModel.TenureType));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => $"{measureModel.EligibilityType} | {measureModel.TenureType}";

        public string TestNumber => "GBIS0800";
    }
}
