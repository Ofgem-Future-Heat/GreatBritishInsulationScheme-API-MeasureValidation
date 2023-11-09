using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0603RuleLogicTests : GbisInFillMeasureReferenceNumberValidatorRuleLogicTestsBase<Gbis0603RuleLogic>
    {
        public override Gbis0603RuleLogic RuleLogic { get; } = new();

        public override string TestNumber => "GBIS0603";

        public override Action<MeasureModel, string?> FailureFieldSetterFunction { get; } =
            (measureModel, measureReferenceNumber) => measureModel.AssociatedInfillMeasure1 = measureReferenceNumber;
    }
}
