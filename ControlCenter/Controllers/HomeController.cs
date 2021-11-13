using System.Web.Mvc;
using DataAccess.Providers;
using ControlCenter.Models;

namespace ControlCenter.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Constructors
        private readonly CommonProvider commonProvider;

        public HomeController()
        {
            commonProvider = new CommonProvider();
        }
        #endregion

        #region Actions
        public ActionResult Index()
        {
            DashboardModel dashboardModel = new DashboardModel();

            int usersCount = commonProvider.GetUsersCount();
            int producsCount = commonProvider.GetProductsCount();
            int activeOrders = commonProvider.ActiveOrders();
            int ordersCount = commonProvider.GetOrdersCount();
            int outofStock = commonProvider.OutOfStock();

            return View(Mapping(usersCount, producsCount, activeOrders, ordersCount, outofStock));

        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        #endregion

        #region Private
        private DashboardModel Mapping(int usersCount, int producsCount, int activeOrders, int ordersCount, int outOfStock)
        {
            return new DashboardModel
            {
                UsersCount = usersCount,
                ProductsCount = producsCount,
                ActiveOrdersCount = activeOrders,
                OrdersCount = ordersCount,
                outOfStock = outOfStock
            };
        }
        #endregion
    }
}