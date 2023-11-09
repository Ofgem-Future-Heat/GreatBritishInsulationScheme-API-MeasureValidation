using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0624RuleLogic : GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic
    {
        public Gbis0624RuleLogic(IInFillMeasureService? inFillMeasureService)
            : base("GBIS0624", measureModel => measureModel.AssociatedInfillMeasure2, inFillMeasureService)
        {
        }
    }
}
