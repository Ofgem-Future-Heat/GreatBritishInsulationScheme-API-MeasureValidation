using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;

namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class ReferenceDataDetails
    {
        public IEnumerable<string>? PurposeOfNotificationsList { get; set; }
        public IEnumerable<MeasureTypeDto>? MeasureTypesList { get; set; }
        public IEnumerable<string>? EligibilityTypesList { get; set; }
        public IEnumerable<string>? TenureTypesList { get; set; }
        public IEnumerable<string>? VerificationMethodTypesList { get; set; }
        public IEnumerable<InnovationMeasureDto>? InnovationMeasureList { get; set; }
        public IEnumerable<string>? FlexReferralRouteList { get; set; }
    }
}
