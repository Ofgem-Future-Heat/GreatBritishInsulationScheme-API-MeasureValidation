using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0604RuleLogic : GbisInFillMeasureReferenceNumberValidatorRuleLogic
    {
        public Gbis0604RuleLogic()
            : base("GBIS0604", measureModel => measureModel.AssociatedInfillMeasure2)
        {
        }
    }
}
