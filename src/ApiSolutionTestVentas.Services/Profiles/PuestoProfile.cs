using System.Globalization;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class PuestoProfile : Profile
{
    public PuestoProfile()
    {
        // Origen --> destino
        CreateMap<PuestoRequestDto, Puesto>();
        CreateMap<Puesto, PuestoResponseDto>()
            .ForMember(d => d.FechaHoraCreacion, o 
                => o.MapFrom(x => x.FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)))
            .ForMember(d => d.FechaHoraBaja, o 
                => o.MapFrom(x => x.FechaBaja != null?  x.FechaBaja.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : String.Empty))
            .ForMember(d => d.Status, o => o.MapFrom(x => x.Status ? "Activo" : "Inactivo"));

    }
}