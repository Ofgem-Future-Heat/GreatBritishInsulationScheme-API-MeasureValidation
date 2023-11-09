using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics;

public class Gbis0625RuleLogicTests : GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogicTestsBase<Gbis0625RuleLogic>
{
    public override Func<IInFillMeasureService, Gbis0625RuleLogic> RuleLogicCreator { get; } =
        inFillMeasureService => new Gbis0625RuleLogic(inFillMeasureService);

    public override string TestNumber { get; } = "GBIS0625";

    public override Action<MeasureModel, string?> FailureFieldSetterFunction { get; } =
        (measureModel, infillMeasureReferenceNumber) =>
            measureModel.AssociatedInfillMeasure3 = infillMeasureReferenceNumber;
}