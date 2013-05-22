using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using LanguageFeatures.Models;

namespace LanguageFeatures.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "Navigate to a URL to show an example";
        }

        public ViewResult AutoProperty()
        {
            //create a new Product object
            Product myProduct = new Product();

            //set the property value
            myProduct.Name = "Kayak";

            //get the property
            string productName = myProduct.Name;

            //generate the view
            return View("Result", (object)String.Format("Product Name: {0}", productName));
        }

        public ViewResult CreateProduct()
        {
            //create and populate a new Product object
            Product myProduct = new Product
            {
                productID = 100, 
                Name = "Kayak2", 
                Description = "A canoe for people with a short leg complex",
                price = 275M, 
                category = "Watersports"
            };

            return View("Result", (object)String.Format("Category: {0}", myProduct.category));
        }

        public ViewResult CreateCollection()
        {
            string[] stringArray = { "pear", "banana", "watermelon" };

            List<int> intList = new List<int> { 10, 20, 30, 40 };

            Dictionary<string, int> myDict = new Dictionary<string, int> { { "pear", 10 }, { "banana", 20 }, { "watermelon", 30 } };

            return View("Result", (object)stringArray[1]);
        }

        public ViewResult UseExtension()
        {
            //create and populate a ShoppingCart instance
            ShoppingCart cart = new ShoppingCart {
                Products = new List<Product> {
                    new Product {Name = "Kayak", price = 275M},
                    new Product {Name = "LifeJacket", price = 48.95M},
                    new Product {Name = "Hacky Sack", price = 2M},
                    new Product {Name = "Kayak Paddle", price = 175M},
                    new Product {Name = "Kayak cargo attachment", price = 75M}
                }
            };

            //get the total value of the products in the cart
            decimal cartTotal = cart.TotalPrices();

            return View("Result", (object)String.Format("Total: {0:c}", cartTotal));
        }

        public ViewResult UseExtensionEnumerable()
        {
            IEnumerable<Product> products = new ShoppingCart
            {
                Products = new List<Product> {
                    new Product {Name = "Kayak", price = 275M},
                    new Product {Name = "LifeJacket", price = 48.95M},
                    new Product {Name = "Hacky Sack", price = 2M},
                    new Product {Name = "Kayak Paddle", price = 175M},
                    new Product {Name = "Kayak cargo attachment", price = 75M}
                }
            };

            Product[] productArray = {
                    new Product {Name = "Kayak", price = 275M},
                    new Product {Name = "LifeJacket", price = 48.95M},
                    new Product {Name = "Hacky Sack", price = 2M},
                    new Product {Name = "Kayak Paddle", price = 175M},
                    new Product {Name = "Kayak cargo attachment", price = 75M}
            };

            //get the total value of the products in the cart
            decimal cartTotal = products.TotalPrices();
            decimal arrayTotal = productArray.TotalPrices();

            return View("Result", (object)String.Format("Cart Total: {0:c}, Array Total: {1:c}", cartTotal, arrayTotal));
        }

        public ViewResult UseFilterExtensionMethod()
        {
            IEnumerable<Product> products = new ShoppingCart
            {
                Products = new List<Product> {
                    new Product {Name = "Kayak", price = 275M, category = "Watersports"},
                    new Product {Name = "LifeJacket", price = 48.95M, category = "Watersports"},
                    new Product {Name = "Hacky Sack", price = 2M, category = "Landsports"},
                    new Product {Name = "Kayak Paddle", price = 175M, category = "Watersports"},
                    new Product {Name = "Kayak cargo attachment", price = 75M, category = "Watersport Utilities"}
                }
            };

            decimal total = 0;
            foreach (Product prod in products.FilterByCategory("Watersports"))
            {
                total += prod.price;
            }

            Func<Product, bool> categoryFilter = delegate(Product prod)
            {
                return prod.category == "Watersports";
            };

            decimal total1 = 0;
            foreach (Product prod in products.Filter(categoryFilter))
            {
                total1 += prod.price;
            }


            Func<Product, bool> categoryFilterLambda = prod => prod.category == "Watersports";

            decimal total2 = 0;
            foreach (Product prod in products.Filter(categoryFilterLambda))
            {
                total2 += prod.price;
            }


            decimal total3 = 0;
            foreach (Product prod in products.Filter(prod => prod.category == "Watersports"))
            {
                total3 += prod.price;
            }

            decimal total4 = 0;
            foreach (Product prod in products.Filter(prod => prod.category == "Watersports" || prod.price > 20M))
            {
                total4 += prod.price;
            }

            return View("Result", (object)String.Format("Total: {0:c}, Total1: {1:c}, Total2: {2:c}, Total3: {3:c}, Total4: {4:c}", total, total1, total2, total3, total4));
 

        }

        public ViewResult CreateAnonArray()
        {
            var oddsAndEnds = new[] {
                new { Name = "MVC", Category = "Pattern"},
                new { Name = "Hat", Category = "Clothing"},
                new { Name = "Apple", Category = "Fruit"}
            };

            StringBuilder result = new StringBuilder();
            foreach (var item in oddsAndEnds)
            {
                result.Append(item.Name + " ");
            }

            return View("Result", (object)result.ToString());
        }

        public ViewResult FindProducts()
        {
            Product[] products = {
                    new Product {Name = "Kayak", price = 275M, category = "Watersports"},
                    new Product {Name = "LifeJacket", price = 48.95M, category = "Watersports"},
                    new Product {Name = "Hacky Sack", price = 2M, category = "Landsports"},
                    new Product {Name = "Kayak Paddle", price = 175M, category = "Watersports"},
                    new Product {Name = "Kayak cargo attachment", price = 75M, category = "Watersport Utilities"}
            };

            //define the array to hold the results
            Product[] foundProducts = new Product[3];

            //sort the contents of the array
            Array.Sort(products, (item1, item2) => {
                return Comparer<decimal>.Default.Compare(item1.price, item2.price);
            });

            //get first three items
            Array.Copy(products, foundProducts, 3);

            //create result
            StringBuilder result = new StringBuilder();
            foreach (Product p in foundProducts)
            {
                result.AppendFormat("Price: {0:c} ", p.price);
            }

            return View("Result", (object)result.ToString());
        }

        //using query syntax LINQ
        public ViewResult FindProductsLINQ() 
        {
            Product[] products = {
                    new Product {Name = "Kayak", price = 275M, category = "Watersports"},
                    new Product {Name = "LifeJacket", price = 48.95M, category = "Watersports"},
                    new Product {Name = "Hacky Sack", price = 2M, category = "Landsports"},
                    new Product {Name = "Kayak Paddle", price = 175M, category = "Watersports"},
                    new Product {Name = "Kayak cargo attachment", price = 75M, category = "Watersport Utilities"}
            };

            var foundProducts = from match in products
                                orderby match.price descending
                                select new
                                {
                                    match.Name,
                                    match.price
                                };

            //create result
            int count = 0;
            StringBuilder result = new StringBuilder();
            foreach (var p in foundProducts)
            {
                if (count++ == 3)
                {
                    break;
                }
                result.AppendFormat("Price: {0:c} ", p.price);
            }

            return View("Result", (object)result.ToString());
        }

        //using dot notation LINQ
        public ViewResult FindProductsLINQ2()
        {
            Product[] products = {
                    new Product {Name = "Kayak", price = 275M, category = "Watersports"},
                    new Product {Name = "LifeJacket", price = 48.95M, category = "Watersports"},
                    new Product {Name = "Hacky Sack", price = 2M, category = "Landsports"},
                    new Product {Name = "Kayak Paddle", price = 175M, category = "Watersports"},
                    new Product {Name = "Kayak cargo attachment", price = 75M, category = "Watersport Utilities"}
            };

            var foundProducts = products.OrderByDescending(p => p.price)
                                .Take(3)
                                .Select(p => new {
                                    p.Name,
                                    p.price
                                });

            //create result
            StringBuilder result = new StringBuilder();
            foreach (var p in foundProducts)
            {
                result.AppendFormat("Price: {0:c} ", p.price);
            }

            return View("Result", (object)result.ToString());
        }


        //using deferred LINQ methods
        public ViewResult FindProductsLINQDeferred()
        {
            Product[] products = {
                    new Product {Name = "Kayak", price = 275M, category = "Watersports"},
                    new Product {Name = "LifeJacket", price = 48.95M, category = "Watersports"},
                    new Product {Name = "Hacky Sack", price = 2M, category = "Landsports"},
                    new Product {Name = "Kayak Paddle", price = 175M, category = "Watersports"},
                    new Product {Name = "Kayak cargo attachment", price = 75M, category = "Watersport Utilities"}
            };

            var foundProducts = products.OrderByDescending(p => p.price)
                                .Take(3)
                                .Select(p => new
                                {
                                    p.Name,
                                    p.price
                                });

            //The above query has not yet been performed. It is deferred so it waits until the data is iterated over (i.e. enumerated)
            products[2] = new Product { Name = "Stadium", price = 79600M };

            StringBuilder result = new StringBuilder();
            foreach (var p in foundProducts)
            {
                result.AppendFormat("Price: {0:c} ", p.price);
            }

            return View("Result", (object)result.ToString());
        }


        public ViewResult SumProducts()
        {
            Product[] products = {
                    new Product {Name = "Kayak", price = 275M, category = "Watersports"},
                    new Product {Name = "LifeJacket", price = 48.95M, category = "Watersports"},
                    new Product {Name = "Hacky Sack", price = 2M, category = "Landsports"},
                    new Product {Name = "Kayak Paddle", price = 175M, category = "Watersports"},
                    new Product {Name = "Kayak cargo attachment", price = 75M, category = "Watersport Utilities"}
            };


            var results = products.Sum(p => p.price); //Sum is not deferred and therefore is executed immediately.

            products[2] = new Product { Name = "Stadium", price = 79600M }; //this product is not included in the sum returned to the results variable.


            return View("Result", (object)String.Format("Sum: {0:c}", results));
        }
    }
}
