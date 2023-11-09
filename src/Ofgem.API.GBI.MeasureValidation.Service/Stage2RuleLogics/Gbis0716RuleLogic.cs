using Microsoft.IdentityModel.Tokens;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0716RuleLogic : IRuleLogic
    {
        private static readonly IEnumerable<string> CouncilTaxBands = new List<string>()
        {
            ReferenceDataConstants.CouncilTaxBand.A,
            ReferenceDataConstants.CouncilTaxBand.B,
            ReferenceDataConstants.CouncilTaxBand.C,
            ReferenceDataConstants.CouncilTaxBand.D,
            ReferenceDataConstants.CouncilTaxBand.E,
        };

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel => 
            measureModel.CouncilTaxBand != null && (measureModel.CouncilTaxBand.IsNullOrEmpty() 
                                                    || (!CouncilTaxBands.CaseInsensitiveContainsInList(measureModel.CouncilTaxBand) && !measureModel.CouncilTaxBand.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable)));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.CouncilTaxBand;

       public string TestNumber { get; } = "GBIS0716";
    }
}
