using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using System.Text.RegularExpressions;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0904RuleLogicTests
    {
        [Theory]
        [InlineData("A12345678-12345")]
        [InlineData("Z99999999-99999")]
        [InlineData(CommonTypesConstants.NotApplicable)]
        public void Gbis0904_GivenValidLaDecRef_ReturnsNoError(string laDeclarationReference)
        {
            var measureModel = new MeasureModel()
            {
                LaDeclarationReferenceNumber = laDeclarationReference
            };

            var rule = new Gbis0904RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        [Theory]
        [InlineData("112345678-12345")]
        [InlineData("AA2345678-12345")]
        [InlineData("112345678-1234")]
        [InlineData("A11234567812345")]
        public void Gbis0903_GivenInvalidDecRef_ReturnsError(string laDeclarationReference)
        {
            var measureModel = new MeasureModel()
            {
                LaDeclarationReferenceNumber = laDeclarationReference
            };

            var rule = new Gbis0904RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.True(result);
        }
    }
}
