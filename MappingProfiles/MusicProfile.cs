using AutoMapper;
using MusiCloud.Dtos;
using MusiCloud.Models;

namespace MusiCloud.MappingProfiles;

public class MusicProfile : Profile
{
    public MusicProfile()
    {
        CreateMap<Artist, ArtistWithAlbumDto>();

        CreateMap<Artist, ArtistDto>();
        
        CreateMap<Metadata, MetadataDto>();

        CreateMap<Album, AlbumDto>();
        
        CreateMap<Album, AlbumWithMusicsArtistsDto>()
            .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => 
                src.AlbumArtists.Select(aa => aa.Artist)))
            .ForMember(dest=>dest.Musics, opt => opt.MapFrom(src => 
                src.Musics.ToList()));

        CreateMap<Music, MusicDto>();

        CreateMap<Music, MusicWithAlbumArtistDto>()
            .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => 
                src.MusicArtists.Select(ma => ma.Artist)));

        CreateMap<Music, MusicWithArtistDto>()
            .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => 
                src.MusicArtists.Select(ma => ma.Artist)));

        CreateMap<SearchResult, SearchResultDto>()
            .ForMember(dest => dest.Musics, opt => opt.MapFrom(src => src.Musics))
            .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => src.Albums))
            .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Artists));
    }
}