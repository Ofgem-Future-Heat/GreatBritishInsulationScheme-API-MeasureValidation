using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0904RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !string.IsNullOrEmpty(measureModel.LaDeclarationReferenceNumber) &&
            !(GbisRegexes.LaDeclarationReferenceRegex().IsMatch(measureModel.LaDeclarationReferenceNumber) || measureModel.LaDeclarationReferenceNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } =
            measureModel => measureModel.LaDeclarationReferenceNumber;
        public string TestNumber { get; } = "GBIS0904";
    }
}

