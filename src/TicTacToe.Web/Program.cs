using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TicTacToe.DI;
using TicTacToe.Domain.Model;
using TicTacToe.Web.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройки БД, сервисы
builder.Services.ConfigureServices(builder.Configuration);

// 2. Настройки Web-слоя
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddOpenApi();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));

var jwtOptions = builder.Configuration.GetSection("JwtSettings").Get<JwtOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
    };
});

var app = builder.Build();

// 3. Middleware Pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

// логгер запросов
app.Use(async (ctx, next) =>
{
    var t0 = System.Diagnostics.Stopwatch.GetTimestamp();
    await next();
    var ms = (System.Diagnostics.Stopwatch.GetTimestamp() - t0) * 1000.0
             / System.Diagnostics.Stopwatch.Frequency;
    Console.WriteLine($"{ctx.Request.Method} {ctx.Request.Path} -> {ctx.Response.StatusCode} in {ms:F0}ms");
});

// Регистрируем сам провайдер
// builder.Services.AddScoped<IJwtProvider, JwtProvider>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();