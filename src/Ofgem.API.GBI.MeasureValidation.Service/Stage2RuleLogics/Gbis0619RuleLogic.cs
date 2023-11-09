using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

public class Gbis0619RuleLogic : IRuleLogic
{
    public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
    {
        var eligibilityCheck = measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill);

        var inFillNaCheck =
            !measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
            !measureModel.AssociatedInfillMeasure2.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
            !measureModel.AssociatedInfillMeasure3.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

        var propertyTypeCheck = measureModel.Property.CaseInsensitiveEquals(PropertyTypes.House) ||
                                measureModel.Property.CaseInsensitiveEquals(PropertyTypes.Bungalow) ||
                                measureModel.Property.CaseInsensitiveEquals(PropertyTypes.ParkHome);

        var checkDociDates = false;

        if (measureModel.AssociatedInfillMeasure1Details != null && measureModel.AssociatedInfillMeasure2Details != null && measureModel.AssociatedInfillMeasure3Details != null)
        {
            var latestAssocInfilDate = new List<DateTime?>
            {
                measureModel.AssociatedInfillMeasure1Details.DateOfCompletedInstallation,
                measureModel.AssociatedInfillMeasure2Details.DateOfCompletedInstallation,
                measureModel.AssociatedInfillMeasure3Details.DateOfCompletedInstallation
            }.Max();


            if (!string.IsNullOrWhiteSpace(measureModel.DateOfCompletedInstallation) && measureModel.AssociatedInfillMeasure1Details.DateOfCompletedInstallation.HasValue)
            {
                checkDociDates = CheckDateTime((DateTime)latestAssocInfilDate!, measureModel.DateOfCompletedInstallation);
            }
        }

        return eligibilityCheck && inFillNaCheck && propertyTypeCheck && checkDociDates;
    };

    public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => $"{measureModel.AssociatedInfillMeasure1} | {measureModel.AssociatedInfillMeasure2} | {measureModel.AssociatedInfillMeasure3}";

    public string TestNumber { get; } = "GBIS0619";

    private static bool CheckDateTime(DateTime assocInfillMeasureDoci, string measureModelDoci)
    {
        if (DateTime.TryParseExact(measureModelDoci, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var doci))
        {
            var assocInfilMeasureDociMaxRange = assocInfillMeasureDoci.AddMonths(3);
            return !(assocInfilMeasureDociMaxRange >= doci && doci >= assocInfillMeasureDoci);
        }

        return false;
    }
}