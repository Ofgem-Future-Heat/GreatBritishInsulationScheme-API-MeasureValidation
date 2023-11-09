using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0703RuleLogic : IRuleLogic
    {
        private const int MaxLength = 150;

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !string.IsNullOrWhiteSpace(measureModel.InstallerName) &&
            measureModel.InstallerName.Length > MaxLength;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.InstallerName;
        public string TestNumber { get; } = "GBIS0703";
    }
}

