using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EssentialTools.Models;


namespace EssentialTools.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private IDiscountHelper getTestObject()
        {
            return new MinimumDiscountHelper();
        }

        [TestMethod]
        public void DiscountIs10PercentOver100() //discount should be 10%
        {
            //arrange
            IDiscountHelper target = this.getTestObject();
            decimal total = 200;

            //act
            var discountedTotal = target.ApplyDiscount(total);

            //assert
            Assert.AreEqual(total * 0.9M, discountedTotal);
        }

        [TestMethod]
        public void DiscountIs5DollarsBetween10And100()
        {
            //arrange
            IDiscountHelper target = this.getTestObject();
            
            decimal total1 = 100; //high end edge case
            decimal total2 = 55; //middle
            decimal total3 = 10;  //low end edge case

            //act
            var withDiscount1 = target.ApplyDiscount(total1);
            var withDiscount2 = target.ApplyDiscount(total2);
            var withDiscount3 = target.ApplyDiscount(total3);

            //assert
            Assert.AreEqual(total1 - 5M, withDiscount1);
            Assert.AreEqual(total2 - 5M, withDiscount2);
            Assert.AreEqual(total3 - 5M, withDiscount3);
        }

        [TestMethod]
        public void noDiscountUnder10Dollars()
        {
            //arrange
            IDiscountHelper target = this.getTestObject();

            decimal total = 9;

            //act
            var withDiscount = target.ApplyDiscount(total);

            //assert
            Assert.AreEqual(withDiscount, total);
            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))] //assert
        public void NegativeTotalsNotAllowed()
        {
            //arrange
            IDiscountHelper target = getTestObject();

            //act
            target.ApplyDiscount(-1);
        }
        
    }
}
