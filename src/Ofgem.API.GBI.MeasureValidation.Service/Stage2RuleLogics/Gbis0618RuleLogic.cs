using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0618RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
        {
            var eligibilityCheck = measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill);
            var inFillNaCheck = !measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);
            var propertyTypeCheck = (measureModel.Property.CaseInsensitiveEquals(PropertyTypes.Flat) || measureModel.Property.CaseInsensitiveEquals(PropertyTypes.Maisonette));
            var checkDociDates = false;

            if (!string.IsNullOrWhiteSpace(measureModel.DateOfCompletedInstallation) && measureModel.AssociatedInfillMeasure1Details != null && measureModel.AssociatedInfillMeasure1Details.DateOfCompletedInstallation.HasValue)
            {
                checkDociDates = CheckDateTime(measureModel.AssociatedInfillMeasure1Details.DateOfCompletedInstallation.Value, measureModel.DateOfCompletedInstallation);
            }

            return eligibilityCheck && inFillNaCheck && propertyTypeCheck && checkDociDates;
        };

        private static bool CheckDateTime(DateTime assocInfilMeasureDoci, string measureModelDoci)
        {
            if (DateTime.TryParseExact(measureModelDoci, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var doci))
            {
                var assocInfilMeasureDociMaxRange = assocInfilMeasureDoci.AddMonths(3);

                return !((assocInfilMeasureDociMaxRange >= doci) && (doci >= assocInfilMeasureDoci));
            }
            return false;
        }

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInfillMeasure1;
        public string TestNumber { get; } = "GBIS0618";
    }
}