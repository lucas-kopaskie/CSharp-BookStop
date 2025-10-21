using CSharp_BookStop.API.Services;
using CSharp_BookStop.Database.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
            .WithHeaders("Content-Type", "Authorization")
            .AllowCredentials();
        });
});
builder.Services.AddAuthorization();

builder.Services.AddDbContext<BookStopContext>(options =>
    options.UseNpgsql(builder.Configuration["DefaultConnection"]));
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddRoles<IdentityRole>().
    AddEntityFrameworkStores<BookStopContext>();
builder.Services.AddScoped<IDataService, DataService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapIdentityApi<IdentityUser>();
app.Run();