using RoletaBrindes.Application.Services;
using RoletaBrindes.Infrastructure.Data;
using RoletaBrindes.Infrastructure.Repositories;
using RoletaBrindes.Infrastructure.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionFactory, PgConnectionFactory>();

builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<ISpinRepository, SpinRepository>();

builder.Services.AddScoped<SpinService>();

builder.Services.AddCors(
    o => o.AddPolicy("public", 
        p => 
            p.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("public");
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();