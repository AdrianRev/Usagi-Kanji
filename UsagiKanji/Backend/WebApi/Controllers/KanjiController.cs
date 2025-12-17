using Application.Dtos;
using Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KanjiController : ControllerBase
    {
        private readonly IKanjiService _kanjiService;
        private readonly IValidator<KanjiListParams> _validator;
        private readonly ICurrentUserService _currentUser;

        public KanjiController(
            IKanjiService kanjiService,
            IValidator<KanjiListParams> validator,
            ICurrentUserService currentUser)
        {
            _kanjiService = kanjiService;
            _validator = validator;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] KanjiListParams parameters, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
                return Unauthorized();

            var result = await _kanjiService.GetAllKanjiAsync(parameters, userId.Value, cancellationToken);

            return result.IsFailed ? BadRequest(result.Errors) : Ok(result.Value);
        }

        [HttpGet("{kanjiId}")]
        public async Task<IActionResult> GetKanjiById(Guid kanjiId)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
                return Unauthorized();

            var kanjiDto = await _kanjiService.GetKanjiByIdAsync(kanjiId, userId.Value);

            return kanjiDto == null ? NotFound() : Ok(kanjiDto);
        }

        [HttpPost("{kanjiId}")]
        public async Task<IActionResult> AddOrUpdateUserKanji(Guid kanjiId, [FromBody] UpdateOrAddUserKanjiDto dto)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
                return Unauthorized();

            var result = await _kanjiService.UpdateOrAddUserKanjiAsync(kanjiId, userId.Value, dto);

            return result.IsFailed ? BadRequest(result.Errors.Select(e => e.Message)) : Ok("UserKanji updated successfully.");
        }
        [HttpGet("{kanjiId}/neighbor")]
        public async Task<IActionResult> GetNeighborKanji(Guid kanjiId, [FromQuery] string sortBy, [FromQuery] bool next = true)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
                return Unauthorized();

            var neighbor = await _kanjiService.GetNeighborKanjiAsync(kanjiId, userId.Value, sortBy, next);

            if (neighbor == null)
                return NotFound();

            return Ok(neighbor);
        }

        [HttpGet("next-unlearned")]
        public async Task<IActionResult> GetNextUnlearnedKanji([FromQuery] string sortBy = "SortIndex_Grade", CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
                return Unauthorized();

            var kanjiDto = await _kanjiService.GetNextUnlearnedKanjiAsync(userId.Value, sortBy, cancellationToken);

            if (kanjiDto == null)
                return NotFound(new { message = "No unlearned kanji found" });

            return Ok(kanjiDto);
        }
    }
}
