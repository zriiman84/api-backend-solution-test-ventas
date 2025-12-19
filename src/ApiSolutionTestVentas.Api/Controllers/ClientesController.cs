using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiSolutionTestVentas.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // api/clientes/
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        this._clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PaginationDto paginacion)
    {
        var response = await _clienteService.GetAsync(paginacion);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var response = await _clienteService.GetAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("GetByEmail")]
    public async Task<IActionResult> Get([FromQuery] string email)
    {
        var response = await _clienteService.GetAsync(email);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("GetByParametros")]
    public async Task<IActionResult> Get([FromQuery] string? nombres, [FromQuery] string? apellidos, [FromQuery] PaginationDto paginacion)
    {
        var response = await _clienteService.GetAsync(nombres, apellidos, paginacion);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

}