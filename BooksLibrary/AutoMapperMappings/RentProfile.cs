using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Models.RentViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.AutoMapperMappings
{
    public class RentProfile : Profile
    {
        private readonly IMapperConfiguration mapperConfiguration;

        public RentProfile(IMapperConfiguration config)
        {
            mapperConfiguration = config;
        }

        protected override void Configure()
        {
            mapperConfiguration.CreateMap<Rent, UserRentsViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Book.Title));
        }
    }
}