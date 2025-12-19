using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiSolutionTestVentas.Api.Controllers;


[ApiController]
[Route("api/[controller]")] // api/productos/
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        this._productoService = productoService;
    }

    [HttpGet("{id:int}")] //api/productos/id
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var response = await _productoService.GetAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("GetByNombre")]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] string? nombreProducto, [FromQuery] PaginationDto paginacion)
    {

        var response = await _productoService.GetAsync(nombreProducto, paginacion);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] ProductoRequestDto productoRequestDto)
    {
        var response = await _productoService.AddAsync(productoRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? StatusCode((int)HttpStatusCode.Created, response) : BadRequest(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromForm] ProductoRequestDto productoRequestDto)
    {
        var response = await _productoService.UpdateAsync(id, productoRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var response = await _productoService.DeleteAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

}