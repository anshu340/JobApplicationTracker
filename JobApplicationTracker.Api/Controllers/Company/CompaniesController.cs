using JobApplicationTracker.Api.ApiResponses;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.Company;

[Route("api/companyies")]
public class CompaniesController(ICompaniesRepository companyService, IRegistrationService registrationService, IWebHostEnvironment environment) : ControllerBase
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    [HttpGet]
    [Route("/getallcompanies")]
    public async Task<IActionResult> GetAllCompanies()
    {
        var company = await companyService.GetAllCompaniesAsync();
        return Ok(company);
    }

    [HttpGet]
    [Route("/getcompanybyid")]
    public async Task<IActionResult> GetCompanyById(int id)
    {
        var company = await companyService.GetCompaniesByIdAsync(id);
        if (company == null)
        {
            return NotFound();
        }
        return Ok(company);
    }

    [HttpDelete]
    [Route("/deletecompany")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var response = await companyService.DeleteCompanyAsync(id);
        if (response is ResponseDto responseDto)
        {
            return responseDto.IsSuccess ? Ok(responseDto) : BadRequest(responseDto);
        }
        return BadRequest("Invalid response type.");
    }

    [HttpPost]
    [Route("/submitcompany")]
    public async Task<IActionResult> SubmitCompany([FromBody] CompaniesDataModel companyDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await companyService.SubmitCompaniesAsync(companyDto);
        if (response.IsSuccess)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpPost]
    [Route("/uploadcompanylogo/{companyId}")]
    public async Task<IActionResult> UploadCompanyLogo(int companyId, IFormFile logoFile)
    {
        try
        {
            // Validate file
            if (logoFile == null || logoFile.Length == 0)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "No file uploaded."
                });
            }

            // Check file size
            if (logoFile.Length > MaxFileSize)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "File size exceeds 5MB limit."
                });
            }

            // Check file extension
            var fileExtension = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid file type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed."
                });
            }

            // Check if company exists
            var existingCompany = await companyService.GetCompaniesByIdAsync(companyId);
            if (existingCompany == null)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Company not found."
                });
            }

            // Create images/companylogo directory if it doesn't exist
            var uploadsPath = Path.Combine(environment.WebRootPath, "images", "companylogo");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Delete old logo file if exists
            if (!string.IsNullOrEmpty(existingCompany.CompanyLogo))
            {
                var oldLogoPath = GetPhysicalPathFromUrl(existingCompany.CompanyLogo);
                if (System.IO.File.Exists(oldLogoPath))
                {
                    System.IO.File.Delete(oldLogoPath);
                }
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logoFile.CopyToAsync(stream);
            }

            // Generate URL
            var logoUrl = $"{Request.Scheme}://{Request.Host}/images/companylogo/{fileName}";

            // Update company with new logo URL
            var updateResponse = await companyService.UploadCompanyLogoAsync(companyId, logoUrl);

            if (updateResponse.IsSuccess)
            {
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Company logo uploaded successfully.",
                    Data = new { LogoUrl = logoUrl }
                });
            }
            else
            {
                // Delete uploaded file if database update failed
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return BadRequest(updateResponse);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = $"An error occurred while uploading the logo: {ex.Message}"
            });
        }
    }

    [HttpDelete]
    [Route("/deletecompanylogo/{companyId}")]
    public async Task<IActionResult> DeleteCompanyLogo(int companyId)
    {
        try
        {
            // Check if company exists
            var existingCompany = await companyService.GetCompaniesByIdAsync(companyId);
            if (existingCompany == null)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Company not found."
                });
            }

            // Delete logo file if exists
            if (!string.IsNullOrEmpty(existingCompany.CompanyLogo))
            {
                var logoPath = GetPhysicalPathFromUrl(existingCompany.CompanyLogo);
                if (System.IO.File.Exists(logoPath))
                {
                    System.IO.File.Delete(logoPath);
                }
            }

            // Update company to remove logo URL
            var updateResponse = await companyService.UploadCompanyLogoAsync(companyId, null);

            if (updateResponse.IsSuccess)
            {
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Company logo deleted successfully."
                });
            }
            else
            {
                return BadRequest(updateResponse);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = $"An error occurred while deleting the logo: {ex.Message}"
            });
        }
    }

    private string GetPhysicalPathFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return string.Empty;

        try
        {
            // Extract the relative path from the URL
            var uri = new Uri(url);
            var relativePath = uri.AbsolutePath.TrimStart('/');

            // Convert to physical path
            return Path.Combine(environment.WebRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        }
        catch
        {
            return string.Empty;
        }
    }
    [HttpGet]
    [Route("/getcompanylogo/{companyId}")]
    public async Task<IActionResult> GetCompanyLogo(int companyId)
    {
        try
        {
            var logoUrl = await companyService.GetCompanyLogoAsync(companyId);

            if (logoUrl == null)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Company not found or no logo available."
                });
            }

            if (string.IsNullOrEmpty(logoUrl))
            {
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Company found but no logo uploaded.",
                    Data = new { LogoUrl = (string?)null }
                });
            }

            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Message = "Company logo retrieved successfully.",
                Data = new { LogoUrl = logoUrl }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = $"An error occurred while retrieving the company logo: {ex.Message}"
            });
        }
    }
}