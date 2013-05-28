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
                    new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                }.AsQueryable());
        }

        [TestMethod]
        public void canPaginate()
        {
            //arrange
            ProductController controller = new ProductController(this.mockProductRepo.Object);
            controller.productsPerPage = 3;

            //act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

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
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void canFilterProducts()
        {
            //arrange
            ProductController controller = new ProductController(this.mockProductRepo.Object);
            controller.productsPerPage = 3;

            //action
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            //assert
            Assert.IsTrue(result.Length == 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void CanCreateCategoriesPartialView()
        {
            //arrange
            NavController target = new NavController(this.mockProductRepo.Object);

            //act
            string[] result = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0], "Cat1");
            Assert.AreEqual(result[1], "Cat2");
            Assert.AreEqual(result[2], "Cat3");
        }

        [TestMethod]
        public void CanKnowCurrentCategory()
        {
            //arrange
            NavController target = new NavController(this.mockProductRepo.Object);
            string categoryToSelect = "Cat2";

            //action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void CanGenerateCategoryProductCount()
        {
            //arrange
            ProductController controller = new ProductController(this.mockProductRepo.Object);
            controller.productsPerPage = 3;

            //act
            int result1 = ((ProductsListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int result2 = ((ProductsListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int result3 = ((ProductsListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resultAll = ((ProductsListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            //assert
            Assert.AreEqual(result1, 2);
            Assert.AreEqual(result2, 2);
            Assert.AreEqual(result3, 1);
            Assert.AreEqual(resultAll, 5);
        }

    }
}
