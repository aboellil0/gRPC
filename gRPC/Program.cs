using gRPC.data;
using gRPC.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//////////////
///

// Configure Kestrel to support HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2; // Support both HTTP/1.1 and HTTP/2
    });

    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2; // Explicitly enable HTTP/2 for HTTPS
        listenOptions.UseHttps(); // HTTPS is required for gRPC
    });
});


builder.Services.AddScoped<ToDoService>();
builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Conn")));
builder.Services.AddLogging();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

///
////////////////
var app = builder.Build();

app.MapGrpcService<ToDoService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "This server supports gRPC over HTTP/2.");

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
