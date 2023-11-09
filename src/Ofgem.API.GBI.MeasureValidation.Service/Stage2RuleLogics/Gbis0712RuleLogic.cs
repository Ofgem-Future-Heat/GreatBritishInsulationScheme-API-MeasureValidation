using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0712RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            (!measureModel.VerificationMethod.CaseInsensitiveEquals(VerificationMethods.DwpMatch) &&
            !measureModel.VerificationMethod.CaseInsensitiveEquals(VerificationMethods.EcoEligibleReferral)) &&
            !measureModel.DwpReferenceNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.DwpReferenceNumber;

        public string TestNumber { get; } = "GBIS0712";
    }
}