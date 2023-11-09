using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Service.Common
{
    public class GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic : IRuleLogic
    {
        private readonly IInFillMeasureService? _inFillMeasureService;

        public GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic(string testNumber, Func<MeasureModel, string?> infillAssociatedInfillMeasureFunction, IInFillMeasureService? inFillMeasureService)
        {
            TestNumber = testNumber;
            FailureFieldValueFunction = infillAssociatedInfillMeasureFunction;
            FailureCondition = measureModel =>
                measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
                !infillAssociatedInfillMeasureFunction(measureModel).CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                IsInfillMeasureAlreadyAssigned(measureModel);

            _inFillMeasureService = inFillMeasureService;
        }

        public Predicate<MeasureModel> FailureCondition { get; }

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; }

        public string TestNumber { get; }

        private bool IsInfillMeasureAlreadyAssigned(MeasureModel measureModel)
        {
            if (string.IsNullOrWhiteSpace(measureModel.MeasureReferenceNumber) ||
                string.IsNullOrWhiteSpace(FailureFieldValueFunction(measureModel)))
            {
                return false;
            }

            var result = _inFillMeasureService!.IsInfillMeasureAssigned(measureModel.MeasureReferenceNumber,
                FailureFieldValueFunction(measureModel)!).GetAwaiter().GetResult();

            return result;
        }
    }
}
