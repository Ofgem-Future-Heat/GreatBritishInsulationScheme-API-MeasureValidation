using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0803RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
               !measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                (measureModel.MeasureType == MeasureTypes.Trv || 
                measureModel.MeasureType == MeasureTypes.PAndRt) &&
                !measureModel.SupplierReference.CaseInsensitiveEquals(measureModel.AssociatedInsulationMeasureForHeatingMeasureDetails?.SupplierReference);

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.SupplierReference;

        public string TestNumber => "GBIS0803";
    }
}
