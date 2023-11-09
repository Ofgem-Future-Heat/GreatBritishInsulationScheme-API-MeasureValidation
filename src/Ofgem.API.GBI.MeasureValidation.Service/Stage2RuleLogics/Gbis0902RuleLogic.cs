using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0902RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.LILADeclaration) &&
            !measureModel.LaDeclarationReferenceNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.EligibilityType;

        public string TestNumber => "GBIS0902";

    }
}