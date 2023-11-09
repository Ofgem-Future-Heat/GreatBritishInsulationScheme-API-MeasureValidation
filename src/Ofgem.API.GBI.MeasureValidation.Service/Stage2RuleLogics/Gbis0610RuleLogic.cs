using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;


namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0610RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } =
            measureModel =>  (!measureModel.AssociatedInfillMeasure1.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                              !measureModel.AssociatedInfillMeasure2.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) &&
                              !measureModel.AssociatedInfillMeasure3.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable)) &&
                                (measureModel.Property.CaseInsensitiveEquals(PropertyTypes.House) || measureModel.Property.CaseInsensitiveEquals(PropertyTypes.Bungalow) ||
                                 measureModel.Property.CaseInsensitiveEquals(PropertyTypes.ParkHome)) &&
                                    !measureModel.ReferenceDataDetails.MeasureTypesList!.Any(m => m.Name.CaseInsensitiveEquals(measureModel.MeasureType) &&
                                     m.MeasureCategoryName == MeasureCategories.ExternalInternalWallInsulation); 

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.MeasureType;


        public string TestNumber => "GBIS0610";
    }
}
