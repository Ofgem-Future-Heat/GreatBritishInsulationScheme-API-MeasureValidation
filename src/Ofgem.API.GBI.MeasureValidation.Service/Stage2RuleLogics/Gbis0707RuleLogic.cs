using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public partial class Gbis0707RuleLogic : IRuleLogic
    {
        [GeneratedRegex(@"^\d{1,4}?$")]
        private static partial Regex AcceptedFloorAreaRegex();
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.FloorArea) ||
            !AcceptedFloorAreaRegex().IsMatch(measureModel.FloorArea!);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.FloorArea;
        public string TestNumber { get; } = "GBIS0707";
    }
}