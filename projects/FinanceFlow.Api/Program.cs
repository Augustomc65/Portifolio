using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Finance") ?? "Data Source=financeflow.db"));

var app = builder.Build();
app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "FinanceFlow API" }));

app.MapGet("/api/transactions", async (FinanceDbContext db, int? month, int? year) =>
{
    var query = db.Transactions.AsNoTracking();
    if (month is >= 1 and <= 12 && year is not null)
    {
        var start = new DateTime(year.Value, month.Value, 1);
        var end = start.AddMonths(1);
        query = query.Where(item => item.Date >= start && item.Date < end);
    }

    var items = await query.OrderByDescending(item => item.Date).ToListAsync();
    return Results.Ok(items.Select(ToView));
});

app.MapPost("/api/transactions", async (CreateTransactionRequest request, FinanceDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.Category))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["description"] = ["Descrição e categoria são obrigatórias."]
        });

    if (request.Amount <= 0 || request.Amount > 1_000_000)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["amount"] = ["O valor deve ser maior que zero e menor ou igual a 1.000.000."]
        });

    var type = request.Type.Trim().ToLowerInvariant();
    if (type is not ("income" or "expense"))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["type"] = ["Use income ou expense."]
        });

    var item = new FinanceTransaction
    {
        Id = Guid.NewGuid(),
        Description = request.Description.Trim(),
        AmountInCents = (long)Math.Round(request.Amount * 100, MidpointRounding.AwayFromZero),
        Type = type,
        Category = request.Category.Trim(),
        Date = request.Date ?? DateTime.UtcNow
    };

    db.Transactions.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/transactions/{item.Id}", ToView(item));
});

app.MapDelete("/api/transactions/{id:guid}", async (Guid id, FinanceDbContext db) =>
{
    var item = await db.Transactions.FindAsync(id);
    if (item is null) return Results.NotFound();
    db.Transactions.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/api/summary", async (FinanceDbContext db, int? month, int? year) =>
{
    var selectedMonth = month is >= 1 and <= 12 ? month.Value : DateTime.UtcNow.Month;
    var selectedYear = year ?? DateTime.UtcNow.Year;
    var start = new DateTime(selectedYear, selectedMonth, 1);
    var end = start.AddMonths(1);
    var items = await db.Transactions.AsNoTracking()
        .Where(item => item.Date >= start && item.Date < end)
        .ToListAsync();

    var income = items.Where(item => item.Type == "income").Sum(item => item.AmountInCents);
    var expenses = items.Where(item => item.Type == "expense").Sum(item => item.AmountInCents);

    return Results.Ok(new
    {
        month = selectedMonth,
        year = selectedYear,
        income = income / 100m,
        expenses = expenses / 100m,
        balance = (income - expenses) / 100m,
        transactionCount = items.Count
    });
});

app.Run();

static FinanceTransactionView ToView(FinanceTransaction item) =>
    new(item.Id, item.Description, item.AmountInCents / 100m, item.Type, item.Category, item.Date);

public sealed class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<FinanceTransaction> Transactions => Set<FinanceTransaction>();
}

public sealed class FinanceTransaction
{
    public Guid Id { get; set; }
    public string Description { get; set; } = "";
    public long AmountInCents { get; set; }
    public string Type { get; set; } = "expense";
    public string Category { get; set; } = "";
    public DateTime Date { get; set; }
}

public sealed record CreateTransactionRequest(string Description, decimal Amount, string Type, string Category, DateTime? Date);
public sealed record FinanceTransactionView(Guid Id, string Description, decimal Amount, string Type, string Category, DateTime Date);
