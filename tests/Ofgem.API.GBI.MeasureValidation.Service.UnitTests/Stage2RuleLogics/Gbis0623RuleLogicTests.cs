using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0623RuleLogicTests : GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogicTestsBase<Gbis0623RuleLogic>
    {
        public override Func<IInFillMeasureService, Gbis0623RuleLogic> RuleLogicCreator { get; } =
            inFillMeasureService => new Gbis0623RuleLogic(inFillMeasureService);

        public override string TestNumber { get; } = "GBIS0623";

        public override Action<MeasureModel, string?> FailureFieldSetterFunction { get; } =
            (measureModel, infillMeasureReferenceNumber) =>
                measureModel.AssociatedInfillMeasure1 = infillMeasureReferenceNumber;
    }
}