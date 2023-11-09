using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
	public interface IMeasureStage2RulesService
	{
        Task<Stage2ValidationResultModel> ValidateMeasures(IEnumerable<MeasureModel> measures);
    }
}
