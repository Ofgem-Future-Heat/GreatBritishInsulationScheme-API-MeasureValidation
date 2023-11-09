using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0605RuleLogic : GbisInFillMeasureReferenceNumberValidatorRuleLogic
    {
        public Gbis0605RuleLogic() 
            : base("GBIS0605", measureModel => measureModel.AssociatedInfillMeasure3)
        {
        }
    }
}
