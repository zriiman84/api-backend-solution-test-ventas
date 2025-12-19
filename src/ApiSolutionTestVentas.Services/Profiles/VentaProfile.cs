using System.Globalization;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Entities.Info;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class VentaProfile : Profile
{
    public VentaProfile()
    {
        
        CreateMap<VentaProducto, DetalleVentaInfo>()
            .ForMember(d => d.NombreProducto, o
                => o.MapFrom(x => x.Producto.Nombre))
            .ForMember(d => d.Precio, o => o.MapFrom(x => x.PrecioCompra))
            .ForMember(d => d.CategoriaProductoId, o
                => o.MapFrom(x =>  x.Producto.CategoriaProducto.Id ))
            .ForMember(d => d.NombreCategoriaProducto, o
                => o.MapFrom(x => x.Producto.CategoriaProducto.Nombre));

        CreateMap<Venta, VentaResponseDto>()
            .ForMember(d => d.IdVenta, o
                => o.MapFrom(x => x.Id))
            .ForMember(d => d.FechaVenta, o
                => o.MapFrom(x => x.FechaHoraVenta.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
            .ForMember(d => d.HoraVenta, o
                => o.MapFrom(x => x.FechaHoraVenta.ToString("HH:mm:ss", CultureInfo.InvariantCulture)))
            .ForMember(d => d.NombreCompletoCliente, o
                => o.MapFrom(x =>  $"{x.Cliente.Nombre} {x.Cliente.Apellidos}"))
            .ForMember(d => d.NombreCompletoEmpleado, o
                => o.MapFrom(x => x.Empleado != null ? $"{x.Empleado.Nombre} {x.Empleado.Apellidos}" : String.Empty));

        CreateMap<VentaReporteClienteInfo, VentaReporteClienteResponseDto>();
        CreateMap<VentaReporteProductoInfo, VentaReporteProductoResponseDto>();
    }
}