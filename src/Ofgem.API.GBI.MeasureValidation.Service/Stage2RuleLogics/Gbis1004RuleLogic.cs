using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
	public class Gbis1004RuleLogic : IRuleLogic
	{
		public Predicate<MeasureModel> FailureCondition { get; } =
			measureModel => measureModel.PurposeOfNotification.CaseInsensitiveEquals(PurposeOfNotificationConstants.EditedNotification) &&
			!measureModel.IsExistingMeasure;

		public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.MeasureReferenceNumber;

		public string TestNumber => "GBIS1004";
	}
}
