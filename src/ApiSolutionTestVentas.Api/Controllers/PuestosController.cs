using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiSolutionTestVentas.Api.Controllers;


[ApiController]
[Route("api/[controller]")] // api/puestos/
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
public class PuestosController : ControllerBase
{
    private readonly IPuestoService _puestoService;

    public PuestosController(IPuestoService service)
    {
        this._puestoService = service;
    }

    [HttpGet("{id:int}")] //api/puestos/id
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var response = await _puestoService.GetAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("GetByNombre")]
    public async Task<IActionResult> Get([FromQuery] string? descripcion)
    {
        var response = await _puestoService.GetAsync(descripcion);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PuestoRequestDto puestoRequestDto)
    {
        var response = await _puestoService.AddAsync(puestoRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? StatusCode((int)HttpStatusCode.Created, response) : BadRequest(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PuestoRequestDto puestoRequestDto)
    {
        var response = await _puestoService.UpdateAsync(id, puestoRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var response = await _puestoService.DeleteAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }
}