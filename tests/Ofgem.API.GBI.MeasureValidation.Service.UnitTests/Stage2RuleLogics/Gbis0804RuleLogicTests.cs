using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0804RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidHeatingMeasures))]
        public void Gbis0804Rule_WithValidInput_PassValidation(string measureType, string assoicatedInsulationMrn, string doci, AssociatedMeasureModelDto associatedMeasure)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                AssociatedInsulationMrnForHeatingMeasures = assoicatedInsulationMrn,
                DateOfCompletedInstallation = doci,
                AssociatedInsulationMeasureForHeatingMeasureDetails = associatedMeasure
            };
            var rule = new Gbis0804RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        public static TheoryData<string, string, string?, AssociatedMeasureModelDto?> ValidHeatingMeasures()
        {
            var validHeatingMeasures = new TheoryData<string, string, string?, AssociatedMeasureModelDto?>
            {
                // doci same day as associated doci
                { MeasureTypes.Trv, "FOX0123456789", "23/10/2023",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified)} },

                // doci between associated doci up to + 3 months after
                { MeasureTypes.PAndRt, "FOX0123456789", "24/10/2023",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified)} },
                { MeasureTypes.Trv, "FOX0123456789", "02/02/2023",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 01, 02, 0, 0, 0, DateTimeKind.Unspecified)} },
                { MeasureTypes.PAndRt, "FOX0123456789", "01/04/2023",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 01, 02, 0, 0, 0, DateTimeKind.Unspecified)} },

                // doci on last day ( + 3 months after assoicated doci )
                { MeasureTypes.Trv, "FOX0123456789", "02/04/2023",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 01, 02, 0, 0, 0, DateTimeKind.Unspecified)} },

                // other
                { "other", "FOX0123456789", "02/02/2024",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 01, 02, 0, 0, 0, DateTimeKind.Unspecified)} },
                { MeasureTypes.Trv, CommonTypesConstants.NotApplicable, "02/02/2024",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 01, 02, 0, 0, 0, DateTimeKind.Unspecified)} },
                { MeasureTypes.Trv, "FOX0123456789", "23 Oct 2024",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified)} },
                { MeasureTypes.Trv, "FOX0123456789", "23.10.2021",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified)} },
                { MeasureTypes.Trv, "FOX0123456789", null,
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified)} },
                { MeasureTypes.Trv, "FOX0123456789", "23/10/2023",
                    new AssociatedMeasureModelDto() { DateOfCompletedInstallation = null }}
            };
            return validHeatingMeasures;
        }

        [Theory]
        [MemberData(nameof(InvalidHeatingMeasures))]
        public void Gbis0804Rule_WithInvalidInput_FailsValidation(string measureType, string assoicatedInsulationMrn, string doci, AssociatedMeasureModelDto associatedMeasure)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                AssociatedInsulationMrnForHeatingMeasures = assoicatedInsulationMrn,
                DateOfCompletedInstallation = doci,
                AssociatedInsulationMeasureForHeatingMeasureDetails = associatedMeasure
            };
            var rule = new Gbis0804RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0804", rule.TestNumber);
            Assert.Equal(measureModel.DateOfCompletedInstallation, observedValue);
        }

        public static IEnumerable<object[]> InvalidHeatingMeasures
        {
            get
            {
                // doci before associated doci
                yield return new object[]
                {
                    MeasureTypes.Trv, "FOX0123456789", "22/10/2023",
                    new AssociatedMeasureModelDto()
                        { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified) }
                };
                yield return new object[]
                {
                    MeasureTypes.PAndRt, "FOX0123456789", "21/10/2023",
                    new AssociatedMeasureModelDto()
                        { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified) }
                };
                yield return new object[]
                {
                    MeasureTypes.Trv, "FOX0123456789", "01/01/2020",
                    new AssociatedMeasureModelDto()
                        { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified) }
                };
                yield return new object[]
                {
                    MeasureTypes.PAndRt, "FOX0123456789", "01/01/1987",
                    new AssociatedMeasureModelDto()
                        { DateOfCompletedInstallation = new DateTime(2023, 10, 23, 0, 0, 0, DateTimeKind.Unspecified) }
                };

                // doci after associated doci
                yield return new object[]
                {
                    MeasureTypes.Trv, "FOX0123456789", "02/04/2024",
                    new AssociatedMeasureModelDto()
                        { DateOfCompletedInstallation = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Unspecified) }
                };
                yield return new object[]
                {
                    MeasureTypes.PAndRt, "FOX0123456789", "03/04/2024",
                    new AssociatedMeasureModelDto()
                        { DateOfCompletedInstallation = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Unspecified) }
                };
                yield return new object[]
                {
                    MeasureTypes.Trv, "FOX0123456789", "02/04/2036",
                    new AssociatedMeasureModelDto()
                        { DateOfCompletedInstallation = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Unspecified) }
                };
            }
        }

    }
}