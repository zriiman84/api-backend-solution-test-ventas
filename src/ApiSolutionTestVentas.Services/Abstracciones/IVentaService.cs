using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IVentaService
{
    Task<BaseResponseGeneric<VentaResponseDto>> GetAsync(int idVenta);
    
    Task<BaseResponseGeneric<ICollection<VentaResponseDto>>> GetAsync(bool flagPagination, PaginationDto? pagination);
    Task<BaseResponseGeneric<int>> RealizarVentaAsync(string email, VentaRequestDto ventaRequestDto);
    Task<BaseResponseGeneric<ICollection<VentaResponseDto>>> GetAsync(string emailCliente, string? nombresEmpleado, string? apellidosEmpleado, PaginationDto pagination);
    Task<BaseResponseGeneric<ICollection<VentaResponseDto>>> GetAsync(SearchVentaDto? searchVentaDto, PaginationDto pagination);
    Task<BaseResponseGeneric<ICollection<VentaReporteClienteResponseDto>>> GetAsyncSaleReportByClient(string? fechaInicio, string? fechaFin);
    Task<BaseResponseGeneric<ICollection<VentaReporteProductoResponseDto>>> GetAsyncSaleReportByProduct(string? fechaInicio, string? fechaFin);

    Task<BaseResponseGeneric<ICollection<ProductoResponseDto>>> GetListProductsByListIdAsync(List<DetalleVentaRequestDto> detallesVentaRequestDto);

}