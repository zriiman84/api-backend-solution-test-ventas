using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class CategoriaProductoProfile : Profile
{
    public CategoriaProductoProfile()
    {
        CreateMap<CategoriaProductoRequestDto, CategoriaProducto>();
        CreateMap<CategoriaProducto, CategoriaProductoResponseDto>()
            .ForMember(d => d.Status, o => o.MapFrom(x => x.Status ? "Activo" : "Inactivo"));
    }
}