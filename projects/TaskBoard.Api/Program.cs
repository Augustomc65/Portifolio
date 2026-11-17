using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TaskBoardDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Tasks") ?? "Data Source=taskboard.db"));

var app = builder.Build();
app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "TaskBoard API" }));

app.MapGet("/api/tasks", async (TaskBoardDbContext db, string? status, string? priority) =>
{
    var query = db.WorkItems.AsNoTracking();
    if (!string.IsNullOrWhiteSpace(status)) query = query.Where(item => item.Status == status);
    if (!string.IsNullOrWhiteSpace(priority)) query = query.Where(item => item.Priority == priority);
    return Results.Ok(await query.OrderBy(item => item.DueDate).ToListAsync());
});

app.MapGet("/api/tasks/{id:guid}", async (Guid id, TaskBoardDbContext db) =>
{
    var item = await db.WorkItems.AsNoTracking().FirstOrDefaultAsync(task => task.Id == id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/api/tasks", async (CreateWorkItemRequest request, TaskBoardDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = ["O título é obrigatório."]
        });

    var priorities = new[] { "Low", "Medium", "High" };
    if (!priorities.Contains(request.Priority))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["priority"] = ["Use Low, Medium ou High."]
        });

    var item = new WorkItem
    {
        Id = Guid.NewGuid(),
        Title = request.Title.Trim(),
        Description = request.Description?.Trim() ?? "",
        Priority = request.Priority,
        Status = "Todo",
        DueDate = request.DueDate,
        CreatedAt = DateTimeOffset.UtcNow
    };

    db.WorkItems.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{item.Id}", item);
});

app.MapPut("/api/tasks/{id:guid}", async (Guid id, UpdateWorkItemRequest request, TaskBoardDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = ["O título é obrigatório."]
        });

    var item = await db.WorkItems.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Title = request.Title.Trim();
    item.Description = request.Description?.Trim() ?? "";
    item.Priority = request.Priority;
    item.DueDate = request.DueDate;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapPatch("/api/tasks/{id:guid}/status", async (Guid id, ChangeWorkItemStatusRequest request, TaskBoardDbContext db) =>
{
    var statuses = new[] { "Todo", "InProgress", "Blocked", "Done" };
    if (!statuses.Contains(request.Status))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["status"] = ["Use Todo, InProgress, Blocked ou Done."]
        });

    var item = await db.WorkItems.FindAsync(id);
    if (item is null) return Results.NotFound();
    item.Status = request.Status;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/api/tasks/{id:guid}", async (Guid id, TaskBoardDbContext db) =>
{
    var item = await db.WorkItems.FindAsync(id);
    if (item is null) return Results.NotFound();
    db.WorkItems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/api/tasks/summary", async (TaskBoardDbContext db) =>
{
    var tasks = await db.WorkItems.AsNoTracking().ToListAsync();
    return Results.Ok(new
    {
        total = tasks.Count,
        todo = tasks.Count(item => item.Status == "Todo"),
        inProgress = tasks.Count(item => item.Status == "InProgress"),
        blocked = tasks.Count(item => item.Status == "Blocked"),
        done = tasks.Count(item => item.Status == "Done"),
        overdue = tasks.Count(item => item.DueDate < DateTime.UtcNow && item.Status != "Done")
    });
});

app.Run();

public sealed class TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options) : DbContext(options)
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
}

public sealed class WorkItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Todo";
    public DateTime? DueDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed record CreateWorkItemRequest(string Title, string? Description, string Priority, DateTime? DueDate);
public sealed record UpdateWorkItemRequest(string Title, string? Description, string Priority, DateTime? DueDate);
public sealed record ChangeWorkItemStatusRequest(string Status);
