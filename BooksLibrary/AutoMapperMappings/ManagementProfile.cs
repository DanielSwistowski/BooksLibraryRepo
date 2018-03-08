using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Models.ManagementViewModels;
using BooksLibrary.Models.ReservationViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.AutoMapperMappings
{
    public class ManagementProfile : Profile
    {
        private readonly IMapperConfiguration mapperConfiguration;
        public ManagementProfile(IMapperConfiguration config)
        {
            mapperConfiguration = config;
        }
        protected override void Configure()
        {
            mapperConfiguration.CreateMap<Reservation, ReservationsManagementViewModel>()
                .ForMember(dest => dest.UserFullName, opts => opts.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Book.Title));

            mapperConfiguration.CreateMap<Reservation, ReleaseBookViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserFullName, opts => opts.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.RentTimeInDays, opts => opts.Ignore());

            mapperConfiguration.CreateMap<Rent, ReturnBookViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserFullName, opts => opts.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

            mapperConfiguration.CreateMap<Rent,RentsManagementViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserFullName, opts => opts.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
        }
    }
}