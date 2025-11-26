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

        [HttpPost("batch")]
        public async Task<IActionResult> SubmitBatchReview([FromBody] SubmitBatchReviewDto dto)
        {
            var userId = _currentUser.UserId;
            if (userId == null) return Unauthorized();

            var result = await _srsService.SubmitBatchReviewAsync(userId.Value, dto);
            return result.IsFailed
                ? BadRequest(result.Errors.Select(e => e.Message))
                : Ok(new { message = "Reviews submitted successfully" });
        }
    }
}
