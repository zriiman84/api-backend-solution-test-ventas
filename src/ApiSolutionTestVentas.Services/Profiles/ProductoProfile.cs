using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class ProductoProfile : Profile
{
    public ProductoProfile()
    {
        CreateMap<ProductoRequestDto, Producto>()
            .ForMember(d => d.ImageUrl, options => options.Ignore()); //Ignora el mapeo de este campo
        CreateMap<Producto, ProductoResponseDto>()
            .ForMember(d => d.Status, o 
                => o.MapFrom(x => x.Status? "Activo": "Inactivo"))
            .ForMember(d => d.NombreCategoriaProducto, o => o.MapFrom(x => x.CategoriaProducto != null ? x.CategoriaProducto.Nombre : String.Empty));
    }
}