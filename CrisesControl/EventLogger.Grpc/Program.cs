using EventLogger.Core.AuditLog.Services;
using EventLogger.Core.Mongo.Repositories;
using EventLogger.Core.Mongo.Settings;
using EventLogger.Grpc.Services;
using EventLogger.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuditLogMongoOptions>(
    builder.Configuration.GetSection(AuditLogMongoOptions.AuditLogMongo));

builder.Services.AddTransient<IAuditLogService, EventLogger.Infrastructure.Services.AuditLogService>();
builder.Services.AddTransient<IMongoRepositoryFactory, MongoRepositoryFactory>();

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuditLogService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
