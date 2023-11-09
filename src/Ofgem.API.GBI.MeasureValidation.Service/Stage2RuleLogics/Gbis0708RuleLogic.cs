using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0708RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !DateTime.TryParseExact((measureModel.DateOfHouseholderEligibility), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) &&
            !measureModel.DateOfHouseholderEligibility.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.DateOfHouseholderEligibility;
        public string TestNumber { get; } = "GBIS0708";
    }
}