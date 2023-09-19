using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Profiles;

/// <summary>
/// 継承しているProfileはAutoMapperです
/// これ使わなくてもいい気がする　というか読み込まれていないような...
/// </summary>
public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        // source => target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandCreateDto, Command>();
        CreateMap<Command, CommandReadDto>();
        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(dest => dest.ExternalId, 
                opt => opt.MapFrom(src => src.Id));
    }
}