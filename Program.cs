using EventifyAPI.Application.Interfaces;
using EventifyAPI.Application.Services;
using EventifyAPI.Infrastructure.Data;
using EventifyAPI.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddJsonFile("appsettings.Production.json", optional: false, reloadOnChange: true);
}

// CORS para Frontend (ACTUALIZADO para producción)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://localhost:3000",
            "https://eventify-frontend.vercel.app",
            "https://eventify-frontend.azurewebsites.net"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Add services to container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Eventify API",
        Version = "v1",
        Description = "API para la gestión de eventos - Eventify"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// EF Core
builder.Services.AddDbContext<EventifyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(EventifyAPI.Application.Services.MappingProfile));

// Repositories (Scoped)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IBoletoRepository, BoletoRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();

// Services (Scoped)
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IBoletoService, BoletoService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSettings:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("JwtSettings:SecretKey no encontrada en appsettings.json");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("OrganizadorOnly", policy => policy.RequireRole("Organizador"));
    options.AddPolicy("CompradorOnly", policy => policy.RequireRole("Comprador"));
    options.AddPolicy("LoggedIn", policy => policy.RequireAuthenticatedUser());
});

// IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure pipeline (ACTUALIZADO para producción)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // En producción
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eventify API v1");
        c.RoutePrefix = "api/docs"; // Swagger en ruta específica
    });

    // Manejo de errores en producción
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Endpoint para errores
app.Map("/error", (HttpContext context) =>
{
    var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
    return Results.Problem(
        title: "Error en el servidor",
        detail: app.Environment.IsDevelopment() ? exceptionHandler?.Error.Message : "Ocurrió un error interno",
        statusCode: StatusCodes.Status500InternalServerError
    );
});

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();