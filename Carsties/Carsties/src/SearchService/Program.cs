
using Contracts;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Models;
using SearchService.Services;

namespace SearchService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddHttpClient<AuctionSvcHttpClient>()
                .AddPolicyHandler(GetPolicy());

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

                x.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("updated", false));

                x.AddConsumersFromNamespaceContaining<AuctionDeletedConsumer>();
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("deleted", false));

                x.UsingRabbitMq((context, cfg) =>
                {
                    //To handle in case mongo db is down
                    cfg.ReceiveEndpoint("search-auction-created", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(5, 5));
                        e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            app.Lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    await DbInitializer.InitDb(app);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            app.Run();
        }

        static IAsyncPolicy<HttpResponseMessage> GetPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

    }
}