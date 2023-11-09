using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Tgbis0103RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.TrustmarkProjectReferenceNumber) ||
            !GbisRegexes.TrustmarkProjectReferenceNumberRegex().IsMatch(measureModel.TrustmarkProjectReferenceNumber);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TrustmarkProjectReferenceNumber;

        public string TestNumber => "TGBIS0103";
    }
}