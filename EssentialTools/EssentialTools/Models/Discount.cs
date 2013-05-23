using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EssentialTools.Models
{
    public interface IDiscountHelper
    {
        decimal ApplyDiscount(decimal total);
    }

    public class DefaultDiscountHelper : IDiscountHelper
    {
        public decimal DiscountSize;

        public DefaultDiscountHelper(decimal discount)
        {
            this.DiscountSize = discount;
        }

        public decimal ApplyDiscount(decimal total)
        {
            return (total - (this.DiscountSize / 100M * total));
        }
    }
}