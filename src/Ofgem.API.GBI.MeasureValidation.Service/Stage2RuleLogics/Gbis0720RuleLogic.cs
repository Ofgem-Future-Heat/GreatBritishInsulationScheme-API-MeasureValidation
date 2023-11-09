using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0720RuleLogic : IRuleLogic
	{
		public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
		{
			var innovationMeasures = measureModel.ReferenceDataDetails.InnovationMeasureList ?? new List<InnovationMeasureDto>();

			return !measureModel.InnovationMeasureNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
				!innovationMeasures.Any(
					m => m.InnovationMeasureNumber.CaseInsensitiveEquals(measureModel.InnovationMeasureNumber) &&
					m.MeasureTypeName.CaseInsensitiveEquals(measureModel.MeasureType));
		};

		public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.InnovationMeasureNumber;

		public string TestNumber => "GBIS0720";
	}
}
