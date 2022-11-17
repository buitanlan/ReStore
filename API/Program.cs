using API.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSqlServer<StoreContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());

var app = builder.Build();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
try
{
    context.Database.Migrate();
    await DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Problem migrating data");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));

app.MapControllers();

await app.RunAsync();
