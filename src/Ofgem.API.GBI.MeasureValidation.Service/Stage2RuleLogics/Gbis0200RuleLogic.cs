using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0200RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            string.IsNullOrWhiteSpace(measureModel.TenureType) ||
            !measureModel.ReferenceDataDetails.TenureTypesList!.CaseInsensitiveContainsInList(measureModel.TenureType);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.TenureType;
        public string TestNumber => "GBIS0200";
    }
}
