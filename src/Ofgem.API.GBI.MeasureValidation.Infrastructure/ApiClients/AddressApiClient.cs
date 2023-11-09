using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Constants;
using System.Net.Http.Json;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.ApiClients
{
    public class AddressApiClient : IAddressApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AddressApiClient> _logger;

        public AddressApiClient(HttpClient httpClient, ILogger<AddressApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<AddressValidationResponse>> ValidateAddressAsync(IEnumerable<AddressValidationModel> request)
        {
            try
            {
                string url = InfrastructureConstants.AddressApiClient.AddressValidationUrl;
                var response = await _httpClient.PostAsync(url, JsonContent.Create(request));

                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<AddressValidationResponse>>(responseMessage)!;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cannot validate address. {ex.Message}", ex);
                throw;
            }
        }
    }
}