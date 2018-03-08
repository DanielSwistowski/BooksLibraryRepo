using AutoMapper;
using AutoMapper.QueryableExtensions;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.Service;
using BooksLibrary.Business.UnitOfWork;
using BooksLibrary.Hangfire;
using Ninject;
using Ninject.Web.Common;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BooksLibrary.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private static IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            GetKernel();
            AddBindings();
        }

        public static IKernel GetKernel()
        {
            if(ninjectKernel == null)
                ninjectKernel = new StandardKernel();
            return ninjectKernel;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            ninjectKernel.Bind<IBookService>().To<BookService>();
            ninjectKernel.Bind<IReservationService>().To<ReservationService>();
            ninjectKernel.Bind<IRentService>().To<RentService>();
            ninjectKernel.Bind<IManagementService>().To<ManagementService>();
            ninjectKernel.Bind<IUserService>().To<UserService>();
            ninjectKernel.Bind<IEmailService>().To<EmailService>();
            ninjectKernel.Bind<IRoleService>().To<RoleService>();
            ninjectKernel.Bind<IReminderService>().To<ReminderService>();
            ninjectKernel.Bind<ICategoryService>().To<CategoryService>();

            ninjectKernel.Bind<MapperConfiguration>().ToSelf().InRequestScope().WithConstructorArgument<Action<IMapperConfiguration>>(cfg => new AutoMapperConfiguration(cfg));
            ninjectKernel.Bind<IMapper>().ToMethod(maper => ninjectKernel.Get<MapperConfiguration>().CreateMapper()).InSingletonScope();
        }
    }
}