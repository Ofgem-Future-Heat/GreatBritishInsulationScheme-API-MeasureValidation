using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.Database.GBI.Measures.Domain.Entities;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0903RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.LILADeclaration, "A123456789-12345")]
        [InlineData( EligibilityTypes.GeneralGroup, CommonTypesConstants.NotApplicable)]

        public void Gbis0903_GivenValidLaDeclarationReference_ReturnsNoError(string eligibilityType, string laDeclarationReference)
        {
            var measureModel = new MeasureModel()
            {
                EligibilityType = eligibilityType,
                LaDeclarationReferenceNumber = laDeclarationReference
            };

            var rule = new Gbis0903RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.LILADeclaration, CommonTypesConstants.NotApplicable)]
        public void Gbis0903_GivenInvalidDecRef_ReturnsError(string eligibilityType, string laDeclarationReference)
        {
            var measureModel = new MeasureModel()
            {
                EligibilityType = eligibilityType,
                LaDeclarationReferenceNumber = laDeclarationReference
            };

            var rule = new Gbis0903RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.True(result);
        }
    }
}
