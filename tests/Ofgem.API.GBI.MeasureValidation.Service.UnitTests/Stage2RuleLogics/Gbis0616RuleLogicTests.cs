using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0616RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, "EON9899001595", CommonTypesConstants.NotApplicable, "BGT03078711E", "BGT03078711E")]
        [InlineData(EligibilityTypes.InFill, "EON9899001595", CommonTypesConstants.NotApplicable, "BGT03078711E", "BGT03078711E")]
        [InlineData(EligibilityTypes.InFill, "BGT9899001598", "BGT9899001500", "BGT03078711E", "BGT03078711E")]
        [InlineData(EligibilityTypes.InFill, "EON9899001595", "EON9899001000", "EON03078711E", "EON03078711E")]
        [InlineData(EligibilityTypes.InFill, "BGT9899001603", "BGT9899001598", "BGT03078711E", "BGT03078711E")]
        public void Gbis0616Rule_WithValidInput_PassesValidation(string eligibilityType, string measureReferenceNumber, string associatedInfillMeasure2, string supplierReference, string associatedSupplierReference)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = measureReferenceNumber,
                EligibilityType = eligibilityType,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
                SupplierReference = supplierReference,
                AssociatedInfillMeasure2Details = new AssociatedMeasureModelDto
                {
                    MeasureReferenceNumber = associatedInfillMeasure2,
                    SupplierReference = associatedSupplierReference
                }
            };

            var rule = new Gbis0616RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "BGT9899001596", "EON9899001595", "EON03078711E", "BGT03078711E")]
        [InlineData(EligibilityTypes.InFill, "EON9899001595", "EON9899001595", "EON03078711E", "BGT03078711E")]
        [InlineData(EligibilityTypes.InFill, "BGT9899001603", "BGT9899001598", "BGT03078711E", "BGT01234567E")]
        [InlineData(EligibilityTypes.InFill, "BGT9899001603", "BGT9899001598", "BGT03078711E", "ABC03078711E")]
        public void Gbis0616Rule_WithInvalidInput_FailsValidation(string eligibilityType, string measureReferenceNumber, string associatedInfillMeasure2, string supplierReference, string associatedSupplierReference)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = measureReferenceNumber,
                EligibilityType = eligibilityType,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
                SupplierReference = supplierReference,
                AssociatedInfillMeasure2Details = new AssociatedMeasureModelDto
                {
                    MeasureReferenceNumber = associatedInfillMeasure2,
                    SupplierReference = associatedSupplierReference
                }
            };

            var rule = new Gbis0616RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0616", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure2, observedValue);
        }
    }
}
