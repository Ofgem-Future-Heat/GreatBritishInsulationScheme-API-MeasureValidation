using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0704RuleLogic : IRuleLogic
    {
        private static readonly List<string> PropertyTypesToCheck = new()
        {
            PropertyTypes.Bungalow,
            PropertyTypes.House,
            PropertyTypes.Flat,
            PropertyTypes.Maisonette,
            PropertyTypes.ParkHome,
        };
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !PropertyTypesToCheck.CaseInsensitiveContainsInList(measureModel.Property);
  
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.Property;
        public string TestNumber { get; } = "GBIS0704";
    }
}