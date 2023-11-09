using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0609RuleLogicTests
    {

        private readonly IEnumerable<MeasureTypeDto> _validMeasureTypes = new List<MeasureTypeDto>
        {
            new MeasureTypeDto() { Name = "CWI_0.027", MeasureCategoryName = MeasureCategories.CavityWallInsulation },
            new MeasureTypeDto() { Name = "CWI_0.033", MeasureCategoryName = MeasureCategories.CavityWallInsulation},
            new MeasureTypeDto() { Name = "CWI_partial_fill", MeasureCategoryName = MeasureCategories.CavityWallInsulation},
            new MeasureTypeDto() { Name = "PWI_Cavity", MeasureCategoryName = MeasureCategories.CavityWallInsulation},
            new MeasureTypeDto() { Name = "EWI_cavity_0.45_0.21", MeasureCategoryName = MeasureCategories.ExternalInternalWallInsulation},
            new MeasureTypeDto() { Name = "HWI_cavity_0.45_0.21", MeasureCategoryName = MeasureCategories.ExternalInternalWallInsulation},
            new MeasureTypeDto() { Name = "IWI_cavity_0.45_0.21", MeasureCategoryName = MeasureCategories.ExternalInternalWallInsulation },
            new MeasureTypeDto() { Name = "IWI_solid_2.0_0.6", MeasureCategoryName = MeasureCategories.ExternalInternalWallInsulation },
        };

        [Theory]
        [InlineData("FOX0123456789", "Flat", "CWI_0.027")]
        [InlineData("FOX0123456789", "Flat", "cwi_0.027")]
        [InlineData("FOX0123456789", "Flat", "CWI_0.033")]
        [InlineData("FOX0123456789", "Flat", "CWI_partial_fill")]
        [InlineData("FOX0123456789", "Flat", "PWI_Cavity")]
        [InlineData("FOX0123456789", "Flat", "pwi_cavity")]
        [InlineData("FOX0123456789", "Maisonette", "EWI_cavity_0.45_0.21")]
        [InlineData("FOX0123456789", "Maisonette", "HWI_cavity_0.45_0.21")]
        [InlineData("FOX0123456789", "Maisonette", "IWI_cavity_0.45_0.21")]
        [InlineData("FOX0123456789", "Maisonette", "IWI_solid_2.0_0.6")]
        [InlineData("FOX0123456789", "Maisonette", "iwi_SOLID_2.0_0.6")]
        [InlineData("FOX0123456789", "House", "CWI_0.027")]
        [InlineData("FOX0123456789", "House", " not in measure types list ")]
        [InlineData("FOX0123456789", "Park Home", "CWI_0.027")]
        [InlineData("FOX0123456789", "Park Home", " not in measure types list ")]
        [InlineData("N/A", "Flat", "FRI")]
        [InlineData("N/A", "Flat", "CWI_0.027")]
        [InlineData("N/A", "Maisonette", "TRV")]
        [InlineData("N/A", "Maisonette", "CWI_0.027")]
        public void Gbis0609Rule_WithValidInput_PassValidation(string associatedInFillMeasure1, string property, string measureCategory)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0012345678",
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                Property = property,
                MeasureType = measureCategory,
                ReferenceDataDetails = new ReferenceDataDetails { MeasureTypesList = this._validMeasureTypes }
            };
            var rule = new Gbis0609RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("FOX0123456789", "Flat", "trv")]
        [InlineData("FOX0123456789", "Flat", "TRV")]
        [InlineData("FOX0123456789", "Flat", "P&RT")]
        [InlineData("FOX0123456789", "Maisonette", "LI_greater100")]
        [InlineData("FOX0123456789", "Maisonette", "li_greater100")]
        [InlineData("FOX0123456789", "Maisonette", "TRV")]
        [InlineData("FOX0123456789", "Flat", "SWI")]
        [InlineData("FOX0123456789", "Flat", "SWI_")]
        [InlineData("FOX0123456789", "Maisonette", "Cwi")]
        [InlineData("FOX0123456789", "Maisonette", "CWI_")]
        [InlineData("FOX0123456789", "Flat", "CWI_0.000000")]
        [InlineData("FOX0123456789", "Flat", "CWI_000000")]
        [InlineData("FOX0123456789", "Maisonette", "PWI_Cavity_100000")]
        [InlineData("FOX0123456789", "Maisonette", "EWI_cavity_0.45_0.200000")]
        public void Gbis0609Rule_WithInvalidInput_FailsValidation(string associatedInFillMeasure1, string property, string measureCategory)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                Property = property,
                MeasureType = measureCategory,
                ReferenceDataDetails = new ReferenceDataDetails { MeasureTypesList = this._validMeasureTypes }
            };
            var rule = new Gbis0609RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0609", rule.TestNumber);
            Assert.Equal(measureModel.MeasureType, observedValue);
        }

    }
}