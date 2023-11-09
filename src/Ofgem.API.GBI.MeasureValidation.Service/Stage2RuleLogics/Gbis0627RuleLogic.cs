using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0627RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
        {
            var eligibilityCheck = measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill);

            var inFillNaCheck = !measureModel.AssociatedInfillMeasure2.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

            var mrnCheck = measureModel.MeasureReferenceNumber.CaseInsensitiveEquals(measureModel.AssociatedInfillMeasure2);

            return eligibilityCheck && inFillNaCheck && mrnCheck;
        };
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInfillMeasure2;

        public string TestNumber { get; } = "GBIS0627";
    }
}
