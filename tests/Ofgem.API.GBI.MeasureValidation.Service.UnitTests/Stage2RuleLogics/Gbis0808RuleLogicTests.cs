using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0808RuleLogicTests
    {
        private const string AssociatedInsulationMrnForHeatingMeasureTestValue = "Test123";

        private const string AssociatedInsulationMrnForHeatingBuildingName = "ABC House";
        private const string AssociatedInsulationMrnForHeatingBuildingNumber = "123";
        private const string AssociatedInsulationMrnForHeatingStreetName = "Hardwicke Rd";
        private const string AssociatedInsulationMrnForHeatingTown = "Brighton";
        private const string AssociatedInsulationMrnForHeatingPostCode = "ABC DEC";

        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0808RuleLogic_WithValidInput_PassesValidation(string measureType, string property, string associatedInsulationMrn, string? postCode, string? buildingName, string? buildingNumber, string? streetName, string? town)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                Property = property,
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrn,
                PostCode = postCode,
                BuildingName = buildingName,
                BuildingNumber = buildingNumber,
                StreetName = streetName,
                Town = town,
                AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        BuildingName = AssociatedInsulationMrnForHeatingBuildingName,
                        BuildingNumber = AssociatedInsulationMrnForHeatingBuildingNumber,
                        StreetName = AssociatedInsulationMrnForHeatingStreetName,
                        Town = AssociatedInsulationMrnForHeatingTown,
                        PostCode = AssociatedInsulationMrnForHeatingPostCode
                    }
                }
            };

            var rule = new Gbis0808RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(AssociatedInsulationMrnForHeatingMeasureTestValue));
        }

        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0808RuleLogic_WithInvalidInput_FailsValidation(string measureType, string property, string associatedInsulationMrn, string? postCode, string? buildingName, string? buildingNumber, string? streetName, string? town, bool hasAssocInFillAddress = false)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                Property = property,
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrn,
                PostCode = postCode,
                BuildingName = buildingName,
                BuildingNumber = buildingNumber,
                StreetName = streetName,
                Town = town,
                AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        BuildingName = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingBuildingName : null,
                        BuildingNumber = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingBuildingNumber : null,
                        StreetName = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingStreetName : null,
                        Town = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingTown : null,
                        PostCode = hasAssocInFillAddress ? AssociatedInsulationMrnForHeatingPostCode : null
                    }
                }
            };

            var rule = new Gbis0808RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0808", rule.TestNumber);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(measureModel.AssociatedInsulationMrnForHeatingMeasures));
        }


        public static IEnumerable<object[]> InvalidInputArguments()
        {

            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Building XYZ", AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, "789", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town" };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Building XYZ", AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, "789", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town" };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Building XYZ", AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, "789", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town" };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Building XYZ", AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, "789", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town", true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Building XYZ", AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, "789", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town", true };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, "ZZ1 YY2", AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, "Building XYZ", AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, "789", AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, "OPN Street", AssociatedInsulationMrnForHeatingTown, true };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, "KLM Town", true };
        }

        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.House, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Bungalow, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.ParkHome, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.PAndRt, PropertyTypes.Maisonette, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
            yield return new object[] { MeasureTypes.Trv, PropertyTypes.Flat, AssociatedInsulationMrnForHeatingMeasureTestValue, AssociatedInsulationMrnForHeatingPostCode, AssociatedInsulationMrnForHeatingBuildingName, AssociatedInsulationMrnForHeatingBuildingNumber, AssociatedInsulationMrnForHeatingStreetName, AssociatedInsulationMrnForHeatingTown };
        }
    }
}
