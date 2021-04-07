using OnlineShop.DbFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OnlineShop.DAL
{
    public class ImageProvider
    {
        public List<Models.Image> SaveImage(Models.Image image)
        {
            DomainEntities.Image img = new DomainEntities.Image
            {
                Name = image.Name,
                DisplayOrder = image.DisplayOrder,
                ProductId = image.ProductId,
                Url = image.Url,
                IsPrimary = image.IsPrimary,
                IsSizeGuide = image.IsSizeGuide,
                IsActive = true
            };
            using (ShopDbContext context = new ShopDbContext())
            {
                context.Image.Add(img);
                context.SaveChanges();
            }

            return this.GetImages(image.ProductId);
        }

        public string DeleteImageAndReturnName(int imageId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var image = context.Image.Where(x => x.Id == imageId).FirstOrDefault();
                context.Image.Remove(image);
                context.SaveChanges();
                return image.Name;
            }
        }

        public List<Models.Image> GetImages(int productId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                List<Models.Image> modelList = new List<Models.Image>();
                var images = context.Image.Where(x => x.ProductId == productId).OrderByDescending(x => x.DisplayOrder).ToList();
                images.ForEach(c =>
                {
                    Models.Image model = new Models.Image
                    {
                        Name = c.Name,
                        DisplayOrder = c.DisplayOrder,
                        ProductId = c.ProductId,
                        Url = c.Url,
                        IsPrimary = c.IsPrimary,
                        IsSizeGuide = c.IsSizeGuide,
                        ImageId = c.Id
                    };
                    modelList.Add(model);
                });
                if (modelList.Count==0)
                {
                    modelList.Add(new Models.Image
                    {
                        Name = "default",
                        Url = ConfigurationManager.AppSettings["DefaultImageUrl"]
                    });
                }
                return modelList;
            }

        }

        public List<DomainEntities.GeneralImages> GetGeneralImages()
        {
            using (ShopDbContext context = new ShopDbContext())
            {
               return context.GeneralImages.ToList();
            }
        }
    }
}