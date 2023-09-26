using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService;
using SearchService.Consumer;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Makes connection to Service Bus Rabbit Mq uses localhost name
builder.Services.AddMassTransit(x => 
{
    // Where to find consumers
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    // Add better naming convention
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5,5));

            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try{
    await DbInitializer.InitDb(app);
}catch (Exception e)
{
    Console.WriteLine(e);    
}

app.Run();
