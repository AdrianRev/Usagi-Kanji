using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly ISrsService _srsService;
        private readonly ICurrentUserService _currentUser;

        public ReviewController(ISrsService srsService, ICurrentUserService currentUser)
        {
            _srsService = srsService;
            _currentUser = currentUser;
        }

        [HttpGet("due")]
        public async Task<IActionResult> GetDueKanji()
        {
            var userId = _currentUser.UserId;
            if (userId == null) return Unauthorized();

            var dueKanji = await _srsService.GetDueKanjiAsync(userId.Value);
            return Ok(dueKanji);
        }

        [HttpPost("{kanjiId}")]
        public async Task<IActionResult> SubmitReview(Guid kanjiId, [FromBody] SubmitReviewDto dto)
        {
            var userId = _currentUser.UserId;
            if (userId == null) return Unauthorized();

            var result = await _srsService.SubmitReviewAsync(userId.Value, kanjiId, dto.Rating);
            return result.IsFailed ? BadRequest(result.Errors.Select(e => e.Message)) : Ok("Review submitted");
        }
    }
}
