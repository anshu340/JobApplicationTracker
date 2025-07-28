using JobApplicationTracke.Data.Database;
using JobApplicationTracker.Api.GlobalExceptionHandler;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Data.Repository;
using JobApplicationTracker.Service;
using JobApplicationTracker.Service.Configuration;
using JobApplicationTracker.Service.Services.Interfaces;
using JobApplicationTracker.Service.Services.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(config =>
{
    config.Filters.AddService<GlobalExceptionHandler>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// registering GlobalExceptionHandler as a service as it has ILogger as a dependency    
builder.Services.AddScoped<GlobalExceptionHandler>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("jwtSettings"));
builder.Services.AddEndpointsApiExplorer();

// ✅ Register your repository & DB connection service
builder.Services.AddScoped<IJobsRepository, JobRepository>();
builder.Services.AddScoped<IDatabaseConnectionService, DatabaseConnectionService>();


builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter Token",
        Name = "Token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// adding the cors policy for all origins default.
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAllOrigins",
        options => options.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials())
);

// authentication service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("jwtSettings").Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = true,
            ValidateIssuer = true,
            ValidAudience = jwtSettings?.Audience,
            ValidIssuer = jwtSettings?.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key ?? "")),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
        };

    })
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/";
        options.LogoutPath = "/";
        options.Cookie.HttpOnly = false;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Add service layer dependency
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

// Calling the extension method to register all services from Service and Data layers
builder.Services.AddServiceLayer(builder.Configuration);

var app = builder.Build();
app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Jobs Applications Tracker API V1");
        options.RoutePrefix = string.Empty; // This makes Swagger UI the root
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
