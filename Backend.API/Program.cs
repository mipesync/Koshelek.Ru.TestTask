using Backend.API.Middlewares;
using Backend.Application;
using Backend.Application.Handlers;
using Backend.DAL;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDataAccessLayer(builder.Configuration.GetConnectionString("PostgreSQL"));
builder.Services.AddBusinessLogicLayer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Messages API", Version = "v1" });
    
    var subProjectAssemblies = AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(u => u.FullName!.Contains("Backend.API")).ToArray();

    foreach (var subProject in subProjectAssemblies )
    {
        var xmlFile = $"{subProject.GetName().Name}.xml";
        var assemblyRootPath = Directory.GetParent(subProject.Location)!.FullName;
        var xmlPath = Path.Combine(assemblyRootPath, xmlFile);
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5190") // URL вашего фронтенда
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var initializer = services.GetRequiredService<DatabaseInitializer>();
        initializer.EnsureDatabaseCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseAuthorization();

app.UseExceptionMiddleware();

app.UseWebSockets();
app.UseRouting();

app.MapControllers();
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        await webSocketHandler.HandleWebSocketAsync(webSocket, context.Connection.Id);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();