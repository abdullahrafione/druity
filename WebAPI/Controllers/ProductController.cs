using OnlineShop.DAL;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductController : ApiController
    {
        private readonly ProductProvider productProvider;
        private readonly ImageProvider imageProvider;
        public ProductController()
        {
            productProvider = new ProductProvider();
            imageProvider = new ImageProvider();
        }

        [Route("categories")]
        [HttpGet]
        public IHttpActionResult Categories()
        {
            var categories = productProvider.GetCategories();
            var parentcategories = categories.Where(x => x.ParentCategoryId == 0).ToList();

            List<ParentChildCategory> mappedList = new List<ParentChildCategory>();
            parentcategories.ForEach(c =>
            {
                ParentChildCategory model = new ParentChildCategory();
                model.ParentCategoryId = c.CategoryId;
                model.ParentCategoryName = c.CategoryName;
                model.ChildCategories = categories.Where(x => x.ParentCategoryId == c.CategoryId).ToList();
                mappedList.Add(model);
            });
            mappedList.FirstOrDefault().ParentCategories = parentcategories;
            return Ok(mappedList);
        }

        public IHttpActionResult Detail(int id)
        {
            var products = productProvider.GetbyId(id);
            products.Products.ForEach(x =>
            {
                x.Image = imageProvider.GetImages(x.ProductId);
            });
            return Ok(products.Products.FirstOrDefault());
        }
    }
}
