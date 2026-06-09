using Job4Students.Data;
using Job4Students.Repos;
using Job4Students.Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<JobStudentDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("JobStudentDbContext")));

builder.Services.AddScoped<AuthRepo>();
builder.Services.AddScoped<JobRepo>();
builder.Services.AddScoped<ApplicationRepo>();
builder.Services.AddScoped<StudentRepo>();
builder.Services.AddScoped<EmployerRepo>();
builder.Services.AddScoped<AdminRepo>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("JobAuth")
    .AddCookie("JobAuth", opt =>
    {
        opt.AccessDeniedPath = "/Auth/Denied";
        opt.LoginPath = "/Auth/Login";
        opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserHelper>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
