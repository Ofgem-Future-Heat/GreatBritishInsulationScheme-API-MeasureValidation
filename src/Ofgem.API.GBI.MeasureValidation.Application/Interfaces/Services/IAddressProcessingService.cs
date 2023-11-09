using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;

public interface IAddressProcessingService
{
    public Task<IEnumerable<MeasureModel>> AddressVerificationAsync(IEnumerable<MeasureModel> measureModels);
}