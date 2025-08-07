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
using System.Security.Claims;
using System.Text;
using JobApplicationTracker.Data.Repository;


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

var jwtSettings = builder.Configuration.GetSection("jwtSettings").Get<JwtSettings>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
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
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//    .AddJwtBearer(options =>
//    {
//        var jwtSettings = builder.Configuration.GetSection("jwtSettings").Get<JwtSettings>();

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateActor = true,
//            ValidateIssuer = true,
//            ValidAudience = jwtSettings?.Audience,
//            ValidIssuer = jwtSettings?.Issuer,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key ?? "")),
//            ValidateIssuerSigningKey = true,

//            ClockSkew = TimeSpan.Zero,
//        };

//    })
//    .AddCookie(options =>
//    {
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//        options.AccessDeniedPath = "/";
//        options.LogoutPath = "/";
//        options.Cookie.HttpOnly = false;
//        options.Cookie.SameSite = SameSiteMode.None;
//        options.Cookie.IsEssential = true;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    });



// Add service layer dependency
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IJobSeekersEducationRepository, JobSeekerEducationRepository>();

builder.Services.AddScoped<IJobSeekerSkillRepository, JobSeekerSkillsRepository>();
builder.Services.AddScoped<ISkillsRepository, SkillsRepository>();

// In your Program.cs, add this AFTER the AddServiceLayer call:

// Calling the extension method to register all services from Service and Data layers


// Calling the extension method to register all services from Service and Data layers
builder.Services.AddServiceLayer(builder.Configuration);

builder.Services.AddScoped<IUsersEducationRepository, UsersEducationRepository>();


builder.Services.AddScoped<IJobsRepository, JobRepository>();



// add global exception handler service
// builder.Services.AddExceptionHandler<AppExceptionHandler>();
// enable the exception handler service early in the pipeline with default options
// app.UseExceptionHandler(_ => { });

var app = builder.Build();
app.UseCors("AllowAllOrigins");
app.UseStaticFiles(); // make sure this is added


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