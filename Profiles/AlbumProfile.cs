using AutoMapper;
using MusiCloud.Dtos;
using MusiCloud.Models;

namespace MusiCloud.Profiles;

public class AlbumProfile : Profile
{
    public AlbumProfile()
    {
        CreateMap<AlbumDto, Album>();
    }
}
