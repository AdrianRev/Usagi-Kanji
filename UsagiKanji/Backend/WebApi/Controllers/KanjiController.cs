using Application.Dtos;
using Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanjiController : ControllerBase
    {
        private readonly IKanjiService _kanjiService;
        private readonly IValidator<KanjiListParams> _validator;

        public KanjiController(IKanjiService kanjiService, IValidator<KanjiListParams> validator)
        {
            _kanjiService = kanjiService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] KanjiListParams parameters, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(parameters, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var result = await _kanjiService.GetAllKanjiAsync(parameters, cancellationToken);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        //[HttpGet("{id}")]
        //[Authorize(AuthenticationSchemes = "Bearer")]
        //public async Task<IActionResult> GetKanjiById(Guid id)
        //{
        //    var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        //    if (!Guid.TryParse(sub, out var userId))
        //        return Unauthorized("Invalid token.");


        //    var kanjiDto = await _kanjiService.GetKanjiByIdAsync(id, userId);

        //    if (kanjiDto == null)
        //        return NotFound();

        //    return Ok(kanjiDto);
        //}

        [HttpGet("{kanjiId}/{userId}")]
        public async Task<IActionResult> GetKanjiById(Guid kanjiId, Guid userId)
        {
            var kanjiDto = await _kanjiService.GetKanjiByIdAsync(kanjiId, userId);

            if (kanjiDto == null)
                return NotFound();

            return Ok(kanjiDto);
        }


    }
}
