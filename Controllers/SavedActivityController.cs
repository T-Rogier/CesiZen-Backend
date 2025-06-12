using CesiZen_Backend.Dtos.SavedActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.SavedActivityService;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/savedActivities")]
    public class SavedActivityController : ControllerBase
    {
        private readonly ISavedActivityService _SavedActivityService;

        public SavedActivityController(ISavedActivityService SavedActivityService)
        {
            _SavedActivityService = SavedActivityService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSavedActivity([FromBody] CreateSavedActivityDto command)
        {
            SavedActivityDto savedActivity = await _SavedActivityService.CreateSavedActivityAsync(command);
            return CreatedAtAction(nameof(GetSavedActivityByIds), new { userId = savedActivity.UserId, activityId = savedActivity.ActivityId }, savedActivity);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            IEnumerable<SavedActivityDto> SavedActivitys = await _SavedActivityService.GetAllSavedActivitiesAsync();
            return Ok(SavedActivitys);
        }

        [HttpGet("by-ids/{userId}/{activityId}")]
        public async Task<IActionResult> GetSavedActivityByIds(int userId, int activityId)
        {
            SavedActivityDto? SavedActivity = await _SavedActivityService.GetSavedActivityByIdsAsync(userId, activityId);
            return SavedActivity is null ? NotFound(new { Message = $"SavedActivity with IDs {userId} / {activityId} not found." }) : Ok(SavedActivity);
        }

        [HttpGet("by-user-id/{userId}")]
        public async Task<IActionResult> GetSavedActivityByActivityId(int userId)
        {
            IEnumerable<SavedActivityDto> SavedActivitys = await _SavedActivityService.GetSavedActivitiesByUserIdAsync(userId);
            return SavedActivitys is null ? NotFound(new { Message = $"SavedActivitys with userID {userId} not found." }) : Ok(SavedActivitys);
        }

        [HttpGet("by-activity-id/{activityId}")]
        public async Task<IActionResult> GetSavedActivityByUserId(int activityId)
        {
            IEnumerable<SavedActivityDto> SavedActivitys = await _SavedActivityService.GetSavedActivitiesByActivityIdAsync(activityId);
            return SavedActivitys is null ? NotFound(new { Message = $"SavedActivitys with activityID {activityId} not found." }) : Ok(SavedActivitys);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSavedActivity(int id)
        {
            await _SavedActivityService.DeleteSavedActivityAsync(id);
            return NoContent();
        }
    }
}
