using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using ApiSolutionTestVentas.Services.Implementaciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ApiSolutionTestVentas.Api.Controllers;


[ApiController]
[Route("api/[controller]")] // api/ventas/
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaServiceService;

    public VentasController(IVentaService service)
    {
        this._ventaServiceService = service;
    }

    [HttpGet("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var response = await _ventaServiceService.GetAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("ListarVentas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
    public async Task<IActionResult> GetByDate([FromQuery] bool flagPagination, [FromQuery] PaginationDto? pagination)
    {
        var response = await _ventaServiceService.GetAsync(flagPagination, pagination);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("ListarVentasPorFecha")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
    public async Task<IActionResult> GetByDate([FromQuery] SearchVentaDto searchVentaDto, [FromQuery] PaginationDto pagination)
    {
        var response = await _ventaServiceService.GetAsync(searchVentaDto, pagination);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("ListarVentasPorClienteYEmpleado")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
    public async Task<IActionResult> GetByParams(
        [FromQuery] string email,
        [FromQuery] string? nombresEmpleado,
        [FromQuery] string? apellidosEmpleado,
        [FromQuery] PaginationDto pagination)
    {
        var response = await _ventaServiceService.GetAsync(email, nombresEmpleado, apellidosEmpleado, pagination);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleCustomer)]
    public async Task<IActionResult> Post([FromBody] VentaRequestDto ventaRequestDto)
    {
        //Get authenticated user email
        var email = HttpContext.User.Claims.First(p => p.Type == ClaimTypes.Email).Value;

        var response = await _ventaServiceService.RealizarVentaAsync(email, ventaRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? StatusCode((int)HttpStatusCode.Created, response) : BadRequest(response);
    }

    [HttpGet("ListarMisCompras")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleCustomer)]
    public async Task<IActionResult> GetMyPurchases(
        [FromQuery] PaginationDto pagination)
    {
        //Get authenticated user email
        var email = HttpContext.User.Claims.First(p => p.Type == ClaimTypes.Email).Value;
        var response = await _ventaServiceService.GetAsync(email, String.Empty, String.Empty, pagination);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("GetListProductsByIds")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleCustomer)]
    public async Task<IActionResult> GetListProductsByIds([FromBody] List<DetalleVentaRequestDto> detallesVentaRequestDto)
    {
        var response = await _ventaServiceService.GetListProductsByListIdAsync(detallesVentaRequestDto);
        if (!string.IsNullOrEmpty(response.ErrorMessage)) return StatusCode(500, response);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}