using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IProductoService
{
    Task<BaseResponseGeneric<ProductoResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<ICollection<ProductoResponseDto>>> GetAsync(string? nombreProducto, PaginationDto paginacion);
    Task<BaseResponseGeneric<int>> AddAsync(ProductoRequestDto productoRequest);
    Task<BaseResponseGeneric<int>> UpdateAsync(int id, ProductoRequestDto productoRequest);
    Task<BaseResponse> DeleteAsync(int id);

}