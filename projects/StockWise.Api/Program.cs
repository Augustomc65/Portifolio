using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Stock") ?? "Data Source=stockwise.db"));

var app = builder.Build();
app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "StockWise API" }));

app.MapGet("/api/products", async (StockDbContext db) =>
    Results.Ok(await db.Products.AsNoTracking().OrderBy(product => product.Name).ToListAsync()));

app.MapGet("/api/products/low-stock", async (StockDbContext db) =>
    Results.Ok(await db.Products.AsNoTracking()
        .Where(product => product.Quantity <= product.MinimumStock)
        .OrderBy(product => product.Quantity)
        .ToListAsync()));

app.MapPost("/api/products", async (CreateProductRequest request, StockDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Sku))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["product"] = ["Nome e SKU são obrigatórios."]
        });

    if (request.MinimumStock < 0 || request.UnitPrice < 0)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["values"] = ["Estoque mínimo e preço não podem ser negativos."]
        });

    var sku = request.Sku.Trim().ToUpperInvariant();
    if (await db.Products.AnyAsync(product => product.Sku == sku))
        return Results.Conflict(new { message = "Já existe um produto com este SKU." });

    var product = new Product
    {
        Id = Guid.NewGuid(),
        Name = request.Name.Trim(),
        Sku = sku,
        Quantity = 0,
        MinimumStock = request.MinimumStock,
        UnitPriceInCents = (long)Math.Round(request.UnitPrice * 100, MidpointRounding.AwayFromZero)
    };

    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapPost("/api/products/{id:guid}/movements", async (Guid id, CreateMovementRequest request, StockDbContext db) =>
{
    if (request.Quantity <= 0)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["quantity"] = ["A quantidade deve ser maior que zero."]
        });

    var type = request.Type.Trim().ToUpperInvariant();
    if (type is not ("IN" or "OUT"))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["type"] = ["Use IN para entrada ou OUT para saída."]
        });

    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    if (type == "OUT" && product.Quantity < request.Quantity)
        return Results.Conflict(new { message = "Estoque insuficiente para esta saída." });

    product.Quantity += type == "IN" ? request.Quantity : -request.Quantity;
    var movement = new StockMovement
    {
        Id = Guid.NewGuid(),
        ProductId = id,
        Type = type,
        Quantity = request.Quantity,
        Note = request.Note?.Trim() ?? "",
        OccurredAt = DateTimeOffset.UtcNow
    };

    db.Movements.Add(movement);
    await db.SaveChangesAsync();
    return Results.Ok(new { product, movement });
});

app.MapGet("/api/products/{id:guid}/movements", async (Guid id, StockDbContext db) =>
{
    if (!await db.Products.AnyAsync(product => product.Id == id)) return Results.NotFound();
    var movements = await db.Movements.AsNoTracking()
        .Where(movement => movement.ProductId == id)
        .OrderByDescending(movement => movement.OccurredAt)
        .ToListAsync();
    return Results.Ok(movements);
});

app.Run();

public sealed class StockDbContext(DbContextOptions<StockDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> Movements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasIndex(product => product.Sku).IsUnique();
        modelBuilder.Entity<StockMovement>().HasIndex(movement => movement.ProductId);
    }
}

public sealed class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Sku { get; set; } = "";
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public long UnitPriceInCents { get; set; }
}

public sealed class StockMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Type { get; set; } = "IN";
    public int Quantity { get; set; }
    public string Note { get; set; } = "";
    public DateTimeOffset OccurredAt { get; set; }
}

public sealed record CreateProductRequest(string Name, string Sku, int MinimumStock, decimal UnitPrice);
public sealed record CreateMovementRequest(string Type, int Quantity, string? Note);
