using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EssentialTools.Models
{
    public class MinimumDiscountHelper : IDiscountHelper
    {
        public decimal ApplyDiscount(decimal total)
        {
            if (total > 100)
            {
                return total * .9M;
            }
            else if (total >= 10)
            {
                return total - 5M;
            }
            else if (total >= 0)
            {
                return total;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

        }
    }
}