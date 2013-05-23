using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EssentialTools.Models;
using System.Linq;
using Moq;

namespace EssentialTools.Tests
{
    [TestClass]
    public class UnitTest2
    {
        private Product[] products = {
            new Product {Name = "Kayak", Category = "Watersports", Price = 275M},
            new Product {Name = "Lifejacket", Category = "Watersports", Price = 48.95M},
            new Product {Name = "Soccer ball", Category = "Soccer", Price = 19.50M},
            new Product {Name = "Corner flag", Category = "Soccer", Price = 34.95M}
        };

        [TestMethod]
        public void CorrectlySumsProducts()
        {
            //arrange
            Mock<IDiscountHelper> mock = new Mock<IDiscountHelper>();
            mock.Setup(mockedClass => mockedClass.ApplyDiscount(It.IsAny<decimal>()))
                .Returns<decimal>(originalValuePassedToMockedFunction => originalValuePassedToMockedFunction);

            var target = new LinqValueCalculator(mock.Object);

            //act
            var result = target.ValueProducts(this.products);

            //assert
            Assert.AreEqual(this.products.Sum(p => p.Price), result);
        }

        private Product[] createProduct(decimal value)
        {
            return new[] { new Product { Price = value } };
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void PassThroughVariableDiscounts()
        {
            //arrange
            Mock<IDiscountHelper> mock = new Mock<IDiscountHelper>();
            mock.Setup(mockedObject => mockedObject.ApplyDiscount(It.IsAny<decimal>()))
                .Returns<decimal>(valuePassed => valuePassed);
            mock.Setup(mockedObject => mockedObject.ApplyDiscount(It.Is<decimal>(value => value == 0)))
                .Throws<System.ArgumentOutOfRangeException>();
            mock.Setup(mockedObject => mockedObject.ApplyDiscount(It.Is<decimal>(value => value > 100)))
                .Returns<decimal>(passedVal => (passedVal * 0.9M));
            mock.Setup(mockedObject => mockedObject.ApplyDiscount(It.IsInRange<decimal>(10,100,Range.Inclusive)))
                .Returns<decimal>(passedVal => (passedVal - 5M));

            var target = new LinqValueCalculator(mock.Object);

            //act
            decimal fiveDollarDiscount = target.ValueProducts(createProduct(5));
            decimal tenDollarDiscount = target.ValueProducts(createProduct(10));
            decimal fiftyDollarDiscount = target.ValueProducts(createProduct(50));
            decimal hundredDollarDiscount = target.ValueProducts(createProduct(100));
            decimal fiveHundredDollarDiscount = target.ValueProducts(createProduct(500));

            //assert
            Assert.AreEqual(5, fiveDollarDiscount);
            Assert.AreEqual(5, tenDollarDiscount);
            Assert.AreEqual(45, fiftyDollarDiscount);
            Assert.AreEqual(95, hundredDollarDiscount);
            Assert.AreEqual(450, fiveHundredDollarDiscount);
            target.ValueProducts(createProduct(0));
        }
    }
}
