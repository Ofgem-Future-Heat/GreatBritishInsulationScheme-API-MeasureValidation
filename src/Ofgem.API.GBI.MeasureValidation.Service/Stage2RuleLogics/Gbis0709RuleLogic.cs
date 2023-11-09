using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0709RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel => string.IsNullOrWhiteSpace(measureModel.VerificationMethod) || 
                                       !measureModel.ReferenceDataDetails.VerificationMethodTypesList!.CaseInsensitiveContainsInList(measureModel.VerificationMethod);
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.VerificationMethod;

        public string TestNumber => "GBIS0709";
    }
}