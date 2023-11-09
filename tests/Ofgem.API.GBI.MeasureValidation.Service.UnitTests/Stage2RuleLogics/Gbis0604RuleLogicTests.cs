using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0604RuleLogicTests : GbisInFillMeasureReferenceNumberValidatorRuleLogicTestsBase<Gbis0604RuleLogic>
    {
        public override Gbis0604RuleLogic RuleLogic { get; } = new();

        public override string TestNumber => "GBIS0604";

        public override Action<MeasureModel, string?> FailureFieldSetterFunction { get; } =
            (measureModel, measureReferenceNumber) => measureModel.AssociatedInfillMeasure2 = measureReferenceNumber;
    }
}