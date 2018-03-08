using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Models;
using BooksLibrary.Models.BookViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.AutoMapperMappings
{
    public class BookProfile : Profile
    {
        private readonly IMapperConfiguration mapperConfiguration;

        public BookProfile(IMapperConfiguration config)
        {
            mapperConfiguration = config;
        }

        protected override void Configure()
        {
            mapperConfiguration.CreateMap<Book, IndexAllBooksViewModel>().ForMember(dest => dest.Category, opts => opts.MapFrom(src => src.BookCategory.CategoryName));

            mapperConfiguration.CreateMap<BookViewModel, Book>().ReverseMap();
        }
    }
}