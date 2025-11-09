using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Extensions;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpDto dto)
    {
        var result = await _userService.SignUpAsync(dto.Username, dto.Email, dto.Password);
        return result.ToActionResult(this);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto.UsernameOrEmail, dto.Password);
        return result.ToActionResult(this, token => Ok(new { Token = token }));
    }
}
