using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0618RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(Gbis0618ValidInputArguments))]
        public void Gbis0618Rule_WithValidInput_PassValidation(string eligibilityType, string property, string associatedInFillMeasure1, DateTime associatedInFillMeasure1Doci)
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
                }
            };

            var rule = new Gbis0618RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        public static IEnumerable<object[]> Gbis0618ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, "GBT0123456789", new DateTime(2023, 07, 30) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, "GBT0123456789", new DateTime(2023, 09, 01) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, "GBT0123456789", new DateTime(2023, 10, 01) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, "GBT0123456789", new DateTime(2023, 10, 30) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, "N/A", new DateTime(2023, 10, 31) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", new DateTime(2023, 10, 31) };
        }

        [Theory]
        [MemberData(nameof(Gbis0618InvalidInputArguments))]
        public void Gbis0618Rule_WithInvalidInput_FailsValidation(string eligibilityType, string property, string associatedInFillMeasure1, DateTime associatedInFillMeasure1Doci)
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
                }
            };

            var rule = new Gbis0618RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0618", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure1, observedValue);
        }

        public static IEnumerable<object[]> Gbis0618InvalidInputArguments()
        {
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, "GBT0123456789", new DateTime(2028, 12, 31) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, "GBT0123456789", new DateTime(2025, 03, 04) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Flat, "GBT0123456789", new DateTime(2024, 04, 04) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, "GBT0123456789", new DateTime(2023, 07, 29) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, "GBT0123456789", new DateTime(2023, 10, 31) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, "GBT0123456789", new DateTime(2023, 11, 01) };
            yield return new object[] { EligibilityTypes.InFill, PropertyTypes.Maisonette, "GBT0123456789", new DateTime(2021, 01, 15) };
        }
    }
}
