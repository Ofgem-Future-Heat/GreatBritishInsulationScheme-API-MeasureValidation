using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0603RuleLogic : GbisInFillMeasureReferenceNumberValidatorRuleLogic
    {
        public Gbis0603RuleLogic()
            : base("GBIS0603", measureModel => measureModel.AssociatedInfillMeasure1)
        {
        }
    }
}
