using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiSolutionTestVentas.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // api/categorias/
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaProductoService _categoriaService;

    public CategoriasController(ICategoriaProductoService categoriaService)
    {
        this._categoriaService = categoriaService;
    }


    [HttpGet("{id:int}")] //api/departamentos/id
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var response = await _categoriaService.GetAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("GetByNombre")]
    public async Task<IActionResult> Get([FromQuery] string? descripcion)
    {
        var response = await _categoriaService.GetAsync(descripcion);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CategoriaProductoRequestDto categoriaRequestDto)
    {
        var response = await _categoriaService.AddAsync(categoriaRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? StatusCode((int)HttpStatusCode.Created, response) : BadRequest(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] CategoriaProductoRequestDto categoriaRequestDto)
    {
        var response = await _categoriaService.UpdateAsync(id, categoriaRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var response = await _categoriaService.DeleteAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }
}