using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure
{
    public interface IAddressApiClient
    {
        Task<List<AddressValidationResponse>> ValidateAddressAsync(IEnumerable<AddressValidationModel> request);
    }
}