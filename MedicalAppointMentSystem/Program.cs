using MedicalAppointMentSystem.Data;
using MedicalAppointMentSystem.DTOs.EmailDtos;
using MedicalAppointMentSystem.EmailServices;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connection")));

builder.Services.AddCors(settings =>
{
    settings.AddPolicy("MedicalSystemPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200");
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();

    });
});
builder.Services.AddOpenApi();
QuestPDF.Settings.License = LicenseType.Community;
// In Program.cs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("MedicalSystemPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
