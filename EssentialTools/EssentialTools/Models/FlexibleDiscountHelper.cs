using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EssentialTools.Models
{
    public class FlexibleDiscountHelper : IDiscountHelper
    {
        public decimal ApplyDiscount(decimal total)
        {
            decimal discount = total > 100 ? 70 : 25;
            return (total - (discount / 100M * total));
        }
    }
}