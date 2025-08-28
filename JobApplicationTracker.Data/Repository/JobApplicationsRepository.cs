using Dapper;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JobApplicationTracker.Data.Repository;

public class ApplicationsRepository : IJobApplicationRepository
{
    private readonly IDatabaseConnectionService _connectionService;

    public ApplicationsRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    // Add method to validate UserId exists
    private async Task<bool> UserExistsAsync(int userId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = "SELECT COUNT(1) FROM Users WHERE UserId = @UserId";
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.Int32);

        var count = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);

        // Debug logging
        Console.WriteLine($"Checking UserId {userId}: Found {count} records");

        return count > 0;
    }

    // Add method to validate JobId exists
    private async Task<bool> JobExistsAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = "SELECT COUNT(1) FROM Job WHERE JobId = @JobId";
        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobId, DbType.Int32);

        var count = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);

        // Debug logging
        Console.WriteLine($"Checking JobId {jobId}: Found {count} records");

        return count > 0;
    }

    // FIXED: Add method to validate CompanyId exists - changed from 'Company' to 'Companies'
    private async Task<bool> CompanyExistsAsync(int companyId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = "SELECT COUNT(1) FROM Companies WHERE CompanyId = @CompanyId";
        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companyId, DbType.Int32);

        var count = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);

        // Debug logging
        Console.WriteLine($"Checking CompanyId {companyId}: Found {count} records");

        return count > 0;
    }

    // Add method to validate ApplicationStatus exists
    private async Task<bool> ApplicationStatusExistsAsync(int statusId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = "SELECT COUNT(1) FROM ApplicationStatus WHERE ApplicationStatusId = @StatusId";
        var parameters = new DynamicParameters();
        parameters.Add("@StatusId", statusId, DbType.Int32);

        var count = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);

        // Debug logging
        Console.WriteLine($"Checking ApplicationStatus {statusId}: Found {count} records");

        // If not found, get available statuses for debugging
        if (count == 0)
        {
            var availableStatuses = await connection.QueryAsync<dynamic>(
                "SELECT ApplicationStatusId, StatusName FROM ApplicationStatus").ConfigureAwait(false);
            Console.WriteLine($"Available ApplicationStatus records: {string.Join(", ", availableStatuses.Select(s => $"{s.ApplicationStatusId}:{s.StatusName}"))}");
        }

        return count > 0;
    }

    public async Task<IEnumerable<ApplicationsDataModel>> GetAllJobApplicationAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
              SELECT ApplicationId, 
                     JobId, 
                     UserId, 
                     ApplicationStatus,
                     ApplicationDate,
                     CoverLetter,
                     ResumeFile,
                     SalaryExpectation,
                     AvailableStartDate,
                     CreatedAt
              FROM JobApplications
              """;

        return await connection.QueryAsync<ApplicationsDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<ApplicationsDataModel> GetJobApplicationByIdAsync(int jobApplicationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT   ApplicationId,
                           JobId,
                           UserId,
                           ApplicationStatus,
                           ApplicationDate,
                           CoverLetter,
                           ResumeFile,
                           SalaryExpectation,
                           AvailableStartDate,
                           CreatedAt
                  FROM JobApplications
                  WHERE ApplicationId = @JobApplicationId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobApplicationId", jobApplicationId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<ApplicationsDataModel>(sql, parameters).ConfigureAwait(false);
    }

    // NEW METHOD: Get job applications by UserId (following SkillsRepository pattern)
    public async Task<IEnumerable<ApplicationsDataModel>> GetJobApplicationsByUserIdAsync(int userId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT ApplicationId,
                         JobId,
                         UserId,
                         ApplicationStatus,
                         ApplicationDate,
                         CoverLetter,
                         ResumeFile,
                         SalaryExpectation,
                         AvailableStartDate,
                         CreatedAt
                  FROM JobApplications
                  WHERE UserId = @UserId
                  ORDER BY ApplicationDate DESC
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.Int32);

        return await connection.QueryAsync<ApplicationsDataModel>(sql, parameters).ConfigureAwait(false);
    }

    // FIXED: Get applications by CompanyId - now using 'Companies' table consistently
    public async Task<IEnumerable<ApplicationsDataModel>> GetApplicationsByCompanyIdAsync(int companyId)
    {
        // First validate that the company exists
        if (!await CompanyExistsAsync(companyId))
        {
            return Enumerable.Empty<ApplicationsDataModel>();
        }

        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT ja.ApplicationId,
                         ja.JobId,
                         ja.UserId,
                         ja.ApplicationStatus,
                         ja.ApplicationDate,
                         ja.CoverLetter,
                         ja.ResumeFile,
                         ja.SalaryExpectation,
                         ja.AvailableStartDate,
                         ja.CreatedAt
                  FROM JobApplications ja
                  INNER JOIN Job j ON ja.JobId = j.JobId
                  INNER JOIN Companies c ON j.CompanyId = c.CompanyId
                  WHERE c.CompanyId = @CompanyId
                  ORDER BY ja.ApplicationDate DESC
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companyId, DbType.Int32);

        try
        {
            var applications = await connection.QueryAsync<ApplicationsDataModel>(sql, parameters).ConfigureAwait(false);

            // Debug logging
            Console.WriteLine($"Found {applications.Count()} applications for CompanyId {companyId}");

            return applications;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting applications for CompanyId {companyId}: {ex.Message}");
            throw;
        }
    }

    // UPDATED: Modified to follow SkillsRepository pattern for insert/update logic
    public async Task<ResponseDto> SubmitJobApplicationAsync(ApplicationsDataModel jobApplicationDto)
    {
        // Validate input
        if (jobApplicationDto == null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Job application data cannot be null."
            };
        }

        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // Check if ApplicationId exists (similar to SkillsRepository pattern)
        var existsQuery = "SELECT COUNT(1) FROM JobApplications WHERE ApplicationId = @ApplicationId";
        var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new { jobApplicationDto.ApplicationId });

        // Set default ApplicationStatus to "Applied" if not provided or invalid
        if (jobApplicationDto.ApplicationStatus <= 0)
        {
            jobApplicationDto.ApplicationStatus = 1; // Default to "Applied"
            Console.WriteLine("ApplicationStatus was 0 or negative, defaulting to 1 (Applied)");
        }

        // Debug: Log the values being submitted
        Console.WriteLine($"Submitting job application - UserId: {jobApplicationDto.UserId}, JobId: {jobApplicationDto.JobId}, ApplicationStatus: {jobApplicationDto.ApplicationStatus}");

        // Validate required foreign key references
        if (!await UserExistsAsync(jobApplicationDto.UserId))
        {
            await using var userConnection = await _connectionService.GetDatabaseConnectionAsync();
            var availableUserIds = await userConnection.QueryAsync<int>("SELECT TOP 5 UserId FROM Users ORDER BY UserId").ConfigureAwait(false);
            var userIdList = string.Join(", ", availableUserIds);

            return new ResponseDto
            {
                IsSuccess = false,
                Message = $"User with ID {jobApplicationDto.UserId} does not exist. Available UserIds: {userIdList}"
            };
        }

        if (!await JobExistsAsync(jobApplicationDto.JobId))
        {
            await using var jobConnection = await _connectionService.GetDatabaseConnectionAsync();
            var availableJobIds = await jobConnection.QueryAsync<int>("SELECT TOP 5 JobId FROM Job ORDER BY JobId").ConfigureAwait(false);
            var jobIdList = string.Join(", ", availableJobIds);

            return new ResponseDto
            {
                IsSuccess = false,
                Message = $"Job with ID {jobApplicationDto.JobId} does not exist. Available JobIds: {jobIdList}"
            };
        }

        // Updated validation to reflect new status options: 1 (Applied), 2 (Approve), 3 (Rejected)
        if (jobApplicationDto.ApplicationStatus < 1 || jobApplicationDto.ApplicationStatus > 3)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = $"Application status with ID {jobApplicationDto.ApplicationStatus} is not valid. Valid options are: 1 (Applied), 2 (Approve), 3 (Rejected)"
            };
        }

        if (!await ApplicationStatusExistsAsync(jobApplicationDto.ApplicationStatus))
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = $"Application status with ID {jobApplicationDto.ApplicationStatus} does not exist in database. Valid options are: 1 (Applied), 2 (Approve), 3 (Rejected)"
            };
        }

        int applicationIdResult = 0;
        int affectedRows = 0;

        try
        {
            if (jobApplicationDto.ApplicationId <= 0 || exists == 0)
            {
                // Insert new job application (following SkillsRepository pattern)
                var sql = """
                    INSERT INTO JobApplications (JobId, UserId, CoverLetter, ResumeFile, ApplicationStatus, ApplicationDate, SalaryExpectation, AvailableStartDate)
                    OUTPUT INSERTED.ApplicationId
                    VALUES (@JobId, @UserId, @CoverLetter, @ResumeFile, @ApplicationStatus, @ApplicationDate, @SalaryExpectation, @AvailableStartDate)
                    """;

                var parameters = new DynamicParameters();
                parameters.Add("@JobId", jobApplicationDto.JobId, DbType.Int32);
                parameters.Add("@UserId", jobApplicationDto.UserId, DbType.Int32);
                parameters.Add("@CoverLetter", jobApplicationDto.CoverLetter, DbType.String);
                parameters.Add("@ResumeFile", jobApplicationDto.ResumeFile, DbType.String);
                parameters.Add("@ApplicationStatus", jobApplicationDto.ApplicationStatus, DbType.Int32);
                parameters.Add("@ApplicationDate", jobApplicationDto.ApplicationDate != default ? jobApplicationDto.ApplicationDate : DateTime.UtcNow, DbType.DateTime);
                parameters.Add("@SalaryExpectation", jobApplicationDto.SalaryExpectation, DbType.Decimal);
                parameters.Add("@AvailableStartDate", jobApplicationDto.AvailableStartDate, DbType.DateTime);

                applicationIdResult = await connection.ExecuteScalarAsync<int>(sql, parameters);
                affectedRows = applicationIdResult > 0 ? 1 : 0;
            }
            else
            {
                // Update existing job application (following SkillsRepository pattern)
                var sql = """
                    UPDATE JobApplications
                    SET 
                        JobId = @JobId,
                        UserId = @UserId,
                        CoverLetter = @CoverLetter,
                        ResumeFile = @ResumeFile,
                        ApplicationStatus = @ApplicationStatus,
                        ApplicationDate = @ApplicationDate,
                        SalaryExpectation = @SalaryExpectation,
                        AvailableStartDate = @AvailableStartDate
                    WHERE ApplicationId = @ApplicationId
                    """;

                var parameters = new DynamicParameters();
                parameters.Add("@ApplicationId", jobApplicationDto.ApplicationId, DbType.Int32);
                parameters.Add("@JobId", jobApplicationDto.JobId, DbType.Int32);
                parameters.Add("@UserId", jobApplicationDto.UserId, DbType.Int32);
                parameters.Add("@CoverLetter", jobApplicationDto.CoverLetter, DbType.String);
                parameters.Add("@ResumeFile", jobApplicationDto.ResumeFile, DbType.String);
                parameters.Add("@ApplicationStatus", jobApplicationDto.ApplicationStatus, DbType.Int32);
                parameters.Add("@ApplicationDate", jobApplicationDto.ApplicationDate != default ? jobApplicationDto.ApplicationDate : DateTime.UtcNow, DbType.DateTime);
                parameters.Add("@SalaryExpectation", jobApplicationDto.SalaryExpectation, DbType.Decimal);
                parameters.Add("@AvailableStartDate", jobApplicationDto.AvailableStartDate, DbType.DateTime);

                affectedRows = await connection.ExecuteAsync(sql, parameters);
                applicationIdResult = jobApplicationDto.ApplicationId;
            }

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                StatusCode = affectedRows > 0 ? 0 : 1,
                Message = affectedRows > 0
                    ? "Job application submitted successfully."
                    : (jobApplicationDto.ApplicationId > 0 ? $"Job application with ID {jobApplicationDto.ApplicationId} not found for update." : "Failed to insert job application."),
                Id = applicationIdResult
            };
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            // More detailed error message
            var constraintName = ex.Message.Contains("FK_JobApplications_ApplicationStatus") ? "ApplicationStatus" :
                               ex.Message.Contains("FK_JobApplications_Users") ? "Users" :
                               ex.Message.Contains("FK_JobApplications_Job") ? "Job" : "Unknown";

            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = $"Foreign key constraint violation in {constraintName} table. Please ensure all referenced IDs exist. Error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = $"An error occurred while submitting the job application: {ex.Message}"
            };
        }
    }

    public async Task<ResponseDto> AcceptJobApplicationAsync(int jobApplicationId)
    {
        if (jobApplicationId <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = "Valid application ID is required."
            };
        }

        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // First check if the application exists
        var existsQuery = "SELECT COUNT(1) FROM JobApplications WHERE ApplicationId = @ApplicationId";
        var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new { ApplicationId = jobApplicationId });

        if (exists == 0)
        {
            return new ResponseDto
            {
                Id = jobApplicationId,
                IsSuccess = false,
                StatusCode = 1,
                Message = $"Job application with ID {jobApplicationId} not found."
            };
        }

        // Check current status to provide better feedback
        var currentStatusQuery = "SELECT ApplicationStatus FROM JobApplications WHERE ApplicationId = @ApplicationId";
        var currentStatus = await connection.ExecuteScalarAsync<int>(currentStatusQuery, new { ApplicationId = jobApplicationId });

        try
        {
            // Update status to Approve (using status ID 2 based on your updated ApplicationStatus table)
            var sql = """
            UPDATE JobApplications 
            SET ApplicationStatus = @ApproveStatusId
            WHERE ApplicationId = @ApplicationId
            """;

            var parameters = new DynamicParameters();
            parameters.Add("@ApplicationId", jobApplicationId, DbType.Int32);
            parameters.Add("@ApproveStatusId", 2, DbType.Int32); // Changed from 4 to 2 to match your updated ApplicationStatus

            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            if (affectedRows > 0)
            {
                return new ResponseDto
                {
                    Id = jobApplicationId,
                    IsSuccess = true,
                    StatusCode = 0,
                    Message = "Job application approved successfully."
                };
            }
            else
            {
                return new ResponseDto
                {
                    Id = jobApplicationId,
                    IsSuccess = false,
                    StatusCode = 1,
                    Message = "Failed to approve job application."
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Accept application exception: {ex.Message}");
            return new ResponseDto
            {
                Id = jobApplicationId,
                IsSuccess = false,
                StatusCode = 1,
                Message = $"An error occurred while approving the job application: {ex.Message}"
            };
        }
    }

    // Reject job application method
    public async Task<ResponseDto> RejectJobApplicationAsync(int jobApplicationId, string? rejectionReason = null)
    {
        if (jobApplicationId <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = "Valid application ID is required."
            };
        }

        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // First check if the application exists
        var existsQuery = "SELECT COUNT(1) FROM JobApplications WHERE ApplicationId = @ApplicationId";
        var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new { ApplicationId = jobApplicationId });

        if (exists == 0)
        {
            return new ResponseDto
            {
                Id = jobApplicationId,
                IsSuccess = false,
                StatusCode = 1,
                Message = $"Job application with ID {jobApplicationId} not found."
            };
        }

        try
        {
            // Update status to Rejected (using existing status ID 3)
            var sql = """
            UPDATE JobApplications 
            SET ApplicationStatus = @RejectedStatusId
            WHERE ApplicationId = @ApplicationId
            """;

            var parameters = new DynamicParameters();
            parameters.Add("@ApplicationId", jobApplicationId, DbType.Int32);
            parameters.Add("@RejectedStatusId", 3, DbType.Int32); // Status ID 3 remains the same for Rejected

            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            if (affectedRows > 0)
            {
                var message = "Job application rejected successfully.";
                if (!string.IsNullOrEmpty(rejectionReason))
                {
                    message += $" Reason: {rejectionReason}";
                    // Note: If you want to store rejection reasons, you'll need to add a RejectionReason column to your table
                }

                return new ResponseDto
                {
                    Id = jobApplicationId,
                    IsSuccess = true,
                    StatusCode = 0,
                    Message = message
                };
            }
            else
            {
                return new ResponseDto
                {
                    Id = jobApplicationId,
                    IsSuccess = false,
                    StatusCode = 1,
                    Message = "Failed to reject job application."
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Reject application exception: {ex.Message}");
            return new ResponseDto
            {
                Id = jobApplicationId,
                IsSuccess = false,
                StatusCode = 1,
                Message = $"An error occurred while rejecting the job application: {ex.Message}"
            };
        }
    }

    // Bulk approve applications method (updated to use "Approve" instead of "Accept")
    public async Task<ResponseDto> BulkApproveApplicationsAsync(IEnumerable<int> applicationIds)
    {
        if (applicationIds == null || !applicationIds.Any())
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = "At least one application ID is required."
            };
        }

        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        try
        {
            var validIds = applicationIds.Where(id => id > 0).ToList();
            if (!validIds.Any())
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    StatusCode = 1,
                    Message = "No valid application IDs provided."
                };
            }

            var sql = """
            UPDATE JobApplications 
            SET ApplicationStatus = @ApproveStatusId
            WHERE ApplicationId IN @ApplicationIds
            """;

            var parameters = new DynamicParameters();
            parameters.Add("@ApproveStatusId", 2, DbType.Int32); // Changed from 4 to 2 for "Approve"
            parameters.Add("@ApplicationIds", validIds);

            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                StatusCode = affectedRows > 0 ? 0 : 1,
                Message = affectedRows > 0
                    ? $"{affectedRows} job applications approved successfully."
                    : "No applications were updated. Please verify the application IDs exist."
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bulk approve applications exception: {ex.Message}");
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = $"An error occurred while approving job applications: {ex.Message}"
            };
        }
    }

    // Bulk reject applications method (bonus feature)
    public async Task<ResponseDto> BulkRejectApplicationsAsync(IEnumerable<int> applicationIds, string? rejectionReason = null)
    {
        if (applicationIds == null || !applicationIds.Any())
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = "At least one application ID is required."
            };
        }

        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        try
        {
            var validIds = applicationIds.Where(id => id > 0).ToList();
            if (!validIds.Any())
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    StatusCode = 1,
                    Message = "No valid application IDs provided."
                };
            }

            var sql = """
            UPDATE JobApplications 
            SET ApplicationStatus = @RejectedStatusId
            WHERE ApplicationId IN @ApplicationIds
            """;

            var parameters = new DynamicParameters();
            parameters.Add("@RejectedStatusId", 3, DbType.Int32);
            parameters.Add("@ApplicationIds", validIds);

            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            var message = affectedRows > 0
                ? $"{affectedRows} job applications rejected successfully."
                : "No applications were updated. Please verify the application IDs exist.";

            if (!string.IsNullOrEmpty(rejectionReason) && affectedRows > 0)
            {
                message += $" Reason: {rejectionReason}";
            }

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                StatusCode = affectedRows > 0 ? 0 : 1,
                Message = message
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bulk reject applications exception: {ex.Message}");
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = 1,
                Message = $"An error occurred while rejecting job applications: {ex.Message}"
            };
        }
    }


    public async Task<ResponseDto> DeleteJobApplicationAsync(int jobApplicationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """DELETE FROM JobApplications WHERE ApplicationId = @JobApplicationId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobApplicationId", jobApplicationId, DbType.Int32);

        try
        {
            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            if (affectedRows <= 0)
            {
                return new ResponseDto
                {
                    Id = jobApplicationId,
                    IsSuccess = false,
                    StatusCode = 1,
                    Message = $"Job application with ID {jobApplicationId} not found or could not be deleted."
                };
            }

            return new ResponseDto
            {
                Id = jobApplicationId,
                IsSuccess = true,
                StatusCode = 0,
                Message = "Job application deleted successfully."
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete exception: {ex.Message}");
            return new ResponseDto
            {
                Id = jobApplicationId,
                IsSuccess = false,
                StatusCode = 1,
                Message = $"An error occurred while deleting the job application: {ex.Message}"
            };
        }
    }
}