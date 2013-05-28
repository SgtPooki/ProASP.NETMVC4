using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private Mock<IProductsRepository> mockProductRepo = new Mock<IProductsRepository>();

        public UnitTest1()
        {
            this.mockProductRepo.Setup(mockedObj => mockedObj.Products)
                .Returns(new Product[] {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                }.AsQueryable());
        }

        [TestMethod]
        public void canPaginate()
        {
            //arrange
            ProductController controller = new ProductController(this.mockProductRepo.Object);
            controller.productsPerPage = 3;

            //act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;

            //assert
            Product[] prodArray = result.Products.ToArray();
            Assert.AreEqual(prodArray.Length, 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void canGeneratePageLinks()
        {
            //arrange
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = (i => "Page" + i);

            //act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //assert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
                                             + @"<a class=""selected"" href=""Page2"">2</a>"
                                             + @"<a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void canSendPageViewModel()
        {
            //Arrange            
            ProductController controller = new ProductController(this.mockProductRepo.Object);
            controller.productsPerPage = 3;

            //act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;

            //assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

    }
}
