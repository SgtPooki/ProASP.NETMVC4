﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using Moq;
using SportsStore.Domain.Concrete;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            this.ninjectKernel = new StandardKernel();
            this.AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return (controllerType == null)
                ? null
                : (IController)this.ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            this.ninjectKernel.Bind<IProductsRepository>().To<EFProductRepository>();
        }
    }
}