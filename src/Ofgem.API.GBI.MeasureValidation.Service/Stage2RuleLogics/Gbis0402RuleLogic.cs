using Microsoft.IdentityModel.Tokens;
using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0402RuleLogic : IRuleLogic
    {
        private readonly IMeasureTypeToInnovationMeasureService _measureTypeToInnovationMeasureService;

        private const decimal minStartingSapRating = 54.5m;
        private const decimal maxStartingSapRating = 68.4m;

        public Gbis0402RuleLogic(IMeasureTypeToInnovationMeasureService measureTypeToInnovationMeasureService)
        {
            _measureTypeToInnovationMeasureService = measureTypeToInnovationMeasureService;

            FailureCondition = measureModel => !string.IsNullOrWhiteSpace(measureModel.EligibilityType) &&
                 measureModel.EligibilityType.Equals(EligibilityTypes.LISocialHousing) &&
                    decimal.TryParse(measureModel.StartingSAPRating, out decimal startingSapRating) &&
                    startingSapRating is >= minStartingSapRating and <= maxStartingSapRating &&
                        CheckInnovationNumbersForMeasureTypeExist(measureModel.MeasureType ?? string.Empty);
        }

        public Predicate<MeasureModel> FailureCondition { get; init; }

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.MeasureType;

        public string TestNumber => "GBIS0402";

        private bool CheckInnovationNumbersForMeasureTypeExist(string measureType)
        {
            var innovationNumbersForType = GetInnovationNumbersForMeasureType(measureType).Result;
            return innovationNumbersForType.IsNullOrEmpty();
        } 

        private Task<List<string>> GetInnovationNumbersForMeasureType(string measureType)
        {
            return _measureTypeToInnovationMeasureService.GetMeasureTypeInnovationNumbers(measureType);
        }
    }
}
