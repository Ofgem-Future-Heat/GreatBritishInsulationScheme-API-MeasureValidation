using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0601RuleLogicTests
    {
        private const string AssociatedInFillMeasureTestValue = "Test123";

        private const string AssocInFillBuildingName = "ABC House";
        private const string AssocInFillStreetName = "Hardwicke Rd";
        private const string AssocInFillTown = "Brighton";
        private const string AssocInFillPostCode = "ABC DEC";

        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0601RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string property, string associatedInFillMeasure1, string? postCode, string? buildingName, string? streetName, string? town)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                PostCode = postCode,
                BuildingName = buildingName,
                StreetName = streetName,
                Town = town,
                AssociatedInfillMeasure1Details = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        BuildingName = AssocInFillBuildingName,
                        StreetName = AssocInFillStreetName,
                        Town = AssocInFillTown,
                        PostCode = AssocInFillPostCode
                    }
                }
            };

            var rule = new Gbis0601RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(AssociatedInFillMeasureTestValue));
        }

        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0601RuleLogic_WithInvalidInput_FailsValidation(string eligibilityType, string property, string associatedInFillMeasure1, string? postCode, string? buildingName, string? streetName, string? town, bool hasAssocInFillAddress = false)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "",
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                PostCode = postCode,
                BuildingName = buildingName,
                StreetName = streetName,
                Town = town,
                AssociatedInfillMeasure1Details = new AssociatedMeasureModelDto()
                {
                    Address = new AddressDto()
                    {
                        BuildingName = hasAssocInFillAddress?AssocInFillBuildingName:null,
                        StreetName = hasAssocInFillAddress ? AssocInFillStreetName : null,
                        Town = hasAssocInFillAddress ? AssocInFillTown : null,
                        PostCode = hasAssocInFillAddress ? AssocInFillPostCode : null
                    }
                }
            };

            var rule = new Gbis0601RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.True(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(measureModel.AssociatedInfillMeasure1));
        }


        public static IEnumerable<object[]> InvalidInputArguments()
        {
           
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, "ZZ1 YY2", AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, "Building XYZ", AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, "OPN Street", AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, "KLM Town" };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, "ZZ1 YY2", AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, AssocInFillPostCode, "Building XYZ", AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, "OPN Street", AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, "KLM Town" };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, "ZZ1 YY2", AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown,true };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, "Building XYZ", AssocInFillStreetName, AssocInFillTown, true };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, "OPN Street", AssocInFillTown, true };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, "KLM Town", true };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, "ZZ1 YY2", AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown, true };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, AssocInFillPostCode, "Building XYZ", AssocInFillStreetName, AssocInFillTown, true };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, "OPN Street", AssocInFillTown, true };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, "KLM Town", true };
        }

        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, AssociatedInFillMeasureTestValue,AssocInFillPostCode,AssocInFillBuildingName,AssocInFillStreetName,AssocInFillTown };
            yield return new object[] { EligibilityTypes.GeneralGroup, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.LILADeclaration, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.LILADeclaration, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, PropertyTypes.Flat, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.House, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, AssociatedInFillMeasureTestValue, AssocInFillPostCode, AssocInFillBuildingName, AssocInFillStreetName, AssocInFillTown };
        }
    }
}
