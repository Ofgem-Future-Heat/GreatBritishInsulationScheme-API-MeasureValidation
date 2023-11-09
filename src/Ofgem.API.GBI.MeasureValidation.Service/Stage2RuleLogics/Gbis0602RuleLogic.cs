using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0602RuleLogic : IRuleLogic
    {
        private static List<string> AcceptableProperties => new()
        {
            PropertyTypes.House,
            PropertyTypes.Bungalow,
            PropertyTypes.ParkHome
        };

        public Predicate<MeasureModel> FailureCondition { get; } =
            measureModel => measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
                            AcceptableProperties.CaseInsensitiveContainsInList(measureModel.Property) &&
                            !measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                            !measureModel.AssociatedInfillMeasure2.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                            !measureModel.AssociatedInfillMeasure3.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                            (
                                !measureModel.StreetName.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure1Details?.Address.StreetName) ||
                                !measureModel.Town.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure1Details?.Address.Town) ||
                                !measureModel.StreetName.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure2Details?.Address.StreetName) ||
                                !measureModel.Town.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure2Details?.Address.Town) ||
                                !measureModel.StreetName.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure3Details?.Address.StreetName) ||
                                !measureModel.Town.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure3Details?.Address.Town) 
                            );

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } =
            measureModel => $"{measureModel.AssociatedInfillMeasure1} | {measureModel.AssociatedInfillMeasure2} | {measureModel.AssociatedInfillMeasure3}";

        public string TestNumber => "GBIS0602";
    }
}
