using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0623RuleLogic : GbisInFillMeasureNotAlreadyAssignedAsInfillMeasureRuleLogic
    {
        public Gbis0623RuleLogic(IInFillMeasureService? inFillMeasureService)
            : base("GBIS0623", measureModel => measureModel.AssociatedInfillMeasure1, inFillMeasureService)
        {
        }
    }
}