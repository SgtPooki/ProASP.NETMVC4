using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductsRepository repository;
        public int productsPerPage = 4;

        public ProductController(IProductsRepository productRepo)
        {
            this.repository = productRepo;
        }

        public ViewResult List(string category, int page = 1)
        {
            IQueryable<Product> products = this.repository.Products;
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = products
                    .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.ProductID)
                    .Skip((page - 1) * this.productsPerPage)
                    .Take(this.productsPerPage),
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = this.productsPerPage,
                    TotalItems = category == null ?
                        products.Count() :
                        products.Where(p2=>p2.Category == category).Count()
                },
                CurrentCategory = category
            };

            return View(model);
        }

    }
}
