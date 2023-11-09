using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

public class Gbis0625RuleLogic : GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic
{
    public Gbis0625RuleLogic(IInFillMeasureService? inFillMeasureService)
        : base("GBIS0625", measureModel => measureModel.AssociatedInfillMeasure3, inFillMeasureService)
    {
    }
}