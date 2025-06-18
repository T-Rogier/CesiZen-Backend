using CesiZen_Backend.Dtos.SavedActivityDtos;
using CesiZen_Backend.Common.Filters;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.SavedActivityService;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpPost]
        public async Task<IActionResult> CreateSavedActivity([FromBody] CreateSavedActivityRequestDto command)
        {
            SavedActivityResponseDto savedActivity = await _SavedActivityService.CreateSavedActivityAsync(command);
            return CreatedAtAction(nameof(GetSavedActivityByIds), new { userId = savedActivity.UserId, activityId = savedActivity.ActivityId }, savedActivity);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            IEnumerable<SavedActivityResponseDto> SavedActivitys = await _SavedActivityService.GetAllSavedActivitiesAsync();
            return Ok(SavedActivitys);
        }

        [HttpGet("by-ids/{userId:int}/{activityId:int}")]
        public async Task<IActionResult> GetSavedActivityByIds(int userId, int activityId)
        {
            SavedActivityResponseDto? SavedActivity = await _SavedActivityService.GetSavedActivityByIdsAsync(userId, activityId);
            return SavedActivity is null ? NotFound(new { Message = $"SavedActivity with IDs {userId} / {activityId} not found." }) : Ok(SavedActivity);
        }

        [HttpGet("by-user-id/{userId:int}")]
        public async Task<IActionResult> GetSavedActivityByActivityId(int userId)
        {
            IEnumerable<SavedActivityResponseDto> SavedActivitys = await _SavedActivityService.GetSavedActivitiesByUserIdAsync(userId);
            return SavedActivitys is null ? NotFound(new { Message = $"SavedActivitys with userID {userId} not found." }) : Ok(SavedActivitys);
        }

        [HttpGet("by-activity-id/{activityId:int}")]
        public async Task<IActionResult> GetSavedActivityByUserId(int activityId)
        {
            IEnumerable<SavedActivityResponseDto> SavedActivitys = await _SavedActivityService.GetSavedActivitiesByActivityIdAsync(activityId);
            return SavedActivitys is null ? NotFound(new { Message = $"SavedActivitys with activityID {activityId} not found." }) : Ok(SavedActivitys);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSavedActivity(int id, [FromBody] UpdateSavedActivityRequestDto command)
        {
            await _SavedActivityService.UpdateSavedActivityAsync(id, command);
            return NoContent();
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSavedActivity(int id)
        {
            await _SavedActivityService.DeleteSavedActivityAsync(id);
            return NoContent();
        }
    }
}
