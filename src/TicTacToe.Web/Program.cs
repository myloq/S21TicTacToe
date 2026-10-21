using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicTacToe.DI;
using TicTacToe.Web.Authenticator;
using TicTacToe.Web.ExceptionHandling;
using TicTacToe.Web.Controller;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddOpenApi();

builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, UserAuthenticator>("Basic", null);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.Use(async (ctx, next) =>
{
    var t0 = System.Diagnostics.Stopwatch.GetTimestamp();
    await next();
    var ms = (System.Diagnostics.Stopwatch.GetTimestamp() - t0) * 1000.0
             / System.Diagnostics.Stopwatch.Frequency;
    Console.WriteLine($"{ctx.Request.Method} {ctx.Request.Path} -> {ctx.Response.StatusCode} in {ms:F0}ms");
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();