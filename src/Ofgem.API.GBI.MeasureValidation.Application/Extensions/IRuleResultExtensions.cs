using DotNetRuleEngine.Interface;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Extensions
{
    public static class IRuleResultExtensions
    {
        public static IList<StageValidationError>? GetMeasureErrors(this IRuleResult ruleResult)
        {
            return ruleResult.Result as IList<StageValidationError>;
        }

        public static void AddError(this IRuleResult ruleResult, StageValidationError error)
        {
            var errors = ruleResult.GetMeasureErrors();

            if (errors == null)
            {
                errors = new List<StageValidationError>
                {
                    error
                };
                ruleResult.Result = errors;
            }
            else
            {
                errors.Add(error);
            }
        }
    }
}
