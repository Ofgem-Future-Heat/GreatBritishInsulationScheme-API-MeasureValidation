using Microsoft.IdentityModel.Tokens;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0717RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.PrsSapBandException.IsNullOrEmpty()
            || (!measureModel.PrsSapBandException!.CaseInsensitiveEquals(CommonTypesConstants.Yes) && !measureModel.PrsSapBandException.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.PrsSapBandException;

        public string TestNumber { get; } = "GBIS0717";
    }
}
