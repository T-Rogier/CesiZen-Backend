using CesiZen_Backend.Dtos.ParticipationDtos;
using CesiZen_Backend.Services.ParticipationService;
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

        [HttpPost]
        public async Task<IActionResult> CreateParticipation([FromBody] CreateParticipationDto command)
        {
            var participation = await _ParticipationService.CreateParticipationAsync(command);
            return CreatedAtAction(nameof(GetParticipationById), new { id = participation.Id }, participation);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllParticpations()
        {
            var participations = await _ParticipationService.GetAllParticipationsAsync();
            return Ok(participations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetParticipationById(int id)
        {
            var user = await _ParticipationService.GetParticipationByIdAsync(id);
            return user is null ? NotFound(new { Message = $"Participation with ID {id} not found." }) : Ok(user);
        }

        [HttpGet("by-ids/{userId}/{activityId}")]
        public async Task<IActionResult> GetParticipationByIds(int userId, int activityId)
        {
            var participation = await _ParticipationService.GetParticipationsByIdsAsync(userId, activityId);
            return participation is null ? NotFound(new { Message = $"Participation with IDs {userId} / {activityId} not found." }) : Ok(participation);
        }

        [HttpGet("by-user-id/{userId}")]
        public async Task<IActionResult> GetParticipationByActivityId(int userId)
        {
            var participations = await _ParticipationService.GetParticipationsByUserIdAsync(userId);
            return participations is null ? NotFound(new { Message = $"Participations with userID {userId} not found." }) : Ok(participations);
        }

        [HttpGet("by-activity-id/{activityId}")]
        public async Task<IActionResult> GetParticipationByUserId(int activityId)
        {
            var participations = await _ParticipationService.GetParticipationsByActivityIdAsync(activityId);
            return participations is null ? NotFound(new { Message = $"Participations with activityID {activityId} not found." }) : Ok(participations);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipation(int id)
        {
            await _ParticipationService.DeleteParticipationAsync(id);
            return NoContent();
        }
    }
}
