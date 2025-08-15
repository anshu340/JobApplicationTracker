
using JobApplicationTracker.Api.ApiResponses;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.Experience
{
    [Route("api/experiences")]
    public class ExperienceController(IExperienceRepository experienceService) : ControllerBase
    {
        [HttpGet]
        [Route("/getallexperiences")]
        public async Task<IActionResult> GetAllExperiences()
        {
            var experiences = await experienceService.GetAllExperiencesAsync();
            return Ok(experiences);
        }

        [HttpGet]
        [Route("/getexperiencebyid")]
        public async Task<IActionResult> GetExperienceById(int id)
        {
            var experience = await experienceService.GetExperienceByIdAsync(id);
            if (experience == null)
            {
                return NotFound();
            }
            return Ok(experience);
        }

        [HttpPost]
        [Route("/submitexperience")]
        public async Task<IActionResult> SubmitExperience([FromBody] ExperienceDataModel experienceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await experienceService.SubmitExperienceAsync(experienceDto);
            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpDelete]
        [Route("/deleteexperience")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var response = await experienceService.DeleteExperienceAsync(id);
            if (response is ResponseDto responseDto)
            {
                return responseDto.IsSuccess ? Ok(responseDto) : BadRequest(responseDto);
            }
            return BadRequest("Invalid response type.");
        }
    }
}
