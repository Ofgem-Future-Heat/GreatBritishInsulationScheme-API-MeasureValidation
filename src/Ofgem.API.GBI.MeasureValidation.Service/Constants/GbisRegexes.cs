using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Constants
{
    public static partial class GbisRegexes
    {
        [GeneratedRegex(@"^[a-zA-Z]{3}\d{10}\z")]
        public static partial Regex MeasureReferenceNumberRegex();

        [GeneratedRegex(@"^[0-9]{3}$")]
        public static partial Regex InnovationNumberRegex();

        [GeneratedRegex(@"^[0-9]{12}$")]
        public static partial Regex TrustmarkBusinessLicenseNumberRegex();

        [GeneratedRegex(@"[a-zA-Z]{1}[0-9]{4,7}-[0-9]{1}$")]
        public static partial Regex TrustmarkLodgedProjectCertIdRegex();

        [GeneratedRegex(@"^[a-zA-Z0-9]{1,20}$")]
        public static partial Regex TrustmarkUniqueMeasureReferenceRegex();

        [GeneratedRegex(@"^[a-zA-Z]{1}[0-9]{4,7}-[a-zA-Z]{1}$")]
        public static partial Regex TrustmarkCompletedProjectCertIdRegex();

        [GeneratedRegex(@"^[a-zA-Z]{1}[0-9]{4,7}$")]
        public static partial Regex TrustmarkProjectReferenceNumberRegex();

        [GeneratedRegex(@"^[a-zA-Z]\d{8}-\d{5}$")]
        public static partial Regex LaDeclarationReferenceRegex();

    }
}
