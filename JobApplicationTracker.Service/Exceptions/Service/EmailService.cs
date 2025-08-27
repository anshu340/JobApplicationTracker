using JobApplicationTracker.Business.Interface;
using JobApplicationTracker.Data.Dto.Requests;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace JobApplicationTracker.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendJobNotificationEmailAsync(UsersDtoResponse user, JobDto job)
        {
            try
            {
                var smtpHost = _configuration["Smtp:Host"];
                var smtpPort = int.Parse(_configuration["Smtp:Port"]);
                var smtpUser = _configuration["Smtp:User"];
                var smtpPass = _configuration["Smtp:Pass"];
                var fromEmail = _configuration["Smtp:From"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };

                var subject = $"New Job Opportunity: {job.JobType}";
                var body = $@"
                    Hi {user.FirstName},

                    We found a new job that matches your profile!

                    Position: {job.JobType}
                    Location: {job.Location}
                    Type: {job.EmpolymentType}
                    Salary: {job.SalaryRangeMin:C0} - {job.SalaryRangeMax:C0}

                    Click here to view more details: https://yourapp.com/jobs/{job.JobId}/details
                ";

                var mailMessage = new MailMessage(fromEmail, user.Email, subject, body);

                await client.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email to {user.Email}: {ex.Message}");
                return false;
            }
        }
    }
}
