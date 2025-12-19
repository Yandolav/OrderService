using Core.Contracts.Orders;
using Core.Contracts.Products;
using Core.Extensions;
using Core.Model.Entities;
using Core.Model.Pagination;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Extensions;
using Kafka.Extensions;
using Kafka.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Task1;
using Task1.DI;
using Task2;
using Task2.Provider;
using Task2.Service;

IConfigurationRoot baseConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();
services.Configure<ConfigClientOptions>(baseConfig.GetSection("ConfigClient"));
services.Configure<ConfigRefreshOptions>(baseConfig.GetSection("ConfigRefresh"));
var externalProvider = new ExternalConfigurationProvider();
services.AddSingleton(externalProvider);
services.AddConfigClientRefit();
services.AddSingleton<IConfigRefreshService, ConfigRefreshService>();

IConfigurationRoot appConfig = new ConfigurationBuilder()
    .AddConfiguration(baseConfig)
    .Add(new ExternalConfigurationSource(externalProvider))
    .Build();

services.Configure<DatabaseOptions>(appConfig.GetSection("Database"));
services
    .AddPostgres()
    .AddRepositories()
    .AddApplicationServices()
    .AddMigrations();

services.AddKafka();
services.Configure<KafkaProducerOptions>(appConfig.GetSection("Kafka:Connection"));
services.Configure<KafkaTopicsOptions>(appConfig.GetSection("Kafka:Topics"));
services.Configure<KafkaConsumerOptions>(appConfig.GetSection("KafkaConsumer"));

ServiceProvider sp = services.BuildServiceProvider();
IConfigRefreshService configRefresh = sp.GetRequiredService<IConfigRefreshService>();
await configRefresh.RefreshOnceAsync(CancellationToken.None);
sp.RunMigrations();

IProductsService products = sp.GetRequiredService<IProductsService>();
IOrdersService orders = sp.GetRequiredService<IOrdersService>();
CancellationToken ct = CancellationToken.None;

long prod1 = await products.CreateProductAsync("Keyboard", 49.99m, ct);
long prod2 = await products.CreateProductAsync("Mouse", 19.50m, ct);
long prod3 = await products.CreateProductAsync("Monitor", 199.00m, ct);
Console.WriteLine($"Созданы товары: {prod1}, {prod2}, {prod3}");

long orderId = await orders.CreateOrderAsync("tester", ct);
Console.WriteLine($"Создан заказ {orderId}");

long item1 = await orders.AddOrderItemAsync(orderId, prod1, 1, ct);
long item2 = await orders.AddOrderItemAsync(orderId, prod2, 2, ct);
long item3 = await orders.AddOrderItemAsync(orderId, prod3, 1, ct);
Console.WriteLine($"Добавлены позиции: {item1}, {item2}, {item3}");

await orders.RemoveOrderItemAsync(item2, ct);
Console.WriteLine($"Удалена позиция {item2}");

await orders.StartProcessingAsync(orderId, ct);
Console.WriteLine("Статус -> processing");
await orders.CompleteAsync(orderId, ct);
Console.WriteLine("Статус -> completed");

IAsyncEnumerable<OrderHistory> hist = orders.GetOrderHistoryAsync([orderId, 1], null, new Paging(100, 0), ct);
Console.WriteLine("История заказа:");
await foreach (OrderHistory h in hist)
{
    Console.WriteLine($"{h.OrderId} | {h.OrderHistoryItemCreatedAt:u} | {h.OrderHistoryItemKind} | {JsonSerializer.Serialize(h.OrderHistoryItemPayload)}");
}

Console.WriteLine("OK");