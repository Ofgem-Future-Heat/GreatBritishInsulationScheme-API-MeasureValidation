using MassTransit;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.ServiceBus
{
    public class SendMessageService : ISendMessageService
    {
        private readonly IBus _bus;

        public SendMessageService(IBus bus)
        {
            _bus = bus;
        }

        public async Task SendMessageToTriggerStage2ValidationAsync(MeasureDocumentDetails message)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri("queue:stage-2-validation"));
            await endpoint.Send(message);
        }
    }
}
