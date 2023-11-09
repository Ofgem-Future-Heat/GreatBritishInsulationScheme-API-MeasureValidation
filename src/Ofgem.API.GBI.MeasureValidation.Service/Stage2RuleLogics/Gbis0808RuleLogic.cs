using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0808RuleLogic : IRuleLogic
    {
        private static readonly List<string> HeatingMeasureTypes = new()
        {
            MeasureTypes.Trv,
            MeasureTypes.PAndRt
        }; 
        
        private static List<string> AcceptableProperties => new()
        {
            PropertyTypes.House,
            PropertyTypes.Bungalow,
            PropertyTypes.ParkHome,
        };
        
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            HeatingMeasureTypes.CaseInsensitiveContainsInList(measureModel.MeasureType) &&
            AcceptableProperties.CaseInsensitiveContainsInList(measureModel.Property) &&
            !measureModel.AssociatedInsulationMrnForHeatingMeasures.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
            (
                !measureModel.PostCode.CaseInsensitiveEquals(measureModel
                    .AssociatedInsulationMeasureForHeatingMeasureDetails?.Address.PostCode) ||
                !measureModel.BuildingName.CaseInsensitiveEquals(measureModel
                    .AssociatedInsulationMeasureForHeatingMeasureDetails?.Address.BuildingName) ||
                !measureModel.BuildingNumber.CaseInsensitiveEquals(measureModel
                    .AssociatedInsulationMeasureForHeatingMeasureDetails?.Address.BuildingNumber) ||
                !measureModel.StreetName.CaseInsensitiveEquals(measureModel
                    .AssociatedInsulationMeasureForHeatingMeasureDetails?.Address.StreetName) ||
                !measureModel.Town.CaseInsensitiveEquals(measureModel
                    .AssociatedInsulationMeasureForHeatingMeasureDetails?.Address.Town)
                );

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } =
            measureModel => measureModel.AssociatedInsulationMrnForHeatingMeasures;

        public string TestNumber => "GBIS0808";
    }
}
