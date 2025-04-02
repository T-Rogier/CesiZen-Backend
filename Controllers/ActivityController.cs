using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/activities")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService; // Assure-toi d'avoir ce service ou adaptes-le

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto command) // Remplace par le DTO approprié
        {
            var activity = await _activityService.CreateActivityAsync(command); // Adapte la méthode à ton service
            return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _activityService.GetAllActivitiesAsync(); // Adapte la méthode à ton service
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id); // Adapte la méthode à ton service
            return activity is null ? NotFound(new { Message = $"Activity with ID {id} not found." }) : Ok(activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityDto command) // Remplace par le DTO approprié
        {
            await _activityService.UpdateActivityAsync(id, command); // Adapte la méthode à ton service
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            await _activityService.DeleteActivityAsync(id); // Adapte la méthode à ton service
            return NoContent();
        }
    }
}
