using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public partial class Gbis0710RuleLogic : IRuleLogic
    {
        [GeneratedRegex("^\\d{10}$")]
        private static partial Regex AcceptedDwpRefNumFormatRegex();

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.VerificationMethod.CaseInsensitiveEquals(VerificationMethods.DwpMatch) &&
            (string.IsNullOrWhiteSpace(measureModel.DwpReferenceNumber) ||
            !AcceptedDwpRefNumFormatRegex().IsMatch(measureModel.DwpReferenceNumber));   
            
        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.DwpReferenceNumber; 

        public string TestNumber { get; } = "GBIS0710";

    }
}