using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

public class Tgbis0101RuleLogic : IRuleLogic
{
    public Predicate<MeasureModel> FailureCondition => measureModel => string.IsNullOrWhiteSpace(measureModel.TrustmarkLodgedCertificateID) || 
    !GbisRegexes.TrustmarkLodgedProjectCertIdRegex().IsMatch(measureModel.TrustmarkLodgedCertificateID);

    public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TrustmarkLodgedCertificateID;

    public string TestNumber => "TGBIS0101";
}