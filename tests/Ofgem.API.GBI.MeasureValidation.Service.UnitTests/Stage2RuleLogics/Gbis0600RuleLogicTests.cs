using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0600RuleLogicTests
    {
        private const string AssociatedInFillMeasureTestValue = "Test123";

		[Theory]
		[InlineData(EligibilityTypes.InFill,CommonTypesConstants.NotApplicable,AssociatedInFillMeasureTestValue,CommonTypesConstants.NotApplicable)]
		[InlineData(EligibilityTypes.InFill, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable,AssociatedInFillMeasureTestValue)]
        public void Gbis0600RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string associatedInFillMeasure1, string associatedInFillMeasure2, string associatedInFillMeasure3)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
				AssociatedInfillMeasure1 = associatedInFillMeasure1,
                AssociatedInfillMeasure2 = associatedInFillMeasure2,
                AssociatedInfillMeasure3 = associatedInFillMeasure3,
            };

			var rule = new Gbis0600RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(string.Empty));
        }

		[Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0600RuleLogic_WithInvalidInput_FailsValidation(string eligibilityType, string associatedInFillMeasure1, string associatedInFillMeasure2, string associatedInFillMeasure3, string expected)
        {
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                AssociatedInfillMeasure2 = associatedInFillMeasure2,
                AssociatedInfillMeasure3 = associatedInFillMeasure3,
            };

			var rule = new Gbis0600RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.True(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(expected));
		}

        public static IEnumerable<object[]> InvalidInputArguments()
        {
            yield return new object[] { EligibilityTypes.LISupplierEvidence, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable
                , $"{CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.GeneralGroup, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable
                , $"{CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable
                , $"{CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LILADeclaration, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable
                , $"{CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LISocialHousing, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable
                , $"{CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable
                , $"{AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.GeneralGroup, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable
                , $"{AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable
                , $"{AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LILADeclaration, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable
                , $"{AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LISocialHousing, AssociatedInFillMeasureTestValue, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable
                , $"{AssociatedInFillMeasureTestValue} | {CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable}" };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue
                , $"{CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue}" };
            yield return new object[] { EligibilityTypes.GeneralGroup, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue
                , $"{CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue}" };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue
                , $"{CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue}" };
            yield return new object[] { EligibilityTypes.LILADeclaration, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue
                , $"{CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue}" };
            yield return new object[] { EligibilityTypes.LISocialHousing, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, AssociatedInFillMeasureTestValue
                , $"{CommonTypesConstants.NotApplicable} | {CommonTypesConstants.NotApplicable} | {AssociatedInFillMeasureTestValue}" };
        }
    }
}
