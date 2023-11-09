using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure
{
    public interface ISupplierApiClient
    {
        Task<IEnumerable<SupplierLicenceResponse>> GetSupplierLicencesAsync();
        Task<IEnumerable<SupplierResponse>> GetSuppliersAsync();
    }
}
