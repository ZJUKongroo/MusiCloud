using AutoMapper;
using MusiCloud.Dtos;
using MusiCloud.Models;

namespace MusiCloud.MappingProfiles;

public class MusicProfile : Profile
{
    public MusicProfile()
    {
        // 艺术家映射
        CreateMap<Artist, ArtistDto>();
        
        // 元数据映射
        CreateMap<Metadata, MetadataDto>();
        
        // 专辑映射 - 包括艺术家集合的转换
        CreateMap<Album, AlbumDto>()
            .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => 
                src.AlbumArtists.Select(aa => aa.Artist)));
        
        // 音乐映射 - 包括艺术家集合的转换
        CreateMap<Music, MusicDto>()
            .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => 
                src.MusicArtists.Select(ma => ma.Artist)));
    }
}