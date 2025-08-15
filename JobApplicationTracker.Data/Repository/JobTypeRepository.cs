using Dapper;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using System.Data;

namespace JobApplicationTracker.Data.Repository;

public class JobTypeRepository : IJobTypeRepository
{
    private readonly IDatabaseConnectionService _connectionService;

    public JobTypeRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<IEnumerable<JobTypesDataModel>> GetAllJobTypesAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        var sql = """
                  SELECT JobTypeId, 
                         Name 
                  FROM JobType
                  """;
        return await connection.QueryAsync<JobTypesDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<JobTypesDataModel> GetJobTypeByIdAsync(int jobTypeId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        var sql = """
                  SELECT JobTypeId, 
                         Name, 
                  FROM JobType
                  WHERE JobTypeId = @JobTypeId
                  """;
        var parameters = new DynamicParameters();
        parameters.Add("@JobTypeId", jobTypeId, DbType.Int32);
        return await connection.QueryFirstOrDefaultAsync<JobTypesDataModel>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitJobTypeAsync(JobTypesDataModel jobTypeDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        if (jobTypeDto.JobTypeId <= 0)
        {
            // Insert new job type and get the generated ID
            var sql = """
                INSERT INTO JobType (Name)
                VALUES (@Name);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            var parameters = new DynamicParameters();
            parameters.Add("@Name", jobTypeDto.Name, DbType.String);

            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Job type submitted successfully.",
                Id = newId
            };
        }
        else
        {
            // Update existing job type
            var sql = """
                UPDATE JobType
                SET Name = @Name
                WHERE JobTypeId = @JobTypeId
                """;

            var parameters = new DynamicParameters();
            parameters.Add("@JobTypeId", jobTypeDto.JobTypeId, DbType.Int32);
            parameters.Add("@Name", jobTypeDto.Name, DbType.String);

            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                Message = affectedRows > 0 ? "Job type updated successfully." : "Failed to update job type.",
                Id = jobTypeDto.JobTypeId
            };
        }
    }

    public async Task<ResponseDto> DeleteJobTypeAsync(int jobTypeId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """DELETE FROM JobType WHERE JobTypeId = @JobTypeId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobTypeId", jobTypeId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete job type."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Job type deleted successfully.",
            Id = jobTypeId
        };
    }
}