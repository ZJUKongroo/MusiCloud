using AutoMapper;
using MusiCloud.Dtos;
using MusiCloud.Models;

namespace MusiCloud.Profiles;

public class MusicProfile : Profile
{
    public MusicProfile()
    {
        CreateMap<Music, MusicDto>();
    }
}
