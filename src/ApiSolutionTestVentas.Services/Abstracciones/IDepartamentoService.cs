using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IDepartamentoService
{
    Task<BaseResponseGeneric<DepartamentoResponseDto>> GetAsync(int id);
    
    Task<BaseResponseGeneric<ICollection<DepartamentoResponseDto>>> GetAsync(string? nombreDepartamento);
    Task<BaseResponseGeneric<int>> AddAsync(DepartamentoRequestDto departamentoRequestDto);
    Task<BaseResponseGeneric<int>> UpdateAsync(int id, DepartamentoRequestDto departamentoRequestDto);
    Task<BaseResponse> DeleteAsync(int id);
}