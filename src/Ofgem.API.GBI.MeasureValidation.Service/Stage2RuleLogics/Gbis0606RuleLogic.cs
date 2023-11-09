using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
	public class Gbis0606RuleLogic : IRuleLogic
	{
		public Predicate<MeasureModel> FailureCondition { get; } =
			measureModel => measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
			(measureModel.AssociatedInfillMeasure1 == null ||
			!GbisRegexes.MeasureReferenceNumberRegex().IsMatch(measureModel.AssociatedInfillMeasure1));

		public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInfillMeasure1;

		public string TestNumber => "GBIS0606";
	}
}
