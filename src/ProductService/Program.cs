using ProductService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 999.99m, StockQuantity = 50 },
    new() { Id = 2, Name = "Mouse", Price = 29.99m, StockQuantity = 200 },
    new() { Id = 3, Name = "Keyboard", Price = 79.99m, StockQuantity = 150 }
};

app.MapGet("/", () => products)
    .WithName("GetAllProducts")
    .WithOpenApi();

app.MapGet("/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProductById")
.WithOpenApi();

app.MapPost("/", (Product product) =>
{
    product.Id = products.Max(p => p.Id) + 1;
    products.Add(product);
    return Results.Created($"/{product.Id}", product);
})
.WithName("CreateProduct")
.WithOpenApi();

app.MapPut("/stock/{id:int}", (int id, int quantity) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();

    product.StockQuantity += quantity;
    return Results.Ok(product);
})
.WithName("UpdateStock")
.WithOpenApi();

app.Run();
