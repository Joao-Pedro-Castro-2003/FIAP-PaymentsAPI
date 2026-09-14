using FiapCloudGames.Contracts;
using MongoDB.Driver;
namespace PaymentsAPI.Messaging;
public sealed class PaymentDecision
{
    public string Id { get; set; } = "";
    public int UserId { get; set; }
    public int GameId { get; set; }
    [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
    public decimal Price { get; set; }
    public bool Approved { get; set; }
}
public interface IPaymentDecisionStore
{
    Task<PaymentDecision> GetOrCreate(OrderPlacedEvent order, int approvalRate, CancellationToken ct);
}
public sealed class PaymentDecisionStore(IMongoClient client) : IPaymentDecisionStore
{
    public async Task<PaymentDecision> GetOrCreate(OrderPlacedEvent order, int approvalRate, CancellationToken ct)
    {
        var collection = client.GetDatabase("fcg_payments").GetCollection<PaymentDecision>("decisions");
        var id = order.OrderId.ToString();
        var proposed = new PaymentDecision { Id = id, UserId = order.UserId, GameId = order.GameId,
            Price = order.Price, Approved = Random.Shared.Next(100) < Math.Clamp(approvalRate, 0, 100) };
        try { await collection.InsertOneAsync(proposed, cancellationToken: ct); return proposed; }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey) {
            var existing = await collection.Find(x => x.Id == id).SingleAsync(ct);
            if (existing.UserId != order.UserId || existing.GameId != order.GameId || existing.Price != order.Price)
                throw new InvalidOperationException("Pedido repetido com valores diferentes");
            return existing;
        }
    }
}
