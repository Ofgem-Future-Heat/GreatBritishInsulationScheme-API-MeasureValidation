using MassTransit;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.ServiceBus;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.UnitTests.Services
{
    public class SendMessageServiceTests
    {
        private readonly Mock<IBus> _bus;
        private readonly Mock<ISendEndpoint> _sendEndPoint;
        private readonly ISendMessageService _sendMessageService;

        public SendMessageServiceTests()
        {
            _bus = new Mock<IBus>();
            _sendEndPoint = new Mock<ISendEndpoint>();
            _sendMessageService = new SendMessageService(_bus.Object);
        }

        [Fact]
        public async Task SendMessage_SuccessfullySent()
        {
            // Arrange
            _bus.Setup(m => m.GetSendEndpoint(new Uri("queue:stage-2-validation"))).Returns(Task.FromResult(_sendEndPoint.Object));

            // Act
            await _sendMessageService.SendMessageToTriggerStage2ValidationAsync(new MeasureDocumentDetails
            {
                DocumentId = default
            });

            // Assert
            _sendEndPoint.Verify(
                m => m.Send(
                    It.IsAny<MeasureDocumentDetails>(),
                    It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );

        }

        [Fact]
        public async Task SendMessage_FailedSent()
        {
            // Arrange
            _sendEndPoint.Setup(
                m => m.Send(
                    It.IsAny<MeasureDocumentDetails>(),
                    It.IsAny<CancellationToken>()
                    )
                ).ThrowsAsync(new Exception());

            _bus.Setup(m => m.GetSendEndpoint(new Uri("queue:stage-2-validation"))).Returns(Task.FromResult(_sendEndPoint.Object));

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _sendMessageService.SendMessageToTriggerStage2ValidationAsync(new MeasureDocumentDetails());
            });

        }

    }
}
