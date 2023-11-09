using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IMeasureStage1RulesService
    {
        Task<Stage1ValidationResultModel> ValidateMeasures(IEnumerable<MeasureModel> measures);
    }
}
