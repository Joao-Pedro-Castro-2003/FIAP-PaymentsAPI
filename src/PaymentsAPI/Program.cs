using MassTransit;
using PaymentsAPI.Messaging;

var b = WebApplication.CreateBuilder(args);

b.Services.AddControllers();

b.Services.AddEndpointsApiExplorer();

b.Services.AddSwaggerGen();

b.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((c, q) =>
    {
        q.Host(b.Configuration["RabbitMq:Host"] ?? "localhost", h =>
        {
            h.Username("guest"); h.Password("guest");
        });

        q.ReceiveEndpoint("payments-order-placed", e => e.ConfigureConsumer<OrderPlacedConsumer>(c));
    });
});

var app = b.Build(); 

app.UseSwagger(); 
app.UseSwaggerUI(); 
app.MapControllers(); 
app.Run(); 
public partial class Program { }
