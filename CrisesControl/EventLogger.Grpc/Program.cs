using EventLogger.Core.AuditLog.Services;
using EventLogger.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IAuditLogService, EventLogger.Infrastructure.Services.AuditLogService>();

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuditLogService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
