namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IMeasureTypeToInnovationMeasureService
    {
        Task<List<string>> GetMeasureTypeInnovationNumbers(string measureType);
    }
}
