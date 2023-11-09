using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Tgbis0104RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.TrustMarkCompletedProjectCertID) ||
            !GbisRegexes.TrustmarkCompletedProjectCertIdRegex().IsMatch(measureModel.TrustMarkCompletedProjectCertID);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TrustMarkCompletedProjectCertID;

        public string TestNumber => "TGBIS0104";

    }
}