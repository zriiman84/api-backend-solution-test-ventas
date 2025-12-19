using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<ClienteRequestDto, Cliente>();
        CreateMap<Cliente, ClienteResponseDto>()
            .ForMember(d => d.Status, o 
                => o.MapFrom(x => x.Status? "Activo": "Inactivo"));
    }
}