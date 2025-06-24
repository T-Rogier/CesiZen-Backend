using CesiZen_Backend.Common.Filters;
using CesiZen_Backend.Dtos;
using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/activities")]
    public class ActivityController : ApiControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService, ICurrentUserService currentUserService) : base(currentUserService)
        {
            _activityService = activityService;
        }

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequestDto command)
        {
            User currentUser = await GetCurrentUserAsync();

            FullActivityResponseDto activity = await _activityService.CreateActivityAsync(command, currentUser);
            return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpPost("save/{activityId:int}")]
        public async Task<IActionResult> SaveActivity(int activityId, [FromBody] SaveActivityRequestDto command)
        {
            User currentUser = await GetCurrentUserAsync();

            FullActivityResponseDto activity = await _activityService.SaveActivityAsync(activityId, command, currentUser);
            return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpPost("participate/{activityId:int}")]
        public async Task<IActionResult> ParticipateActivity(int activityId, [FromBody] ParticipateActivityRequestDto command)
        {
            User currentUser = await GetCurrentUserAsync();

            await _activityService.ParticipateActivityAsync(activityId, command, currentUser);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            ActivityListResponseDto activities = await _activityService.GetAllActivitiesAsync();
            return Ok(activities);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetActivitiesByFilter([FromQuery] ActivityFilterRequestDto filter)
        {
            ActivityListResponseDto activities = await _activityService.GetActivitiesByFilterAsync(filter);
            return Ok(activities);
        }

        [HttpGet("byCategory/{categoryId:int}")]
        public async Task<IActionResult> GetActivitiesByCategory(int categoryId, [FromQuery] PagingRequestDto paging)
        {
            ActivityListResponseDto activities = await _activityService.GetActivitiesByCategoryAsync(categoryId, paging);
            return Ok(activities);
        }

        [HttpGet("byCreator/{creatorId:int}")]
        public async Task<IActionResult> GetActivitiesByCreator(int creatorId, [FromQuery] PagingRequestDto paging)
        {
            ActivityListResponseDto activities = await _activityService.GetActivitiesByCreatorAsync(creatorId, paging);
            return Ok(activities);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpGet("byState")]
        public async Task<IActionResult> GetActivitiesByState([FromQuery] ActivityByStateRequestDto filter)
        {
            User currentUser = await GetCurrentUserAsync();

            ActivityListResponseDto activities = await _activityService.GetActivitiesByStateAsync(currentUser, filter);
            return Ok(activities);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpGet("favorite")]
        public async Task<IActionResult> GetFavoritesActivities([FromQuery] PagingRequestDto paging)
        {
            User currentUser = await GetCurrentUserAsync();

            ActivityListResponseDto activities = await _activityService.GetFavoritesActivitiesAsync(currentUser, paging);
            return Ok(activities);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedActivities([FromQuery] PagingRequestDto paging)
        {
            User currentUser = await GetCurrentUserAsync();

            ActivityListResponseDto activities = await _activityService.GetSavedActivitiesAsync(currentUser, paging);
            return Ok(activities);
        }

        [HttpGet("type")]
        public async Task<IActionResult> GetActivityTypes()
        {
            ActivityTypeListReponseDto activityTypes = await _activityService.GetActivityTypesAsync();
            return Ok(activityTypes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            User? currentUser = await TryGetCurrentUserAsync();

            FullActivityResponseDto? activity = await _activityService.GetActivityByIdAsync(id, currentUser);
            return activity is null ? NotFound(new { Message = $"Activity with ID {id} not found." }) : Ok(activity);
        }

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityRequestDto command)
        {
            await _activityService.UpdateActivityAsync(id, command);
            return NoContent();
        }

        [Authorize]
        [AuthorizeRole(UserRole.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            await _activityService.DeleteActivityAsync(id);
            return NoContent();
        }
    }
}
