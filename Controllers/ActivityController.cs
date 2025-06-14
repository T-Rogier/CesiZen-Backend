using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Services.ActivityService;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/activities")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequestDto command)
        {
            FullActivityResponseDto activity = await _activityService.CreateActivityAsync(command);
            return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            ActivityListResponseDto activities = await _activityService.GetAllActivitiesAsync();
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            FullActivityResponseDto? activity = await _activityService.GetActivityByIdAsync(id);
            return activity is null ? NotFound(new { Message = $"Activity with ID {id} not found." }) : Ok(activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityRequestDto command)
        {
            await _activityService.UpdateActivityAsync(id, command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            await _activityService.DeleteActivityAsync(id);
            return NoContent();
        }
    }
}
