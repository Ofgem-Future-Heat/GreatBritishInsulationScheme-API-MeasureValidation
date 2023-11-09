﻿using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0633RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.InFill) &&
            (!measureModel.AssociatedInfillMeasure2.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
             (
              measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(measureModel.AssociatedInfillMeasure2) ||
              measureModel.AssociatedInfillMeasure3.CaseInsensitiveEquals(measureModel.AssociatedInfillMeasure2)
              ));


        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.AssociatedInfillMeasure2;

        public string TestNumber => "GBIS0633";
    }
}
