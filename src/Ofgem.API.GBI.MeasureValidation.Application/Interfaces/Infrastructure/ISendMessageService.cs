
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure
{
    public interface ISendMessageService
    {
        Task SendMessageToTriggerStage2ValidationAsync(MeasureDocumentDetails message);
    }
}
