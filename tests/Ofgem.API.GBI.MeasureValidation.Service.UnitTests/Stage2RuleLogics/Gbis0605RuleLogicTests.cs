using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0605RuleLogicTests : GbisInFillMeasureReferenceNumberValidatorRuleLogicTestsBase<Gbis0605RuleLogic>
    {
        public override Gbis0605RuleLogic RuleLogic { get; } = new();

        public override string TestNumber => "GBIS0605";

        public override Action<MeasureModel, string?> FailureFieldSetterFunction { get; } =
            (measureModel, measureReferenceNumber) => measureModel.AssociatedInfillMeasure3 = measureReferenceNumber;
    }
}