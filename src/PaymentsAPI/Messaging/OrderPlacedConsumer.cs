using MassTransit;
using FiapCloudGames.Contracts;
namespace PaymentsAPI.Messaging;
public sealed class OrderPlacedConsumer(ILogger<OrderPlacedConsumer> log, IConfiguration cfg) : IConsumer<OrderPlacedEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedEvent> c)
    {
        var approved = Random.Shared.Next(100) < cfg.GetValue("Payment:ApprovalRate", 90);

        log.LogInformation("Pagamento {OrderId}: {Status}", c.Message.OrderId, approved ? "aprovado" : "rejeitado");

        await c.Publish(new PaymentProcessedEvent(c.Message.OrderId, c.Message.UserId, c.Message.GameId, c.Message.Price, approved, approved
            ? "Pagamento aprovado"
            : "Pagamento rejeitado pela simulacao"));
    }
}
