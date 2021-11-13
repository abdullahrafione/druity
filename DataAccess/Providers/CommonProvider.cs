using System.Linq;
using DataAccess.DbFactory;
namespace DataAccess.Providers
{
    public class CommonProvider
    {
        #region DashboardCounts

        public int GetUsersCount()
        {
            using (DataDbContext context = new DataDbContext())
            {
                var count = context.User.Where(x => x.OrganisationId != 1).Count();
                return count;
            }
        }

        public int GetProductsCount()
        {
            using (DataDbContext context = new DataDbContext())
            {
                var count = context.Product.Count();
                return count;
            }
        }

        public int GetOrdersCount()
        {
            using (DataDbContext context = new DataDbContext())
            {
                var count = context.Order.Count();
                return count;
            }
        }

        public int ActiveOrders()
        {
            using (DataDbContext context = new DataDbContext())
            {
                var count = context.Order.Where(x => x.IsActive == true).Count();
                return count;
            }
        }

        public int OutOfStock()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.Product.Include("ProductStock").Where(x => x.ProductStock.FirstOrDefault().StockCount == 0).Count();
            }
        }

        //public int InProgressOrders()
        //{
        //    using (DataDbContext context = new DataDbContext())
        //    {
        //        var count = context.Orders.Where(x => x.Status == "In Progress").Count();
        //        return count;
        //    }
        //}

        //public int DeliveredOrders()
        //{
        //    using (DataDbContext context = new DataDbContext())
        //    {
        //        var count = context.Orders.Where(x => x.Status == "Delivered").Count();
        //        return count;
        //    }
        //}

        //public int DispatchedOrders()
        //{
        //    using (DataDbContext context = new DataDbContext())
        //    {
        //        var count = context.Orders.Where(x => x.Status == "Dispatched").Count();
        //        return count;
        //    }
        //}

        #endregion

        #region Category

        public void AddCategory(Domain.Category category)
        {
            using(DataDbContext context = new DataDbContext())
            {
                context.Category.Add(category);
                context.SaveChanges();
            }
        }

        #endregion
    }
}