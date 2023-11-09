using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
	public class Gbis0900RuleLogic : IRuleLogic
	{
		public Predicate<MeasureModel> FailureCondition { get; } =
			measureModel => !measureModel.FlexReferralRoute.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
			(measureModel.ReferenceDataDetails.FlexReferralRouteList == null ||
			!measureModel.ReferenceDataDetails.FlexReferralRouteList.CaseInsensitiveContainsInList(measureModel.FlexReferralRoute));

		public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.FlexReferralRoute;

		public string TestNumber => "GBIS0900";
	}
}
