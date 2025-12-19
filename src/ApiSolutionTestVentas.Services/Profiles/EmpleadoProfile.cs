using System.Globalization;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Entities.Info;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class EmpleadoProfile : Profile
{
    public EmpleadoProfile()
    {
        CreateMap<EmpleadoRequestDto, Empleado>();
        CreateMap<EmpleadoInfo, EmpleadoResponseDto>()
            .ForMember(d => d.FechaHoraIngreso, o
                => o.MapFrom(x => $"{x.FechaIngreso} {x.HoraIngreso}"))
            .ForMember(d => d.FechaHoraBaja, o
                => o.MapFrom(x => !string.IsNullOrEmpty(x.FechaBaja)? $"{x.FechaBaja} {x.HoraBaja}" : String.Empty));
        CreateMap<Empleado, EmpleadoResponseDto>()
            .ForMember(d => d.FechaHoraIngreso, o 
                => o.MapFrom(x => x.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)))
            .ForMember(d => d.FechaHoraBaja, o 
                => o.MapFrom(x => x.FechaBaja != null?  x.FechaBaja.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : String.Empty))
            .ForMember(d => d.Puesto, o => o.MapFrom(x => x.Puesto.NombrePuesto))
            .ForMember(d => d.Departamento, o => o.MapFrom(x => x.Departamento.NombreDepartamento))
            .ForMember(d => d.Status, o => o.MapFrom(x => x.Status ? "Activo" : "Inactivo"));
    }
}