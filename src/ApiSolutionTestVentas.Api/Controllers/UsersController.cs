using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiSolutionTestVentas.Api.Controllers;


[ApiController]
[Route("api/[controller]")] // api/users/
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        this._userService = userService;
    }

    [HttpPost("RegistrarUsuario")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
    {
        var response = await _userService.RegisterAsync(request);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _userService.LoginAsync(request);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("RequestTokenToResetPassword")]
    public async Task<IActionResult> RequestTokenToResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var response = await _userService.RequestTokenToResetPasswordAsync(request);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] NewPasswordResetRequestDto request)
    {
        var response = await _userService.ResetPasswordAsync(request);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    //Aquí el usuario está logueado y tiene un token.
    [HttpPost("ChangePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequestDto request)
    {
        //Get authenticated user email
        var email = HttpContext.User.Claims.First(p => p.Type == ClaimTypes.Email).Value;
        var response = await _userService.ChangePasswordAsync(email, request);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}