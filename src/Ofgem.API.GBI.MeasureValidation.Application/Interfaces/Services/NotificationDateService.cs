using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public interface INotificationDateService
    {
        bool HasNotificationEndDatePassed(MeasureModel measure);
    }
}
