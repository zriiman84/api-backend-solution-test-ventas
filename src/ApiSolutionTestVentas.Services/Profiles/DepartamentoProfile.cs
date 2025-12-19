using System.Globalization;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class DepartamentoProfile : Profile
{
    public DepartamentoProfile()
    {
        CreateMap<DepartamentoRequestDto, Departamento>();
        CreateMap<Departamento, DepartamentoResponseDto>()
            .ForMember(d => d.FechaHoraCreacion, o => o.MapFrom(x => x.FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)))
            .ForMember(d => d.FechaHoraBaja, o => o.MapFrom(x => x.FechaBaja !=null ? x.FechaBaja.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : String.Empty))
            .ForMember(d => d.Status, o => o.MapFrom(x => x.Status ? "Activo" : "Inactivo"));
    }
}