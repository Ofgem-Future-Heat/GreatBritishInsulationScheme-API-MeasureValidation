using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0624RuleLogicTests : GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogicTestsBase<Gbis0624RuleLogic>
    {
        public override Func<IInFillMeasureService, Gbis0624RuleLogic> RuleLogicCreator { get; } =
            inFillMeasureService => new Gbis0624RuleLogic(inFillMeasureService);

        public override string TestNumber { get; } = "GBIS0624";

        public override Action<MeasureModel, string?> FailureFieldSetterFunction { get; } =
            (measureModel, infillMeasureReferenceNumber) =>
                measureModel.AssociatedInfillMeasure2 = infillMeasureReferenceNumber;
    }
}