using JobApplicationTracker.Api.ApiResponses;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.Company;

[Route("api/companyies")]
public class CompaniesController(ICompaniesRepository companyService, IRegistrationService registrationService) : ControllerBase
{
    
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
        if (response is ResponseDto responseDto) // Ensure the response is cast to the correct type
        {
            return responseDto.IsSuccess ? Ok(responseDto) : BadRequest(responseDto);
        }
        return BadRequest("Invalid response type.");
    }
}