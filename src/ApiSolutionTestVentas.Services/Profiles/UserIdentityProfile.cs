using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Persistencia;
using AutoMapper;

namespace ApiSolutionTestVentas.Services.Profiles;

public class UserIdentityProfile : Profile
{
    public UserIdentityProfile()
    {
        CreateMap<RegisterUserRequestDto, SpecificUserIdentity>()
            .ForMember(p => p.UserName, 
                o => o.MapFrom(s => s.UserName))
            .ForMember(p => p.DocumentType, 
                o => o.MapFrom(s => (DocumentTypeEnum)s.DocumentType))
            ;
    }
}