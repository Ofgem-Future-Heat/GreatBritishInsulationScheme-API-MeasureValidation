using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Tgbis0100RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.TrustmarkBusinessLicenceNumber) ||
            !GbisRegexes.TrustmarkBusinessLicenseNumberRegex().IsMatch(measureModel.TrustmarkBusinessLicenceNumber);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TrustmarkBusinessLicenceNumber;

        public string TestNumber => "TGBIS0100";

    }
}