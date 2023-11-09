using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0719RuleLogic : IRuleLogic
	{
		public Predicate<MeasureModel> FailureCondition { get; } =
			measureModel => !measureModel.InnovationMeasureNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
			(measureModel.InnovationMeasureNumber == null || !GbisRegexes.InnovationNumberRegex().IsMatch(measureModel.InnovationMeasureNumber));

		public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.InnovationMeasureNumber;

		public string TestNumber => "GBIS0719";
	}
}
