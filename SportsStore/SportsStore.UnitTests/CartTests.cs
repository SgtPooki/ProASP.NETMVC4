using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        public Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
        public Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M};
        public Cart cart = new Cart();

        [TestInitialize]
        public void InitializeTests()
        {
            this.cart.AddItem(p1, 1);
            this.cart.AddItem(p2, 1);
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
    }
}
