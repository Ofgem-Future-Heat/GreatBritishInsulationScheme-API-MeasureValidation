using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Common
{
    public interface IRuleLogic
    {
        Predicate<MeasureModel> FailureCondition { get; }
        Func<MeasureModel, string?> FailureFieldValueFunction { get; }
        string TestNumber { get; }
    }
}
