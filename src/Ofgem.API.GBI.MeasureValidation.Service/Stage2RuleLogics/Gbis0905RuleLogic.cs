using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0905RuleLogic : IRuleLogic
    {
        private static readonly List<string> AcceptedEligibilityTypes = new()
        {
            EligibilityTypes.LILADeclaration,
            EligibilityTypes.LISupplierEvidence

        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            AcceptedEligibilityTypes.CaseInsensitiveContainsInList(measureModel.EligibilityType)
            && measureModel.DateOfHouseholderEligibility.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } =
            measureModel => measureModel.DateOfHouseholderEligibility;

        public string TestNumber { get; } = "GBIS0905";

    }
}
