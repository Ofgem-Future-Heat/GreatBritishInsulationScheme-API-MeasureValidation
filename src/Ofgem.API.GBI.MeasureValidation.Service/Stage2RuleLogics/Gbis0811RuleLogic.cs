using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0811RuleLogic : IRuleLogic
    {
        private static readonly List<string> measureTypes = new()
        {
            MeasureTypes.PAndRt,
            MeasureTypes.Trv
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measure =>
            measureTypes.CaseInsensitiveContainsInList(measure.MeasureType) &&
            CommonTypesConstants.NotApplicable.CaseInsensitiveEquals(measure.HeatingSource);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } =
            measureModel => measureModel.HeatingSource;

        public string TestNumber => "GBIS0811";
    }
}