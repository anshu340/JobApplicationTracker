using JobApplicationTracker.Data.Config;
using JobApplicationTracker.Data.Repository;
using JobApplicationTracke.Data.Interface;
using JobApplicationTracke.Data.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JobApplicationTracker.Service
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // This line is correct, provided Microsoft.Extensions.Options is used
            services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));

            services.AddScoped<ICompaniesRepository, CompaniesRepository>();
            services.AddScoped<IIndustriesRepository, IndustriesRepository>();
            services.AddScoped<IAdminActionRepository, AdminActionService>();
            services.AddScoped<ICompaniesRepository, CompaniesRepository>(); // Duplicate
            services.AddScoped<IIndustriesRepository, IndustriesRepository>(); // Duplicate
            services.AddScoped<IJobApplicationRepository, ApplicationsRepository>();
            services.AddScoped<IJobSeekerExperienceRepository, JobSeekerExperienceRepository>();
            services.AddScoped<IJobSeekersRepository, JobSeekerRepository>();
            services.AddScoped<IJobSeekersSkillsRepository, JobSeekerSkillsRepository>();
            services.AddScoped<ISkillsRepository, SkillsRepository>();
            services.AddScoped<IUserRepository, UsersRepository>();

            services.AddScoped<IDatabaseConnectionService, DatabaseConnectionService>();

            return services;
        }
    }
}