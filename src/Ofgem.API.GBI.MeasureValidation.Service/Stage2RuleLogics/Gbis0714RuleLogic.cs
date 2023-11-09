using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public partial class Gbis0714RuleLogic : IRuleLogic
    {
        [GeneratedRegex(@"^(?:[1-9]|[1-5][0-9]|6[0-6]|67\+)$")]
        private static partial Regex AcceptedPercentageTreatedRegex();

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.PercentageOfPropertyTreated) ||
            !AcceptedPercentageTreatedRegex().IsMatch(measureModel.PercentageOfPropertyTreated);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PercentageOfPropertyTreated;

        public string TestNumber { get; } = "GBIS0714";

    }
}