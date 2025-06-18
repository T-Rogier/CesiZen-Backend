using CesiZen_Backend.Dtos.ParticipationDtos;
using CesiZen_Backend.Common.Filters;
using CesiZen_Backend.Models;
using CesiZen_Backend.Services.ParticipationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/participations")]
    public class ParticipationController : ControllerBase
    {
        private readonly IParticipationService _ParticipationService;

        public ParticipationController(IParticipationService participationService)
        {
            _ParticipationService = participationService;
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpPost]
        public async Task<IActionResult> CreateParticipation([FromBody] CreateParticipationRequestDto command)
        {
            ParticipationResponseDto participation = await _ParticipationService.CreateParticipationAsync(command);
            return CreatedAtAction(nameof(GetParticipationById), new { id = participation.Id }, participation);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllParticpations()
        {
            IEnumerable<ParticipationResponseDto> participations = await _ParticipationService.GetAllParticipationsAsync();
            return Ok(participations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetParticipationById(int id)
        {
            ParticipationResponseDto? participation = await _ParticipationService.GetParticipationByIdAsync(id);
            return participation is null ? NotFound(new { Message = $"Participation with ID {id} not found." }) : Ok(participation);
        }

        [HttpGet("by-ids/{userId}/{activityId}")]
        public async Task<IActionResult> GetParticipationByIds(int userId, int activityId)
        {
            IEnumerable<ParticipationResponseDto> participation = await _ParticipationService.GetParticipationsByIdsAsync(userId, activityId);
            return participation is null ? NotFound(new { Message = $"Participation with IDs {userId} / {activityId} not found." }) : Ok(participation);
        }

        [HttpGet("by-user-id/{userId}")]
        public async Task<IActionResult> GetParticipationByActivityId(int userId)
        {
            IEnumerable<ParticipationResponseDto> participations = await _ParticipationService.GetParticipationsByUserIdAsync(userId);
            return participations is null ? NotFound(new { Message = $"Participations with userID {userId} not found." }) : Ok(participations);
        }

        [HttpGet("by-activity-id/{activityId}")]
        public async Task<IActionResult> GetParticipationByUserId(int activityId)
        {
            IEnumerable<ParticipationResponseDto> participations = await _ParticipationService.GetParticipationsByActivityIdAsync(activityId);
            return participations is null ? NotFound(new { Message = $"Participations with activityID {activityId} not found." }) : Ok(participations);
        }

        [Authorize]
        [AuthorizeRole(UserRole.User)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipation(int id)
        {
            await _ParticipationService.DeleteParticipationAsync(id);
            return NoContent();
        }
    }
}
