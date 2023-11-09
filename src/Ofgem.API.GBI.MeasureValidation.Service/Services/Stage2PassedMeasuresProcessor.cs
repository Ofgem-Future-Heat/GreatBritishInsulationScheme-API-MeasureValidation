using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services;

public class Stage2PassedMeasuresProcessor : IStage2PassedMeasuresProcessor
{
    private readonly INotificationDateService _notificationDateService;

    public Stage2PassedMeasuresProcessor(INotificationDateService notificationDateService)
    {
        _notificationDateService = notificationDateService;
    }

    public Task<IEnumerable<MeasureModel>> Process(IEnumerable<MeasureModel> measures)
    {
        if (measures == null) throw new ArgumentNullException(nameof(measures));

        var measuresArray = measures as MeasureModel[] ?? measures.ToArray();
        if (measuresArray.Any())
        {
            var measuresGroupedByIfNotificationEndDatePassed = measuresArray.GroupBy(_notificationDateService.HasNotificationEndDatePassed);
            foreach (var measureModels in measuresGroupedByIfNotificationEndDatePassed)
            {
                var newMeasureStatusId = measureModels.Key
                    ? MeasureStatusConstants.MeasureAwaitingVerification
                    : MeasureStatusConstants.NotifiedPending;
                foreach (var measureModel in measureModels)
                {
                    measureModel.MeasureStatusId = newMeasureStatusId;
                }
            }
        }

        return Task.FromResult(measuresArray.AsEnumerable());
    }
}