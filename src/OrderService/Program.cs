using OrderService.Models;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ProductServiceClient>(client =>
{
    var productServiceUrl = builder.Configuration["ProductService:BaseUrl"] ?? "http://productservice:8080";
    client.BaseAddress = new Uri(productServiceUrl);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var orders = new List<Order>();
var nextOrderId = 1;

app.MapGet("/", () => orders)
    .WithName("GetAllOrders")
    .WithOpenApi();

app.MapGet("/{id:int}", (int id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetOrderById")
.WithOpenApi();

app.MapPost("/", async (CreateOrderRequest request, ProductServiceClient productClient) =>
{
    var product = await productClient.GetProductAsync(request.ProductId);

    if (product is null)
    {
        return Results.BadRequest($"Product with ID {request.ProductId} not found");
    }

    if (product.StockQuantity < request.Quantity)
    {
        return Results.BadRequest($"Insufficient stock. Available: {product.StockQuantity}");
    }

    var stockUpdated = await productClient.UpdateStockAsync(request.ProductId, request.Quantity);
    if (!stockUpdated)
    {
        return Results.BadRequest("Failed to update product stock");
    }

    var order = new Order
    {
        Id = nextOrderId++,
        ProductId = product.Id,
        ProductName = product.Name,
        ProductPrice = product.Price,
        Quantity = request.Quantity,
        TotalAmount = product.Price * request.Quantity,
        OrderDate = DateTime.UtcNow,
        Status = "Confirmed"
    };

    orders.Add(order);

    return Results.Created($"/{order.Id}", order);
})
.WithName("CreateOrder")
.WithOpenApi();

app.Run();
