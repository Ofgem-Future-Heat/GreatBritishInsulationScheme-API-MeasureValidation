﻿using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0601RuleLogic : IRuleLogic
    {
        private static List<string> AcceptableProperties => new()
        {
            PropertyTypes.Flat,
            PropertyTypes.Maisonette
        };

        public Predicate<MeasureModel> FailureCondition { get; } =
            measureModel => measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
                            AcceptableProperties.CaseInsensitiveContainsInList(measureModel.Property) &&
                            !measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                            (
                                !measureModel.PostCode.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure1Details?.Address.PostCode) ||
                                !measureModel.BuildingName.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure1Details?.Address.BuildingName) ||
                                !measureModel.StreetName.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure1Details?.Address.StreetName) ||
                                !measureModel.Town.CaseInsensitiveEquals(measureModel
                                    .AssociatedInfillMeasure1Details?.Address.Town)
                            );

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } =
            measureModel => measureModel.AssociatedInfillMeasure1;

        public string TestNumber => "GBIS0601";
    }
}
