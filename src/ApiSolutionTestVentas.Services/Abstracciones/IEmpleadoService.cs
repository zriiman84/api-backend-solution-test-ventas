using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IEmpleadoService
{
    Task<BaseResponseGeneric<EmpleadoResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<EmpleadoResponseDto>> GetByNroDocumentoAsync(string nroDocumento);
    Task<BaseResponseGeneric<EmpleadoResponseDto>> GetByEmailAsync(string email);
    Task<BaseResponseGeneric<ICollection<EmpleadoResponseDto>>> GetAsync(string? nombre, PaginationDto paginacion);
    Task<BaseResponseGeneric<ICollection<EmpleadoResponseDto>>> GetAsync(string? nombres, string? apellidos, PaginationDto paginacion);
    Task<BaseResponseGeneric<int>> AddAsync(EmpleadoRequestDto empleadoRequestDto);
    Task<BaseResponseGeneric<int>> UpdateAsync(int id, EmpleadoRequestDto empleadoRequestDto);
    Task<BaseResponse> DeleteAsync(int id);
}