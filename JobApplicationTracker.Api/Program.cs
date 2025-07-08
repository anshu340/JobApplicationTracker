using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using JobApplicationTracker.Api.GlobalExceptionHandler;

using JobApplicationTracker.Service;
using JobApplicationTracker.Service.Configuration;
using JobApplicationTracker.Service.Services.Interfaces;
using JobApplicationTracker.Service.Services.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(config =>
{
    config.Filters.AddService<GlobalExceptionHandler>();
});

// registering GlobalExceptionHandler as a service as it has ILogger as a dependency    
builder.Services.AddScoped<GlobalExceptionHandler>(); 
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("jwtSettings"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
        options.Cookie.HttpOnly = true;
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

// add global exception handler service
// builder.Services.AddExceptionHandler<AppExceptionHandler>();
// enable the exception handler service early in the pipeline with default options
// app.UseExceptionHandler(_ => { });

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
app.MapControllers();
app.Run();