using AutoMapper;
using BooksLibrary.Business.Models;
using BooksLibrary.Models.AccountViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.AutoMapperMappings
{
    public class AccountProfile : Profile
    {
        private readonly IMapperConfiguration mapperConfiguration;

        public AccountProfile(IMapperConfiguration config)
        {
            mapperConfiguration = config;
        }

        protected override void Configure()
        {
            mapperConfiguration.CreateMap<UserModel, UsersViewModel>().ForMember(dest => dest.UserFullName, opts => opts.MapFrom(src => src.FirstName + " " + src.LastName));

            mapperConfiguration.CreateMap<UserModel, UserAccountSettingsViewModel>().ForMember(dest => dest.UserRoles, opts => opts.Ignore()).ReverseMap();

            mapperConfiguration.CreateMap<UserModel, AccountManagementViewModel>().ForMember(dest => dest.UserRoles, opts => opts.Ignore()).ReverseMap();

            mapperConfiguration.CreateMap<UserModel, EditPersonalDataViewModel>().ReverseMap();

            mapperConfiguration.CreateMap<Address, AddressViewModel>().ReverseMap();
        }
    }
}