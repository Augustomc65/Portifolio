using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TicketDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Tickets") ?? "Data Source=ticketflow.db"));

var app = builder.Build();
app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "TicketFlow API" }));

app.MapGet("/api/tickets", async (TicketDbContext db, string? status) =>
{
    var query = db.Tickets.AsNoTracking();
    if (!string.IsNullOrWhiteSpace(status))
        query = query.Where(ticket => ticket.Status == status);

    return Results.Ok(await query.OrderByDescending(ticket => ticket.CreatedAt).ToListAsync());
});

app.MapGet("/api/tickets/{id:guid}", async (Guid id, TicketDbContext db) =>
{
    var ticket = await db.Tickets.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);
});

app.MapPost("/api/tickets", async (CreateTicketRequest request, TicketDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) ||
        string.IsNullOrWhiteSpace(request.Description) ||
        string.IsNullOrWhiteSpace(request.Requester))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = ["Título, descrição e solicitante são obrigatórios."]
        });

    var priorities = new[] { "Low", "Medium", "High", "Critical" };
    if (!priorities.Contains(request.Priority))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["priority"] = ["Use Low, Medium, High ou Critical."]
        });

    var ticket = new SupportTicket
    {
        Id = Guid.NewGuid(),
        Title = request.Title.Trim(),
        Description = request.Description.Trim(),
        Requester = request.Requester.Trim(),
        Priority = request.Priority,
        Status = "Open",
        CreatedAt = DateTimeOffset.UtcNow
    };

    db.Tickets.Add(ticket);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tickets/{ticket.Id}", ticket);
});

app.MapPatch("/api/tickets/{id:guid}/status", async (Guid id, ChangeTicketStatusRequest request, TicketDbContext db) =>
{
    var allowed = new[] { "Open", "InProgress", "WaitingForUser", "Resolved", "Closed" };
    if (!allowed.Contains(request.Status))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["status"] = ["Status inválido. Use Open, InProgress, WaitingForUser, Resolved ou Closed."]
        });

    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();

    ticket.Status = request.Status;
    ticket.ClosedAt = request.Status == "Closed" ? DateTimeOffset.UtcNow : null;
    await db.SaveChangesAsync();
    return Results.Ok(ticket);
});

app.MapGet("/api/tickets/summary", async (TicketDbContext db) =>
{
    var tickets = await db.Tickets.AsNoTracking().ToListAsync();
    return Results.Ok(new
    {
        total = tickets.Count,
        open = tickets.Count(ticket => ticket.Status == "Open"),
        inProgress = tickets.Count(ticket => ticket.Status == "InProgress"),
        waitingForUser = tickets.Count(ticket => ticket.Status == "WaitingForUser"),
        resolved = tickets.Count(ticket => ticket.Status == "Resolved"),
        closed = tickets.Count(ticket => ticket.Status == "Closed")
    });
});

app.Run();

public sealed class TicketDbContext(DbContextOptions<TicketDbContext> options) : DbContext(options)
{
    public DbSet<SupportTicket> Tickets => Set<SupportTicket>();
}

public sealed class SupportTicket
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Requester { get; set; } = "";
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
}

public sealed record CreateTicketRequest(string Title, string Description, string Requester, string Priority);
public sealed record ChangeTicketStatusRequest(string Status);
