using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public partial class Gbis0503RuleLogic : IRuleLogic
    {
        [GeneratedRegex("^[0-9]{1,12}$")]
        private static partial Regex Regex();

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel => 
            string.IsNullOrEmpty(measureModel.UniquePropertyReferenceNumber) ||
            !(measureModel.UniquePropertyReferenceNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) ||
              Regex().IsMatch(measureModel.UniquePropertyReferenceNumber));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.UniquePropertyReferenceNumber;
        public string TestNumber { get; } = "GBIS0503";
    }
}
