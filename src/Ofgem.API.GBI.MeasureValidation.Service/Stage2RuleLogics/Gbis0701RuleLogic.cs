using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public partial class Gbis0701RuleLogic : IRuleLogic
    {
        [GeneratedRegex(@"^\d{1,2}(\.\d{1})?$")]
        private static partial Regex AcceptedStartingSapRatingRegex();

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !AcceptedStartingSapRatingRegex().IsMatch(measureModel.StartingSAPRating!);
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.StartingSAPRating;
        public string TestNumber => "GBIS0701";
    }
}
