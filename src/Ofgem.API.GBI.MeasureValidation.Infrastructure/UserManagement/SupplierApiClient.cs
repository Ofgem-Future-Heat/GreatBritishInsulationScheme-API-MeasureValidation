using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.UserManagement
{
    public class SupplierApiClient : ISupplierApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SupplierApiClient> _logger;

        public SupplierApiClient(HttpClient httpClient, ILogger<SupplierApiClient> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SupplierLicenceResponse>> GetSupplierLicencesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("suppliers-licences");

                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<SupplierLicenceResponse>>(responseMessage)!;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Supplier Licences failed. {exMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<SupplierResponse>> GetSuppliersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("suppliers");

                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IEnumerable<SupplierResponse>>(responseMessage)!;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Suppliers failed. {exMessage}", ex.Message);
                throw;
            }
        }
    }
}
