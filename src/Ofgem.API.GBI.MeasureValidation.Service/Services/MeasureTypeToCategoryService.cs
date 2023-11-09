using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using System.Collections.Concurrent;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class MeasureTypeToCategoryService : IMeasureTypeToCategoryService
    {
        private readonly Lazy<Task<ConcurrentDictionary<string, string>>> _measureCategoriesByTypeTask;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MeasureTypeToCategoryService> _logger;

        public MeasureTypeToCategoryService(ILogger<MeasureTypeToCategoryService> logger, IServiceProvider services)
        {
            _serviceProvider = services;
            _measureCategoriesByTypeTask = new Lazy<Task<ConcurrentDictionary<string, string>>>(GetMeasureCategoriesByTypeAsync());
            _logger = logger;
        }

        public async Task<string> GetMeasureCategoryByType(string measureType)
        {
            var measureCategoriesByType = await _measureCategoriesByTypeTask.Value;
            var result = string.Empty;
            if (measureCategoriesByType.TryGetValue(measureType.ToUpperInvariant(), out var foundCategory))
            {
                result = foundCategory;
            }
            return result;
        }

        private async Task<ConcurrentDictionary<string, string>> GetMeasureCategoriesByTypeAsync()
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var measureRepository = scope.ServiceProvider.GetRequiredService<IMeasureRepository>();
                var measureCategoriesByType = await measureRepository.GetMeasureCategoriesByTypeAsync();
                return measureCategoriesByType;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMeasureCategoriesByType failed to populate Measure Type to Measure Categories dictionary. {ex.Message}");
                return new ConcurrentDictionary<string, string>();
            }

        }
    }
}
