using AccountService.Application.DTOs;
using AccountService.Domain;
using AutoMapper;

namespace AccountService.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Account, AccountDTO>().ReverseMap();
        CreateMap<Account, CreateAccountDTO>().ReverseMap();
    }
}