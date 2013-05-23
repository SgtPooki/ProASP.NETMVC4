using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using System.Collections.Generic;
using Moq;

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
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(mockedObj => mockedObj.Products)
                .Returns(new List<Product> {
                    new Product { Name = "SoccerBall", Price = 25},
                    new Product { Name = "Boogie board", Price = 179},
                    new Product { Name = "Walking shoes", Price = 95}
                }.AsQueryable());

            this.ninjectKernel.Bind<IProductsRepository>().ToConstant(mock.Object);
        }
    }
}