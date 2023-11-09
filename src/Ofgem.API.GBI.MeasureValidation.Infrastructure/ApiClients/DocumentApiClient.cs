using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using System.Collections.Immutable;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.ApiClients
{
    public class DocumentApiClient : IDocumentApiClient
    {
        private readonly string _baseurl;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DocumentApiClient> _logger;

        public DocumentApiClient(IConfiguration configuration, HttpClient httpClient, ILogger<DocumentApiClient> logger)
        {
            _baseurl = configuration.GetSection("DocumentServiceApiUrl").Value ?? "";
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<MeasureFileContent> GetDocument(Guid documentId)
        {
            try
            {
                string url = $"{_baseurl}Get/{documentId}";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<MeasureFileContent>(responseMessage)!;
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot get document {documentId}. {exMessage}", documentId, ex.Message);
                throw;
            }
        }

        public async Task<IDictionary<Guid, string>> GetDocumentsNames(IEnumerable<Guid> documentIds)
        {
            try
            {
                if (documentIds.Any())
                {
                    string url = $"{_baseurl}GetDocumentsNames";
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(url),
                        Content = new StringContent(JsonConvert.SerializeObject(documentIds), new MediaTypeHeaderValue(MediaTypeNames.Application.Json)),
                    };
                    var response = await _httpClient.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    var responseMessage = await response.Content.ReadAsStringAsync();

                    var documentNames = JsonConvert.DeserializeObject<IDictionary<Guid, string>>(responseMessage);

                    return documentNames!;
                }
                else
                {
                    return ImmutableDictionary<Guid, string>.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot get document names. {exMessage}", ex.Message);
                throw;
            }
        }
    }
}
