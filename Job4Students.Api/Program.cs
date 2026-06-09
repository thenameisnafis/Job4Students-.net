using Job4Students.Data;
using Job4Students.Repos;
using Job4Students.Shared;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<JobStudentDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("JobStudentDbContext")));

builder.Services.AddScoped<AuthRepo>();
builder.Services.AddScoped<JobRepo>();
builder.Services.AddScoped<ApplicationRepo>();
builder.Services.AddScoped<StudentRepo>();
builder.Services.AddScoped<EmployerRepo>();
builder.Services.AddScoped<AdminRepo>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserHelper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
