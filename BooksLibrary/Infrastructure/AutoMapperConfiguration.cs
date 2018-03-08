using AutoMapper;
using BooksLibrary.AutoMapperMappings;
using BooksLibrary.Business.Models;
using BooksLibrary.Models;
using BooksLibrary.Models.BookViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.Infrastructure
{
    public class AutoMapperConfiguration
    {
        public AutoMapperConfiguration(IMapperConfiguration mapperConfiguration)
        {
            Configure(mapperConfiguration);
        }

        private void Configure(IMapperConfiguration mapperConfiguration)
        {
            mapperConfiguration.AddProfile(new BookProfile(mapperConfiguration));
            mapperConfiguration.AddProfile(new ReservationProfile(mapperConfiguration));
            mapperConfiguration.AddProfile(new RentProfile(mapperConfiguration));
            mapperConfiguration.AddProfile(new ManagementProfile(mapperConfiguration));
            mapperConfiguration.AddProfile(new AccountProfile(mapperConfiguration));
        }
    }
}