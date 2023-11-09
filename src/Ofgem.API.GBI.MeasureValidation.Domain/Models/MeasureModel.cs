using CsvHelper.Configuration.Attributes;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;

namespace Ofgem.API.GBI.MeasureValidation.Domain.Models
{
    public class MeasureModel
    {
        [Name("Supplier_Reference")]
        public string? SupplierReference { get; set; }
        [Name("Measure_Reference_Number")]
        public string? MeasureReferenceNumber { get; set; }
        [Name("Measure_Type")]
        public string? MeasureType { get; set; }
        [Name("Purpose_of_Notification")]
        public string? PurposeOfNotification { get; set; }
        [Name("Date_of_Completed_Installation")]
        public string? DateOfCompletedInstallation { get; set; }
        [Name("Building_Number")]
        public string? BuildingNumber { get; set; }
        [Name("Building_Name")]
        public string? BuildingName { get; set; }
        [Name("Flat_Name/Number")]
        public string? FlatNameNumber { get; set; }
        [Name("Street_Name")]
        public string? StreetName { get; set; }
        [Name("Town")]
        public string? Town { get; set; }
        [Name("Post_Code")]
        public string? PostCode { get; set; }
        [Name("Unique_Property_Reference_Number_(UPRN)")]
        public string? UniquePropertyReferenceNumber { get; set; }
        [Name("Starting_SAP_Rating")]
        public string? StartingSAPRating { get; set; }
        [Name("Floor_Area")]
        public string? FloorArea { get; set; }
        [Name("Rural")]
        public string? Rural { get; set; }
        [Name("Off_Gas")]
        public string? OffGas { get; set; }
        [Name("Private_Domestic_Premises")]
        public string? PrivateDomesticPremises { get; set; }
        [Name("Tenure_Type")]
        public string? TenureType { get; set; }
        [Name("Property")]
        public string? Property { get; set; }
        [Name("Eligibility_Type")]
        public string? EligibilityType { get; set; }
        [Name("Verification_Method")]
        public string? VerificationMethod { get; set; }
        [Name("DWP_Reference_Number")]
        public string? DwpReferenceNumber { get; set; }
        [Name("Council_Tax_Band")]
        public string? CouncilTaxBand { get; set; }
        [Name("PRS_SAP_Band_Exception")]
        public string? PrsSapBandException { get; set; }
        [Name("Associated_Insulation_MRN_for_Heating_Measures")]
        public string? AssociatedInsulationMrnForHeatingMeasures { get; set; }
        [Name("Associated_In-fill_Measure_1")]
        public string? AssociatedInfillMeasure1 { get; set; }
        [Name("Associated_In-fill_Measure_2")]
        public string? AssociatedInfillMeasure2 { get; set; }
        [Name("Associated_In-fill_Measure_3")]
        public string? AssociatedInfillMeasure3 { get; set; }
        [Name("Flex_Referral_Route")]
        public string? FlexReferralRoute { get; set; }
        [Name("LA_Declaration_Reference_Number")]
        public string? LaDeclarationReferenceNumber { get; set; }
        [Name("Date_Of_Householder_Eligibility")]
        public string? DateOfHouseholderEligibility { get; set; }
        [Name("Percentage_Of_Property_Treated")]
        public string? PercentageOfPropertyTreated { get; set; }
        [Name("Heating_Source")]
        public string? HeatingSource { get; set; }
        [Name("Installer_Name")]
        public string? InstallerName { get; set; }
        [Name("Innovation_Measure_Number")]
        public string? InnovationMeasureNumber { get; set; }
        [Name("Trustmark_Business_Licence_Number")]
        public string? TrustmarkBusinessLicenceNumber { get; set; }
        [Name("Trustmark_Unique_Measure_Reference")]
        public string? TrustmarkUniqueMeasureReference { get; set; }
        [Name("Trustmark_Lodged_CertificateID")]
        public string? TrustmarkLodgedCertificateID { get; set; }
        [Name("Trustmark_Project_Reference_Number")]
        public string? TrustmarkProjectReferenceNumber { get; set; }
        [Name("TrustMark_Completed_Project_Cert_ID")]
        public string? TrustMarkCompletedProjectCertID { get; set; }
        [Ignore]
        public string? DocumentId { get; set; }
        [Ignore]
        public string? FileName { get; set; }
        [Ignore]
        public DateTime? CreatedDate { get; set; }
        [Ignore]
        public string? SupplierName { get; set; }
        [Ignore]
        public IEnumerable<SupplierLicenceResponse> SupplierLicences { get; set; } = new List<SupplierLicenceResponse>();
        [Ignore]
        public ReferenceDataDetails ReferenceDataDetails { get; set; } = new();
        [Ignore]
        public bool IsExistingMeasure { get; set; }
		[Ignore]
        public string? ExistingSupplierReference { get; set; }
        [Ignore]
        public int? MeasureStatusId { get; set; }
        [Ignore]
        public int? MeasureCategoryId { get; set; }
        [Ignore]
        public string? CountryCode { get; set; }

        [Ignore]
        public bool AddressIsVerified { get; set; } = false;

        [Ignore] 
        public string? VerifiedUprn { get; set; }

        [Ignore]
        public AssociatedMeasureModelDto? AssociatedInfillMeasure1Details { get; set; }

		[Ignore]
		public AssociatedMeasureModelDto? AssociatedInfillMeasure2Details { get; set; }

		[Ignore]
		public AssociatedMeasureModelDto? AssociatedInfillMeasure3Details { get; set; }

		[Ignore]
		public AssociatedMeasureModelDto? AssociatedInsulationMeasureForHeatingMeasureDetails { get; set; }

        [Ignore]
        public FivePercentExtensionQuotaDto? FivePercentExtensionDto { get; set; }
    }
}
