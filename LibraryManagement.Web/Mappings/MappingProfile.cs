using System.Linq;
using AutoMapper;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models.Api;

namespace LibraryManagement.Web.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres == null
                    ? new System.Collections.Generic.List<string>()
                    : src.Genres.Select(g => g.Name).ToList()))
                .ForMember(dest => dest.RatingAverage, opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any()
                    ? src.Reviews.Average(r => r.Rating)
                    : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews == null ? 0 : src.Reviews.Count));

            CreateMap<BookUpsertDto, Book>()
                .ForMember(dest => dest.BookId, opt => opt.Ignore())
                .ForMember(dest => dest.Genres, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore());

            CreateMap<Genre, GenreDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.GenreId))
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books == null ? 0 : src.Books.Count));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items == null
                    ? new System.Collections.Generic.List<OrderItem>()
                    : src.Items));

            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}