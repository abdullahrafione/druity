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

            return View(Mapping(usersCount, producsCount, activeOrders, ordersCount));

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
        private DashboardModel Mapping(int usersCount, int producsCount, int activeOrders, int ordersCount)
        {
            return new DashboardModel
            {
                UsersCount = usersCount,
                ProductsCount = producsCount,
                ActiveOrdersCount = activeOrders,
                OrdersCount = ordersCount
            };
        }
        #endregion
    }
}