using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Tgbis0102RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.TrustmarkUniqueMeasureReference) ||
            !GbisRegexes.TrustmarkUniqueMeasureReferenceRegex().IsMatch(measureModel.TrustmarkUniqueMeasureReference);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TrustmarkUniqueMeasureReference;

        public string TestNumber => "TGBIS0102";

    }
}