namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface IMeasureTypeToCategoryService
    {
        Task<string> GetMeasureCategoryByType(string measureType);
    }
}