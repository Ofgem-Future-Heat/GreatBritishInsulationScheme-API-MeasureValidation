using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0809RuleLogicTests
    {
        private const string AssociatedInsulationMrnForHeatingMeasureTestValue = "Test123";
        private const string AssociatedInsulationMrnForHeatingFlatNameNumber = "Flat 1A";
        private const string AssociatedInsulationMrnForHeatingStreetName = "Hardwicke Rd";
        private const string AssociatedInsulationMrnForHeatingTown = "Brighton";
        private const string AssociatedInsulationMrnForHeatingPostCode = "ABC DEC";
        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0809RuleLogic_WithValidInput_PassesValidation(string measureType, string property, string associatedInsulationMrn, string? postCode, string? flatNameNumber, string? streetName, string? town)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                Property = property,
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrn,
                PostCode = postCode,
                FlatNameNumber = flatNameNumber,
                StreetName = streetName,
                Town = town,
                AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        FlatNameNumber = AssociatedInsulationMrnForHeatingFlatNameNumber,
                        StreetName = AssociatedInsulationMrnForHeatingStreetName,
                        Town = AssociatedInsulationMrnForHeatingTown,
                        PostCode = AssociatedInsulationMrnForHeatingPostCode
                    }
                }
            };
            var rule = new Gbis0809RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(AssociatedInsulationMrnForHeatingMeasureTestValue));
        }
        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0809RuleLogic_WithInvalidInput_FailsValidation(string measureType, string property, string associatedInsulationMrn, string? postCode, string? flatNameNumber, string? streetName, string? town, bool hasAssocInFillAddress = false)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                Property = property,
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrn,
                PostCode = postCode,
                FlatNameNumber = flatNameNumber,
                StreetName = streetName,
                Town = town,
                AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        FlatNameNumber = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingFlatNameNumber : null,
                        StreetName = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingStreetName : null,
                        Town = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingTown : null,
                        PostCode = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingPostCode : null
                    }
                }
            };
            var rule = new Gbis0809RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0809", rule.TestNumber);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(measureModel.AssociatedInsulationMrnForHeatingMeasures));
        }
        public static IEnumerable<object[]> InvalidInputArguments()
        {
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Flat 2B", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town" };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Flat 2B", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town" };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Flat 2B", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town", true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Flat 2B", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town", true };
        }
        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingFlatNameNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
        }
    }
}