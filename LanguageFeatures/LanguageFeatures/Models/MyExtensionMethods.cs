using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanguageFeatures.Models
{
    public static class MyExtensionMethods
    {
        public static decimal TotalPrices(this IEnumerable<Product> productEnum) 
        {
            decimal total = 0;
            foreach (Product prod in productEnum)
            {
                total += prod.price;
            }
            return total;
        }

        public static IEnumerable<Product> FilterByCategory(this IEnumerable<Product> product, string category)
        {
            foreach (Product prod in product)
            {
                if (prod.category == category)
                {
                    yield return prod;
                }
            }
        }

        public static IEnumerable<Product> Filter(this IEnumerable<Product> product, Func<Product, bool> selector)
        {
            foreach (Product prod in product)
            {
                if (selector(prod))
                {
                    yield return prod;
                }
            }
        }

    }
}