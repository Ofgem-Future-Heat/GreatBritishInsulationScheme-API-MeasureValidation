using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;


namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0208RuleLogicTests
    {
        private readonly Mock<IMeasureTypeToCategoryService> _mockMeasureTypeToCategoryService = new();
        public static TheoryData<string, string, string, string> PassingMeasures =>
            new()
            {
                {"MRN0123456781", "General Group", "Private Rented Sector", "some measure category" },
                {"MRN0123456781", "general group", "private rented sector", "some measure category" },
                {"MRN0123456781", "General Group", "some tenure type", "CWI_categpry" },
                {"MRN0123456781", "Some Group", "Private Rented Sector", "CWI_categpry" }
            };

        [Theory]
        [MemberData(nameof(PassingMeasures))]
        public void Gbis0208_GivenPassingMeasures_ReturnsNoError(string mrn, string eligibilityType, string tenureType, string measureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = mrn,
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                MeasureType = measureType
            };
            var rule = new Gbis0208RuleLogic(_mockMeasureTypeToCategoryService.Object);
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }


        public static TheoryData<string, string, string, string> FailingMeasures =>
            new()
            {
                {"MRN0123456782", "general group", "private rented sector", "CWI_theRestIsUnimportant" },
                {"MRN0123456782", "General Group", "Private Rented Sector", "CWI_theRestIsUnimportant" },
                {"MRN0123456782", "General Group", "Private Rented Sector", "LI_theRestIsUnimportant" }
            };

        [Theory]
        [MemberData(nameof(FailingMeasures))]
        public void Gbis0208_GivenFailingMeasures_ReturnsError(string mrn, string eligibilityType, string tenureType, string measureType)
        {
            var measureModel = new MeasureModel()
            {
                MeasureReferenceNumber = mrn,
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                MeasureType = measureType
            };

            var cavityWallInsulation = "Cavity Wall Insulation";
            _mockMeasureTypeToCategoryService.Setup(m => m.GetMeasureCategoryByType(It.IsAny<string>()))
                .Returns(Task.FromResult(cavityWallInsulation));
            var rule = new Gbis0208RuleLogic(_mockMeasureTypeToCategoryService.Object);
            var result = rule.FailureCondition(measureModel);
            Assert.True(result);
        }
    }
}
