using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IStage2PassedMeasuresProcessor
    {
        Task<IEnumerable<MeasureModel>> Process(IEnumerable<MeasureModel> measures);
    }
}
