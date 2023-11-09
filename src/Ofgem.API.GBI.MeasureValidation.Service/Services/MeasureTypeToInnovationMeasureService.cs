using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using System.Collections.Concurrent;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class MeasureTypeToInnovationMeasureService : IMeasureTypeToInnovationMeasureService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MeasureTypeToInnovationMeasureService> _logger;
        private readonly ConcurrentDictionary<string, List<string>> _measureTypeInnovationMeasureDictionary;
        
        public MeasureTypeToInnovationMeasureService(ILogger<MeasureTypeToInnovationMeasureService>  logger, IServiceProvider services)
        {
            _serviceProvider = services;
            _logger = logger;
            _measureTypeInnovationMeasureDictionary = new ConcurrentDictionary<string, List<string>>();
        }

        public async Task<List<string>> GetMeasureTypeInnovationNumbers(string measureType)
        {
            if (_measureTypeInnovationMeasureDictionary.TryGetValue(measureType, out var numbers)) {
                return numbers;
            }

            var measureTypeInnovationNumbers = await GetMeasureTypeInnovationNumberFromDatabase(measureType);

            if (!measureTypeInnovationNumbers.IsNullOrEmpty())
            {
                _measureTypeInnovationMeasureDictionary.TryAdd(measureType, measureTypeInnovationNumbers);
                return _measureTypeInnovationMeasureDictionary[measureType];
            }
            return new List<string>();
        }

        private async Task<List<string>> GetMeasureTypeInnovationNumberFromDatabase(string measureType)
        {
            try
            {
                using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var measureRepository = scope.ServiceProvider.GetRequiredService<IMeasureRepository>();
                    var measureTypeInnovationNumbers = await measureRepository.GetMeasureTypesInnovationNumbers(measureType);
                    return measureTypeInnovationNumbers;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get Measure Type Innovation Numbers failed. {ex.Message}");
                throw;
            }
        }
    }
}
