using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        public Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
        public Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M};
        public Product[] productArray = 
        {
            new Product {ProductID = 1, Name = "P1", Category = "Apples", Price = 50M},
            new Product {ProductID = 2, Name = "P2", Category = "Apples", Price = 50M},
            new Product {ProductID = 3, Name = "P3", Category = "Bananas", Price = 50M},
            new Product {ProductID = 4, Name = "P4", Category = "Oranges", Price = 50M}
        };
        public Cart cart = new Cart();
        public Cart cart2 = new Cart();
        public Mock<IProductsRepository> mockProductRepo = new Mock<IProductsRepository>();
        public Mock<IOrderProcessor> mockOrderProcessor = new Mock<IOrderProcessor>();
        public CartController cartController;
        public ShippingDetails shippingDetails;

        [TestInitialize]
        public void InitializeTests()
        {
            this.cart.AddItem(p1, 1);
            this.cart.AddItem(p2, 1);
            this.mockProductRepo.Setup(m => m.Products).Returns(this.productArray.AsQueryable());

            this.cartController = new CartController(this.mockProductRepo.Object, this.mockOrderProcessor.Object);

            this.shippingDetails = new ShippingDetails();
        }

        [TestMethod]
        public void CanAddNewLines()
        {
            //arrange
            //check initialization function

            //act 
            CartLine[] result = this.cart.Lines.ToArray();

            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[1].Product, p2);
        }

        [TestMethod]
        public void CanIncreaseProductQuantity()
        {
            //arrange
            //check initialization function

            //act
            this.cart.AddItem(p1, 1);
            this.cart.AddItem(p2, 10);
            CartLine[] result = this.cart.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 2);
            Assert.AreEqual(result[1].Quantity, 11);

        }

        [TestMethod]
        public void CanCalculateCartTotal()
        {
            //arrange
            //check initialization function

            //act
            this.cart.AddItem(p1, 2);
            this.cart.AddItem(p2, 1);
            decimal result = this.cart.ComputeTotalValue();

            //assert
            Assert.AreEqual(result, 400M);

        }

        [TestMethod]
        public void CanEmptyCart()
        {
            //arrange
            //check initialization function

            //act
            this.cart.Clear();
            int result = this.cart.Lines.Count();

            //assert
            Assert.AreEqual(result, 0);

        }

        [TestMethod]
        public void CanControllerAddItem()
        {
            //arrange done in initialization.

            //act 
            this.cartController.AddToCart(this.cart, 1, null);

            //assert
            Assert.AreEqual(this.cart.Lines.Count(), 2);
            Assert.AreEqual(this.cart.Lines.ToArray()[0].Product.ProductID, 1);

        }

        [TestMethod]
        public void CanRedirectToIndexAfterAddingItem()
        {
            //arrange

            //act
            var result = this.cartController.AddToCart(this.cart, 2, "myURL");

            //assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myURL");
        }

        [TestMethod]
        public void CanViewCartAndContinueShopping()
        {
            //arrange

            //act
            var result = (CartIndexViewModel)this.cartController.Index(this.cart, "myURL").ViewData.Model;

            //assert
            Assert.AreSame(result.Cart, this.cart);
            Assert.AreEqual(result.ReturnUrl, "myURL");
        }

        [TestMethod]
        public void CannotCheckoutWithEmptyCart()
        {
            //arrange
            //check initialization function
            
            //act
            //we use cart2 because we don't want to use a cart that has been fiddled with at all.
            ViewResult result = this.cartController.Checkout(this.cart2, this.shippingDetails);

            //assert
            this.mockOrderProcessor.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }


        [TestMethod]
        public void canBlockInvalidShippingDetails()
        {
            //arrange
            this.cartController.ModelState.AddModelError("error", "error");

            //act
            ViewResult result = this.cartController.Checkout(cart, this.shippingDetails);

            //assert
            this.mockOrderProcessor.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void canCheckoutAndSubmitOrder()
        {
            //arrange

            //act
            ViewResult result = this.cartController.Checkout(this.cart, this.shippingDetails);

            //assert
            this.mockOrderProcessor.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
