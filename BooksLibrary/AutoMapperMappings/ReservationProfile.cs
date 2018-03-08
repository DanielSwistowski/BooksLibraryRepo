using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Models.ReservationViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.AutoMapperMappings
{
    public class ReservationProfile : Profile
    {
        private readonly IMapperConfiguration mapperConfiguration;

        public ReservationProfile(IMapperConfiguration config)
        {
            mapperConfiguration = config;
        }

        protected override void Configure()
        {
            mapperConfiguration.CreateMap<Reservation, UserReservationsViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Book.Title));

            mapperConfiguration.CreateMap<Reservation, ReservationViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Book.Title));
        }
    }
}