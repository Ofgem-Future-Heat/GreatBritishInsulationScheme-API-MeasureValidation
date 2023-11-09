using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
	public class Gbis0813RuleLogic : IRuleLogic
	{
		public Predicate<MeasureModel> FailureCondition { get; } =
			measureModel => !measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
			measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(measureModel.MeasureReferenceNumber);

		public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInsulationMrnForHeatingMeasures;

		public string TestNumber => "GBIS0813";
	}
}
