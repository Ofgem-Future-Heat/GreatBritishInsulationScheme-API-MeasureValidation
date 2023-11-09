using AutoMapper;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.Database.GBI.Measures.Domain.Dtos.MeasureUpload;
using Ofgem.Database.GBI.Measures.Domain.Entities;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            DateTime dateOfHouseholderEligibility;

            CreateMap<Stage1ValidationResultModel, Stage1ValidationResult>();
            CreateMap<StageValidationError, ValidationError>();
            CreateMap<Stage1ValidationResult, Stage1ValidationResultResponse>();
            CreateMap<ValidationError, ErrorReportResponse>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate!.Value.ToString("dd-MMMM-yy")))
                .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.ErrorMessage!.Field))
                .ForMember(dest => dest.ReasonForError, opt => opt.MapFrom(src => src.ErrorMessage!.ReasonForError))
                .ForMember(dest => dest.HowToFix, opt => opt.MapFrom(src => src.ErrorMessage!.HowToFix));

            CreateMap<MeasureModel, MeasureDto>()
                .ForMember(dest => dest.MeasureReferenceNumber,
                    opt => opt.MapFrom(src => src.MeasureReferenceNumber!.ToUpper()))
                .ForMember(dest => dest.DateOfCompletedInstallation, opt => opt.MapFrom(src => 
                    DateTime.ParseExact(src.DateOfCompletedInstallation!, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.DateOfHouseholderEligibility, opt => opt.MapFrom(src =>
                    DateTime.TryParseExact(src.DateOfHouseholderEligibility, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfHouseholderEligibility)
                        ? dateOfHouseholderEligibility
                        : (DateTime?)null))
                .ForMember(dest => dest.UniquePropertyReferenceNumberUprn,
                    opt => opt.MapFrom(src => src.UniquePropertyReferenceNumber));

            CreateMap<MeasureDto, Measure>()
                .ForMember(dest => dest.PurposeOfNotification, opt => opt.Ignore())
                .ForMember(dest => dest.MeasureType, opt => opt.Ignore())
                .ForMember(dest => dest.EligibilityType, opt => opt.Ignore());

            CreateMap<Measure, MeasureHistory>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.ModifiedDate));

            CreateMap<MeasureType, MeasureTypeDto>()
                .ForMember(dest => dest.MeasureCategoryName, opt => opt.MapFrom(src => src.MeasureCategory!.Name));

            CreateMap<InnovationMeasure, InnovationMeasureDto>();

            CreateMap<FivePercentExtension, FivePercentExtensionQuotaDto>();
        }
    }
}
