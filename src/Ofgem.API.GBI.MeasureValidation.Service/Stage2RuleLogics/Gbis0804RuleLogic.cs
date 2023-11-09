using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0804RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            (measureModel.MeasureType.CaseInsensitiveEquals(MeasureTypes.Trv) ||
            measureModel.MeasureType.CaseInsensitiveEquals(MeasureTypes.PAndRt)) &&
            !measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                DateTime.TryParseExact(measureModel.DateOfCompletedInstallation, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime doci) &&
                measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails?.DateOfCompletedInstallation != null &&
                    (doci < measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails.DateOfCompletedInstallation ||
                    doci > measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails.DateOfCompletedInstallation.Value.AddMonths(3));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.DateOfCompletedInstallation;

        public string TestNumber { get; } = "GBIS0804";

    }
}