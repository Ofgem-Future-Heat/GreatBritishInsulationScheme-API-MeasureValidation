using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Common
{
    public class GbisInFillMeasureReferenceNumberValidatorRuleLogic : IRuleLogic
    {
        public GbisInFillMeasureReferenceNumberValidatorRuleLogic(string testNumber, Func<MeasureModel, string?> measureReferenceNumberFieldFunction)
        {
            TestNumber = testNumber;
            FailureFieldValueFunction = measureReferenceNumberFieldFunction;
            FailureCondition = measureModel =>
                FailureFieldValueFunction(measureModel) != null && !FailureFieldValueFunction(measureModel).CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) && !GbisRegexes.MeasureReferenceNumberRegex()
                    .IsMatch(FailureFieldValueFunction(measureModel)!);
        }

        public Predicate<MeasureModel> FailureCondition { get; }

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; }

        public string TestNumber { get; }
    }
}