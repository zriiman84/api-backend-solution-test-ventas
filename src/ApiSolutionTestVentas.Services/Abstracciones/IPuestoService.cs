using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IPuestoService
{
    Task<BaseResponseGeneric<PuestoResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<ICollection<PuestoResponseDto>>> GetAsync(string? nombrePuesto);
    Task<BaseResponseGeneric<int>> AddAsync(PuestoRequestDto puestoRequestDto);
    Task<BaseResponseGeneric<int>> UpdateAsync(int id, PuestoRequestDto puestoRequestDto);
    Task<BaseResponse> DeleteAsync(int id);
}