using OnlineShop.DbFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DAL
{
    public class CommonProvider
    {
        #region Color

        public void AddColor(string colour)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var addedOrder = context.Colour.Add(new DomainEntities.Colour() { Name = colour, Code = colour, IsDeleted = false });
                context.SaveChanges();
            }
        }

        #endregion

        #region Sizes

        public void AddSize(string size)
        {
            using (var db = new ShopDbContext())
            {
                db.Size.Add(new DomainEntities.Size() { Name = size, IsDeleted = false });
                db.SaveChanges();
            }
        }

        #endregion

    }
}