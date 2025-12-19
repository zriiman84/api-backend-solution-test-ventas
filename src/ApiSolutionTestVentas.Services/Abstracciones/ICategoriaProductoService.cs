using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface ICategoriaProductoService
{
    Task<BaseResponseGeneric<CategoriaProductoResponseDto>> GetAsync(int id);

    Task<BaseResponseGeneric<ICollection<CategoriaProductoResponseDto>>> GetAsync(string? nombreCategoria);
    Task<BaseResponseGeneric<int>> AddAsync(CategoriaProductoRequestDto categoriaRequestDto);
    Task<BaseResponseGeneric<int>> UpdateAsync(int id, CategoriaProductoRequestDto categoriaRequestDto);
    Task<BaseResponse> DeleteAsync(int id);
}