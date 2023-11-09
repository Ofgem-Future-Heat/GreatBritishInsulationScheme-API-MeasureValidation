using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0619RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(Gbis0619ValidInputArguments))]
        public void Gbis0619Rule_WithValidInput_PassValidation(string eligibilityType, string property, string associatedInFillMeasure1, DateTime associatedInFillMeasure1Doci, DateTime associatedInFillMeasure2Doci, DateTime associatedInFillMeasure3Doci)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                DateOfCompletedInstallation = "30/10/2023",
                AssociatedInfillMeasure1Details = new AssociatedMeasureModelDto
                {
                    DateOfCompletedInstallation = associatedInFillMeasure1Doci
                },
                AssociatedInfillMeasure2Details = new AssociatedMeasureModelDto
                {
                    DateOfCompletedInstallation = associatedInFillMeasure2Doci
                },
                AssociatedInfillMeasure3Details = new AssociatedMeasureModelDto
                {
                    DateOfCompletedInstallation = associatedInFillMeasure3Doci
                }
            };

            var rule = new Gbis0619RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        public static IEnumerable<object[]> Gbis0619ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", new DateTime(2023, 09, 01), new DateTime(2023, 10, 01), new DateTime(2023, 08, 01) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, "GBT0123456789", new DateTime(2023, 10, 01), new DateTime(2023, 09, 01), new DateTime(2023, 09, 01) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.ParkHome, "GBT0123456789", new DateTime(2023, 07, 21), new DateTime(2023, 09, 01), new DateTime(2023, 09, 01) };
        }

        [Theory]
        [MemberData(nameof(Gbis0619InvalidInputArguments))]
        public void Gbis0619Rule_WithInvalidInput_FailsValidation(string eligibilityType, string property, string associatedInFillMeasure1, DateTime associatedInFillMeasure1Doci, DateTime associatedInFillMeasure2Doci, DateTime associatedInFillMeasure3Doci)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                DateOfCompletedInstallation = "30/10/2023",

                AssociatedInfillMeasure1Details = new AssociatedMeasureModelDto
                {
                    DateOfCompletedInstallation = associatedInFillMeasure1Doci
                },
                AssociatedInfillMeasure2Details = new AssociatedMeasureModelDto
                {
                    DateOfCompletedInstallation = associatedInFillMeasure2Doci
                },
                AssociatedInfillMeasure3Details = new AssociatedMeasureModelDto
                {
                    DateOfCompletedInstallation = associatedInFillMeasure3Doci
                }
            };

            var rule = new Gbis0619RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0619", rule.TestNumber);
        }

        public static IEnumerable<object[]> Gbis0619InvalidInputArguments()
        {
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", new DateTime(2025, 03, 04), new DateTime(2024, 04, 04), new DateTime(2028, 12, 31) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Bungalow, "GBT0123456789", new DateTime(2024, 04, 04), new DateTime(2024, 01, 31), new DateTime(2024, 01, 30) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.ParkHome, "GBT0123456789", new DateTime(2024, 01, 31), new DateTime(2024, 01, 30), new DateTime(2024, 02, 02) };
        }
    }
}
