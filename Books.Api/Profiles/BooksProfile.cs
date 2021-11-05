using AutoMapper;
using Books.Api.Entities;
using Books.Api.Models;

namespace Books.Api.Profiles
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Book, BookModel>()
                .ForMember(dest => dest.Author, opt => 
                    opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));
        }
    }
}
