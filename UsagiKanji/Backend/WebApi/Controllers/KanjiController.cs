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
    }
}
