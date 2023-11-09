using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
	public class Gbis0814RuleLogic : IRuleLogic
	{
		private static readonly List<string> HeatingControlMeasureTypes = new()
		{
			MeasureTypes.Trv,
			MeasureTypes.PAndRt
		};

		public Predicate<MeasureModel> FailureCondition { get; } =
			measureModel => !measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
			measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails?.MeasureType != null &&
			HeatingControlMeasureTypes.Contains(measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails.MeasureType);

		public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInsulationMrnForHeatingMeasures;

		public string TestNumber => "GBIS0814";
	}
}
