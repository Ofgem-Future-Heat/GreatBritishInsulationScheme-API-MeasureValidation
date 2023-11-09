using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0610RuleLogicTests
    {
        private static readonly string mrnValue = "FOX0123456789";
        private readonly IEnumerable<MeasureTypeDto> _measureTypes = new List<MeasureTypeDto>
        {
            new MeasureTypeDto() { Name = "EWI_cavity_0.45_0.21", MeasureCategoryName = MeasureCategories.ExternalInternalWallInsulation},
            new MeasureTypeDto() { Name = "HWI_cavity_0.45_0.21", MeasureCategoryName = MeasureCategories.ExternalInternalWallInsulation},
            new MeasureTypeDto() { Name = "IWI_cavity_0.45_0.21", MeasureCategoryName = MeasureCategories.ExternalInternalWallInsulation},
        };

        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0610Rule_WithValidInput_PassValidation(string associatedInFillMeasure1, string associatedInFillMeasure2, string associatedInFillMeasure3, string property, string measureType)
        {
            var measureModel = new MeasureModel
            {
                EligibilityType = EligibilityTypes.InFill,
                MeasureReferenceNumber = mrnValue,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                AssociatedInfillMeasure2 = associatedInFillMeasure2,
                AssociatedInfillMeasure3 = associatedInFillMeasure3,
                Property = property,
                MeasureType = measureType,
                ReferenceDataDetails = new ReferenceDataDetails { MeasureTypesList = this._measureTypes }
            };
            var rule = new Gbis0610RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0610Rule_WithInvalidInput_FailsValidation(string associatedInFillMeasure1, string associatedInFillMeasure2, string associatedInFillMeasure3, string property, string measureType)
        {
            var measureModel = new MeasureModel
            {
                EligibilityType = EligibilityTypes.InFill,
                MeasureReferenceNumber = mrnValue,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                AssociatedInfillMeasure2 = associatedInFillMeasure2,
                AssociatedInfillMeasure3 = associatedInFillMeasure3,
                Property = property,
                MeasureType = measureType,
                ReferenceDataDetails = new ReferenceDataDetails { MeasureTypesList = this._measureTypes }
            };
            var rule = new Gbis0610RuleLogic();

            var result = rule.FailureCondition(measureModel);
           var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0610", rule.TestNumber);
            Assert.Equal(measureModel.MeasureType, observedValue);
        }

    public static IEnumerable<object[]> ValidInputArguments()
    {
        yield return new object[] { mrnValue, mrnValue, mrnValue, PropertyTypes.Flat, "EWI_cavity_0.45_0.21" };
        yield return new object[] { mrnValue, mrnValue, mrnValue, PropertyTypes.Maisonette, "HWI_cavity_0.45_0.21" };
        yield return new object[] { "N/A", mrnValue, mrnValue, PropertyTypes.Maisonette, "IWI_cavity_0.45_0.21" };
    }
        public static IEnumerable<object[]> InvalidInputArguments()
    {
        yield return new object[] { mrnValue, mrnValue, mrnValue, PropertyTypes.House, "CWI_0.027" };
        yield return new object[] { mrnValue, mrnValue, mrnValue, PropertyTypes.Bungalow, "CWI_0.027" };
        yield return new object[] { mrnValue, mrnValue, mrnValue, PropertyTypes.ParkHome, "CWI_0.027" };
    }
    }
}
