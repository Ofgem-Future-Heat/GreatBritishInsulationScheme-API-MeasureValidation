using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0208RuleLogic : IRuleLogic
    {
        private readonly IMeasureTypeToCategoryService _measureTypeToCategoryService;
        
        public Gbis0208RuleLogic(IMeasureTypeToCategoryService measureTypeToCategoryService)
        {
            _measureTypeToCategoryService = measureTypeToCategoryService;

            FailureCondition = measureModel =>
                measureModel.EligibilityType.CaseInsensitiveEquals(EligibilityTypes.GeneralGroup) &&
                measureModel.TenureType.CaseInsensitiveEquals(TenureTypes.PrivateRentedSector) &&
                CheckMeasureCategory(measureModel.MeasureType ?? string.Empty);
        }

       private static readonly List<string> UnacceptedMeasureCategories = new()
        {
            MeasureCategories.CavityWallInsulation,
            MeasureCategories.LoftInsulation
        };

        private bool CheckMeasureCategory(string ?measureType)
        {
            if (measureType == null) return false;
            var measureCategory = GetMeasureCategory(measureType);
            return UnacceptedMeasureCategories.CaseInsensitiveContainsInList(measureCategory);
        }
              
        
        public Predicate<MeasureModel> FailureCondition { get; init; }

        private string GetMeasureCategory(string measureType)
        {
            var result = _measureTypeToCategoryService.GetMeasureCategoryByType(measureType).Result;
            return result;
        }

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.MeasureType;

        public string TestNumber { get; } = "GBIS0208";
    }
}

