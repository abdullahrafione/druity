using System.Collections.Generic;
using System.Linq;
using DataAccess.DbFactory;

namespace DataAccess.Providers
{
    public class ProductProvider
    {
        public int AddProduct(Domain.Product product)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var addedProduct = context.Product.Add(product);
                context.SaveChanges();

                return addedProduct.Id;
            }
        }
        public List<Domain.Category> GetCategories()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.Category.Where(x => x.IsDeleted == false && x.ParentCategoryId != 0).ToList();
            }
        }

        public Domain.Product GetProductByProductId(int productId)
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.Product.Include("Category").Where(x => x.Id == productId).FirstOrDefault();
            }
        }

        public int UpdateStock (Domain.ProductStock productStock)
        {
            using (DataDbContext context = new DataDbContext())
            {
                context.ProductStock.Add(productStock);
                context.SaveChanges();
                return productStock.ProductId;
            }
        }

        public void AddProductImages(Domain.Image image)
        {
            using (DataDbContext context = new DataDbContext())
            {
                context.Image.Add(image);
                context.SaveChanges();
            }
        }

        public List<Domain.Product> GetAllProducts()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.Product.OrderByDescending(x => x.CreationTime).ToList();
            }
        }

        public void UpdateProduct(Domain.Product product)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var obj = context.Product.Where(x => x.Id == product.Id).FirstOrDefault();
                obj.Name = product.Name;
                obj.ShortDescription = product.ShortDescription;
                obj.Detail = product.Detail;

                context.SaveChanges();
            }
        }
    }
}