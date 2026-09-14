using MassTransit;
using PaymentsAPI.Messaging;
using MongoDB.Driver;
using Prometheus;
var b = WebApplication.CreateBuilder(args);
b.Logging.ClearProviders();
b.Logging.AddJsonConsole();
b.Services.AddControllers();
b.Services.AddEndpointsApiExplorer();
b.Services.AddSwaggerGen();
b.Services.AddProblemDetails();
b.Services.AddSingleton<IMongoClient>(_ => new MongoClient(b.Configuration["Mongo:ConnectionString"] ?? "mongodb://localhost:27017/?serverSelectionTimeoutMS=3000"));
b.Services.AddSingleton<IPaymentDecisionStore, PaymentDecisionStore>();
b.Services.AddMassTransit(x => {
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((c, q) => {
        q.Host(b.Configuration["RabbitMq:Host"] ?? "localhost", h => {
            h.Username(b.Configuration["RabbitMq:Username"] ?? "guest");
            h.Password(b.Configuration["RabbitMq:Password"] ?? "guest");
        });
        q.ReceiveEndpoint("payments-order-placed", e => {
            e.UseMessageRetry(r => r.Intervals(1000, 3000, 5000));
            e.UseInMemoryOutbox(c);
            e.ConfigureConsumer<OrderPlacedConsumer>(c);
        });
    });
});
var app = b.Build();
app.UseRouting();
app.UseHttpMetrics();
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));
app.MapMetrics();
app.MapControllers();
app.Run();
public partial class Program { }
