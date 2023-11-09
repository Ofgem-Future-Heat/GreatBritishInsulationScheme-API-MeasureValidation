using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0715RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidInputArguments))]
        public void Gbis0715Rule_WithValidInput_PassValidation(string eligibilityType, string councilTaxBand)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                CouncilTaxBand = councilTaxBand
            };
            var rule = new Gbis0715RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(InvalidInputArguments))]
        public void Gbis0715Rule_WithInvalidInput_FailsValidation(string eligibilityType, string councilTaxBand)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                CouncilTaxBand = councilTaxBand
            };
            var rule = new Gbis0715RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0715", rule.TestNumber);
            Assert.Equal(measureModel.CouncilTaxBand, observedValue);
        }

        public static IEnumerable<object[]> ValidInputArguments()
        {
            yield return new object[] { EligibilityTypes.GeneralGroup, CommonTypesConstants.NotApplicable };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CouncilTaxBand.A };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CouncilTaxBand.B };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CouncilTaxBand.C };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CouncilTaxBand.D };
            yield return new object[] { EligibilityTypes.GeneralGroup, ReferenceDataConstants.CouncilTaxBand.E };
        }

        public static IEnumerable<object[]> InvalidInputArguments()
        {
            yield return new object[] { EligibilityTypes.LILADeclaration, ReferenceDataConstants.CouncilTaxBand.A };
            yield return new object[] { EligibilityTypes.LILADeclaration, ReferenceDataConstants.CouncilTaxBand.B };
            yield return new object[] { EligibilityTypes.LISocialHousing, ReferenceDataConstants.CouncilTaxBand.C };
            yield return new object[] { EligibilityTypes.LISocialHousing, ReferenceDataConstants.CouncilTaxBand.D };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, ReferenceDataConstants.CouncilTaxBand.E };
            yield return new object[] { EligibilityTypes.LISupplierEvidence, ReferenceDataConstants.CouncilTaxBand.A };
            yield return new object[] { EligibilityTypes.InFill, ReferenceDataConstants.CouncilTaxBand.B };
            yield return new object[] { EligibilityTypes.InFill, ReferenceDataConstants.CouncilTaxBand.C };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, ReferenceDataConstants.CouncilTaxBand.D };
            yield return new object[] { EligibilityTypes.LIHelpToHeatGroup, ReferenceDataConstants.CouncilTaxBand.E };
        }
    }
}
