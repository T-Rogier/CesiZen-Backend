using CesiZen_Backend.Dtos;
using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Common.Filters;
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
        [HttpGet("byState/{state:string}")]
        public async Task<IActionResult> GetActivitiesByState(string state, [FromQuery] PagingRequestDto paging)
        {
            User currentUser = await GetCurrentUserAsync();

            ActivityListResponseDto activities = await _activityService.GetActivitiesByStateAsync(state, currentUser, paging);
            return Ok(activities);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpGet("favorite")]
        public async Task<IActionResult> GetActivitiesByCreator([FromQuery] PagingRequestDto paging)
        {
            User currentUser = await GetCurrentUserAsync();

            ActivityListResponseDto activities = await _activityService.GetFavoritesActivitiesAsync(currentUser, paging);
            return Ok(activities);
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
