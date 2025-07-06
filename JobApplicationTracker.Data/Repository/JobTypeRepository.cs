using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

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
                         TypeName, 
                         Description 
                  FROM JobTypes
                  """;

        return await connection.QueryAsync<JobTypesDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<JobTypesDataModel> GetJobTypeByIdAsync(int jobTypeId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        // write the SQL query to fetch a job type by ID
        var sql = """
                  SELECT JobTypeId, 
                         TypeName, 
                         Description 
                  FROM JobTypes
                  WHERE JobTypeId = ?
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobTypeId", jobTypeId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobTypesDataModel>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitJobTypeAsync(JobTypesDataModel jobTypeDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (jobTypeDto.JobTypeId <= 0)
        {
            // Insert new job type (assumes JobTypeId is auto-incremented)
            sql = """
            INSERT INTO JobTypes (TypeName, Description)
            VALUES (@TypeName, @Description)
            """;
        }
        else
        {
            // Update existing job type
            sql = """
            UPDATE JobTypes
            SET TypeName = TypeName,
                Description = Description
            WHERE JobTypeId = JobTypeId
            """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("JobTypeId", jobTypeDto.JobTypeId, DbType.Int32);
        parameters.Add("TypeName", jobTypeDto.Name, DbType.String);
        parameters.Add("Description", jobTypeDto.Description, DbType.String);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Jobs type submitted successfully." : "Failed to submit job type."
        };
    }



    public async Task<ResponseDto> DeleteJobTypeAsync(int jobTypeId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        // write the SQL query to delete a job type by ID
        var sql = """DELETE FROM JobTypes WHERE JobTypeId = JobTypeId""";

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
            Message = "Jobs type deleted successfully."
        };
    }
}
