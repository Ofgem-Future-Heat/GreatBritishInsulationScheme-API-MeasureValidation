using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class InFillMeasureService : IInFillMeasureService
    {
        private readonly IMeasureRepository _repository;
        private readonly ILogger<InFillMeasureService> _logger;

        public InFillMeasureService(IMeasureRepository repository, ILogger<InFillMeasureService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> IsInfillMeasureAssigned(string measureReferenceNumber, string infillMeasureReferenceNumber)
        {
            try
            {
                var result = await _repository.GetMeasureInfillsAsync(measureReferenceNumber, infillMeasureReferenceNumber);
                return result.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsInfillMeasureAssigned failed. {message}", ex.Message);
                throw;
            }
        }
    }
}
