using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IClienteService
{
    Task<BaseResponseGeneric<ICollection<ClienteResponseDto>>> GetAsync(PaginationDto pagination);
    Task<BaseResponseGeneric<ClienteResponseDto>> GetAsync(int id);
    
    Task<BaseResponseGeneric<ClienteResponseDto>> GetAsync(string email);
    Task<BaseResponseGeneric<ICollection<ClienteResponseDto>>> GetAsync(string? nombres, string? apellidos, PaginationDto pagination);
    //Task<BaseResponseGeneric<int>> AddAsync(ClienteRequestDto clienteRequestDto); //se registra el cliente por el UserService
    //Task<BaseResponseGeneric<int>> UpdateAsync(int id, ClienteRequestDto clienteRequestDto);
    //Task<BaseResponse> DeleteAsync(int id);
}