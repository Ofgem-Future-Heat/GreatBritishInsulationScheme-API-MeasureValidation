using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public partial class Gbis0711RuleLogic : IRuleLogic
    {

        [GeneratedRegex("^[A-Za-z]{3}_[A-Za-z]{1}\\d{1,45}$")]
        private static partial Regex AcceptedDwpFormatOneRegex();

        [GeneratedRegex("^[A-Za-z]{3}_[A-Za-z]{2}\\d{1,44}$")]
        private static partial Regex AcceptedDwpFormatTwoRegex();

        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            measureModel.VerificationMethod.CaseInsensitiveEquals(VerificationMethods.EcoEligibleReferral) &&
            (string.IsNullOrWhiteSpace(measureModel.DwpReferenceNumber) ||
                (!AcceptedDwpFormatOneRegex().IsMatch(measureModel.DwpReferenceNumber) &&
                 !AcceptedDwpFormatTwoRegex().IsMatch(measureModel.DwpReferenceNumber)));

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = measureModel => measureModel.DwpReferenceNumber;

        public string TestNumber { get; } = "GBIS0711";

    }
}