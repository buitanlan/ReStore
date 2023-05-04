using System.Text;
using API.Data;
using API.Entities;
using API.Middlewares;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put Bearer + your token in the box below",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme, Array.Empty<string>()
        }
    });
});
builder.Services.AddCors();
builder.Services.AddIdentityCore<User>(opt => 
{
    opt.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<StoreContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt => 
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.
            GetBytes(builder.Configuration["JWTSettings:TokenKey"]))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();
builder.Services.AddNpgsql<StoreContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());

var app = builder.Build();
using var scope = app.Services.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
try
{
    await context.Database.MigrateAsync();
    await DbInitializer.Initialize(context, userManager);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Problem migrating data");
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5173"));

app.MapControllers();

await app.RunAsync();
