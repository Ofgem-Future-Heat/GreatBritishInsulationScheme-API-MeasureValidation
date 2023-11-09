using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0706RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.InnovationMeasureNumber) &&
            !measureModel.InnovationMeasureNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.InnovationMeasureNumber;
        public string TestNumber { get; } = "GBIS0706";
    }
}