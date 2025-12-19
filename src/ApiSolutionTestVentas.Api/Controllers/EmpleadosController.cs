using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiSolutionTestVentas.Api.Controllers;


[ApiController]
[Route("api/[controller]")] // api/empleados/
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;

    public EmpleadosController(IEmpleadoService service)
    {
        this._empleadoService = service;
    }

    [HttpGet("GetById/{id:int}")] //api/empleados/id
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var response = await _empleadoService.GetAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("GetByNroDocumento")]
    public async Task<IActionResult> GetByNroDocumentoAsync([FromQuery] string nroDocumento)
    {
        var response = await _empleadoService.GetByNroDocumentoAsync(nroDocumento);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("GetByEmail")]
    public async Task<IActionResult> GetByEmailAsync([FromQuery] string email)
    {
        var response = await _empleadoService.GetByEmailAsync(email);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? nombre, [FromQuery] PaginationDto paginacion)
    {
        var response = await _empleadoService.GetAsync(nombre, paginacion);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("GetByParametros")]
    public async Task<IActionResult> Get([FromQuery] string? nombres, [FromQuery] string? apellidos, [FromQuery] PaginationDto paginacion)
    {
        var response = await _empleadoService.GetAsync(nombres, apellidos, paginacion);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EmpleadoRequestDto empleadoRequest)
    {
        var response = await _empleadoService.AddAsync(empleadoRequest);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? StatusCode((int)HttpStatusCode.Created, response) : BadRequest(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] EmpleadoRequestDto empleadoRequest)
    {
        var response = await _empleadoService.UpdateAsync(id, empleadoRequest);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);

        var validation = response.Message?.Contains(id.ToString()) ?? false;
        if (!response.Success) return validation ? NotFound(response) : BadRequest(response);
        return Ok(response);

    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var response = await _empleadoService.DeleteAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }
}