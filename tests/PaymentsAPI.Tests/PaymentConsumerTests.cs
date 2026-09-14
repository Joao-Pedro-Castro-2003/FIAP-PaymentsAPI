using FiapCloudGames.Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PaymentsAPI.Messaging;
namespace PaymentsAPI.Tests;
public class PaymentConsumerTests
{
    [Fact]
    public async Task PublishesStoredDecisionInsteadOfResampling()
    {
        var message = new OrderPlacedEvent(Guid.NewGuid(), 42, 1, 80m);
        var store = new Mock<IPaymentDecisionStore>();
        store.Setup(x => x.GetOrCreate(message, 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentDecision { Id = message.OrderId.ToString(), UserId = 42, GameId = 1, Price = 80, Approved = true });
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["Payment:ApprovalRate"] = "0" }).Build();
        var consumer = new OrderPlacedConsumer(NullLogger<OrderPlacedConsumer>.Instance, cfg, store.Object);
        var context = new Mock<ConsumeContext<OrderPlacedEvent>>();
        context.SetupGet(x => x.Message).Returns(message);
        await consumer.Consume(context.Object);
        context.Verify(x => x.Publish(It.Is<PaymentProcessedEvent>(e => e.Approved && e.OrderId == message.OrderId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
