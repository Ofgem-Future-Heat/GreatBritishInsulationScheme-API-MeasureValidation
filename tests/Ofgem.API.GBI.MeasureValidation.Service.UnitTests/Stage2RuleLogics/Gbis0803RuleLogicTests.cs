using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0803RuleLogicTests
    {
        [Theory]
        [InlineData("BGT1236881G", MeasureTypes.Trv, "BGT1236881G", "BGT1236881G")]
        [InlineData("BGT1236882G", MeasureTypes.PAndRt, "BGT1236882G", "BGT1236882G")]
        [InlineData("BGT1236883G", MeasureTypes.PAndRt, "bgt1236883G", "BGT1236883G")]
        public void Gbis0803Rule_WithValidInput_PassValidation(string associatedInsulationMrnForHeatingType, string measureType, string supplierReference, string associatedSupplierReference)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT0123456789",
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingType,
                MeasureType = measureType,
                SupplierReference = supplierReference,
				AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto
                {
                    SupplierReference = associatedSupplierReference
                }
            };

            var rule = new Gbis0803RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("BGT1236883G", MeasureTypes.PAndRt, "BGT1236881G", "BGT1236883G")]
        [InlineData(null, MeasureTypes.PAndRt, "BGT1236882G", null)]
        [InlineData("BGT1236883G", MeasureTypes.Trv, "BGT1236884G", "BGT1236883G")]
        [InlineData("BGT1236883G", MeasureTypes.Trv, null, "BGT1236883G")]
        [InlineData("BGT1236883G", MeasureTypes.Trv, CommonTypesConstants.NotApplicable, "BGT1236883G")]
        public void Gbis0803Rule_WithInvalidInput_FailsValidation(string associatedInsulationMrnForHeatingType, string measureType, string supplierReference, string associatedSupplierReference)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT0123456789",
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingType,
                MeasureType = measureType,
                SupplierReference = supplierReference,
				AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto
                {
                    SupplierReference = associatedSupplierReference
                }
            };

            var rule = new Gbis0803RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0803", rule.TestNumber);
        }
    }
}
