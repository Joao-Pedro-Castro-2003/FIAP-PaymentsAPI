using MassTransit;
using FiapCloudGames.Contracts;
namespace PaymentsAPI.Messaging;
public sealed class OrderPlacedConsumer(ILogger<OrderPlacedConsumer> log, IConfiguration cfg, IPaymentDecisionStore decisions) : IConsumer<OrderPlacedEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedEvent> c)
    {
        var decision = await decisions.GetOrCreate(c.Message, cfg.GetValue("Payment:ApprovalRate", 90), c.CancellationToken);
        log.LogInformation("Pagamento {OrderId}: {Status}", c.Message.OrderId, decision.Approved ? "Approved" : "Rejected");
        await c.Publish(new PaymentProcessedEvent(c.Message.OrderId, decision.UserId, decision.GameId, decision.Price,
            decision.Approved, decision.Approved ? "Pagamento aprovado" : "Pagamento rejeitado pela simulacao"));
    }
}
