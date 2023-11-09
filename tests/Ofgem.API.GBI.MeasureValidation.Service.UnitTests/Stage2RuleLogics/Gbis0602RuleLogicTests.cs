using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0602RuleLogicTests
    {
        private const string AssociatedInFillMeasure1TestValue = "Test123";
        private const string AssociatedInFillMeasure2TestValue = "Test456";
        private const string AssociatedInFillMeasure3TestValue = "Test789";

        private const string AssocInFillStreetName = "Hardwicke Rd";
        private const string AssocInFillTown = "Brighton";

        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0602RuleLogic_AssociatedInfillMeasure1DetailsWithValidInput_PassesValidation(string eligibilityType, string property, string? streetName, string? town)
        {
            // Arrange
            var measureModel = CreateMockMeasureModel(eligibilityType, property, streetName, town);

            var rule = new Gbis0602RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.Equal($"{AssociatedInFillMeasure1TestValue} | {AssociatedInFillMeasure2TestValue} | {AssociatedInFillMeasure3TestValue}", actual);
        }


        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0602RuleLogic_WithInvalidInput_FailsValidation(string eligibilityType, string property, string? streetName, string? town)
        {
            var measureModel = CreateMockMeasureModel(eligibilityType, property, streetName, town);

            var rule = new Gbis0602RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.True(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.Equal($"{AssociatedInFillMeasure1TestValue} | {AssociatedInFillMeasure2TestValue} | {AssociatedInFillMeasure3TestValue}", actual);
        }

        private MeasureModel CreateMockMeasureModel(string eligibilityType, string property, string? streetName, string? town)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = AssociatedInFillMeasure1TestValue,
                AssociatedInfillMeasure2 = AssociatedInFillMeasure2TestValue,
                AssociatedInfillMeasure3 = AssociatedInFillMeasure3TestValue,
                StreetName = streetName,
                Town = town,
                AssociatedInfillMeasure1Details = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        StreetName = AssocInFillStreetName,
                        Town = AssocInFillTown,
                    }
                },
                AssociatedInfillMeasure2Details = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        StreetName = AssocInFillStreetName,
                        Town = AssocInFillTown,
                    }
                },
                AssociatedInfillMeasure3Details = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        StreetName = AssocInFillStreetName,
                        Town = AssocInFillTown,
                    }
                }
            };
            return measureModel;
        }


        public static IEnumerable<object?[]> InvalidInputArguments()
        {

            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.House, AssocInFillStreetName, null };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.House, null, AssocInFillTown };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.House, null, null };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.Bungalow , AssocInFillStreetName, null };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, null, AssocInFillTown };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, null, null };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.ParkHome, AssocInFillStreetName, null };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.ParkHome, null, AssocInFillTown };
            yield return new object?[] { EligibilityTypes.InFill, PropertyTypes.ParkHome, null, null };
        }

        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.House, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.House, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.ParkHome, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.ParkHome, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.GeneralGroup, PropertyTypes.ParkHome, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.GeneralGroup, PropertyTypes.House, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.GeneralGroup, PropertyTypes.Bungalow, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.GeneralGroup, PropertyTypes.Flat, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.GeneralGroup, PropertyTypes.Maisonette, AssocInFillStreetName, AssocInFillTown };
        }
    }
}
