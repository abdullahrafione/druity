using DataAccess.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace ControlCenter.Controllers
{
    [Authorize]
    public class FinanceController : Controller
    {
        #region Props

        private readonly InvoiceProvider invoiceProvider;
        private readonly UserProvider userProvider;
        private readonly OrderProvider orderProvider;
        private readonly ProductProvider productProvider;
        private readonly FinanceProvider financeProvider;

        public FinanceController()
        {
            invoiceProvider = new InvoiceProvider();
            userProvider = new UserProvider();
            orderProvider = new OrderProvider();
            productProvider = new ProductProvider();
            financeProvider = new FinanceProvider();
        }

        #endregion

        #region Search

        public ActionResult SearchByAccountHead(ControlCenter.Models.Expense expense)
        {
            var result = financeProvider.GetExpensesByAccountId(expense.AccountHeadId);

            return View("~/Views/Shared/Search.cshtml", MappingSearch(result));
        }

        #endregion

        #region Ledger
        public ActionResult GetLedger()
        {
            var ledger = financeProvider.GetLedger();
            TempData["PerPersonShare"] = PerPersonShare(ledger.FirstOrDefault().Balance).ToString("##,##0");
            return View(MappingLedger(ledger));
        }
        #endregion

        #region Income
        public ActionResult AddIncome()
        {
            var accountHeads = financeProvider.GetAccountHead();

            return View(MapListtoIncome(accountHeads));
        }

        [HttpPost]
        public ActionResult AddIncome(Models.Income income)
        {
            financeProvider.AddIncome(AddIncomeMapping(income));
            financeProvider.EntryinLedger(income.Details, income.Amount, null);

            return RedirectToAction("GetIncome", "Finance");
        }

        public ActionResult GetIncome()
        {
            var income = financeProvider.GetIncome();

            return View(MappingViewIncome(income));
        }

        #endregion

        #region Expenses
        public ActionResult AddExpense()
        {
            var accountHeads = financeProvider.GetAccountHead();

            return View(MapListofAccountHead(accountHeads));
        }
        [HttpPost]
        public ActionResult AddExpense(Models.Expense expense)
        {
            financeProvider.AddExpense(AddExpenseMapping(expense));
            financeProvider.EntryinLedger(expense.Description, null, expense.Amount);

            return RedirectToAction("GetExpenses", "Finance");
        }

        public ActionResult GetExpenses()
        {
            var expenses = financeProvider.GetExpenses();

            return View(MappingViewExpenses(expenses));
        }
        #endregion

        #region AccountHeads
        public ActionResult AccountHeads()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AccountHead(Models.AccountHead accountHead)
        {
            financeProvider.AddAccountHead(MaptoAccountHead(accountHead));

            return RedirectToAction("AccountHeads", "Finance");
        }
        #endregion

        #region Invoice
        public ActionResult GetInvoice(int orderId)
        {
            var invoice = invoiceProvider.GetInvoicebyOrderId(orderId);
            var user = userProvider.GetUserByUserId(invoice.UserId);
            var orderDetails = orderProvider.GetOrderDetailByOrderId(orderId);
            var model = MaptoInvoiceModel(invoice, user, orderDetails);

            return View("~/Views/Finance/Invoice.cshtml", model);
        }
        #endregion

        #region Finances

        public ActionResult GetReceivables()
        {
            var receivables = GetReceivablesMapping(financeProvider.GetPendingStatus());

            return View(receivables);
        }

        public ActionResult InvoiceStatus(int invoiceId)
        {
            financeProvider.InvoicePaymentStatus(invoiceId);

            return RedirectToAction("GetReceivables", "Finance");
        }

        #endregion

        #region Common

        public ActionResult UpdateShippingCharges()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateShippingCharges(Models.Expense expense)
        {
            UpdateShipping(expense.AccountHeadName);

            return RedirectToAction("UpdateShippingCharges", "Finance");
        }

        #endregion

        #region Private

        private decimal PerPersonShare(decimal Balance)
        {
            decimal ActualBalance = Balance;
            return ActualBalance / 2;
        }
        private Models.InvoiceModel MaptoInvoiceModel(DataAccess.Domain.SaleInvoice sale, DataAccess.Domain.User user, List<DataAccess.Domain.OrderDetail> orderDetail)
        {
            Models.InvoiceModel mapped = new Models.InvoiceModel();
            mapped.Products = new List<Models.Product>();

            orderDetail.ForEach(x =>
            {
                mapped.Products.Add(new Models.Product
                {
                    CurrentPrice = x.UnitSalePrice,
                    Quantity = x.Quantity,
                    Amount = x.Quantity * x.UnitSalePrice,
                    Name = productProvider.GetProductName(x.ProductStock.ProductId)
                });
            });

            mapped.CreationTime = sale.CreationTime;
            mapped.GrandTotal = sale.GrandTotal;
            mapped.InvoiceId = sale.Id;
            mapped.OrderId = sale.OrderId;
            mapped.OrderTotal = sale.OrderTotal;
            mapped.PaymentMode = sale.PaymentMode;
            mapped.PaymentStatus = sale.PaymentStatus;
            mapped.ShippingFee = sale.ShippingFee;
            mapped.UserId = sale.UserId;

            mapped.Address = user.Address;
            mapped.City = user.City;
            mapped.EmailAddress = user.EmailAddress;
            mapped.FirstName = user.FirstName;
            mapped.LastName = user.LastName;
            mapped.PhoneNumber = user.Phone;

            return mapped;
        }

        private DataAccess.Domain.AccountHead MaptoAccountHead(Models.AccountHead account)
        {
            return new DataAccess.Domain.AccountHead
            {
                CreationTime = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                Type = account.AccountHeadName
            };
        }

        private Models.Expense MapListofAccountHead(List<DataAccess.Domain.AccountHead> accountHeads)
        {
            Models.Expense expense = new Models.Expense();
            expense.AccountHead = new List<Models.AccountHead>();

            accountHeads.ForEach(x =>
            {
                expense.AccountHead.Add(new Models.AccountHead
                {
                    AccountHeadId = x.Id,
                    AccountHeadName = x.Type
                });
            });

            return expense;
        }

        private DataAccess.Domain.Expense AddExpenseMapping(Models.Expense expense)
        {
            return new DataAccess.Domain.Expense
            {
                AccountHeadId = expense.AccountHeadId,
                Amount = expense.Amount,
                CreationTime = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                Description = expense.Description
            };
        }

        private List<Models.Expense> MappingViewExpenses(List<DataAccess.Domain.Expense> expenses)
        {
            List<Models.Expense> mapped = new List<Models.Expense>();

            expenses.ForEach(x =>
            {
                mapped.Add(new Models.Expense
                {
                    AccountHeadName = x.AccountHead.Type,
                    Amount = x.Amount,
                    CreatedOn = x.CreationTime,
                    Description = x.Description,
                    AccountHeadId = x.AccountHeadId
                });
            });

            return mapped;
        }

        private List<Models.InvoiceModel> GetReceivablesMapping(List<DataAccess.Domain.SaleInvoice> saleInvoices)
        {
            List<Models.InvoiceModel> mapped = new List<Models.InvoiceModel>();

            saleInvoices.ForEach(x =>
            {
                mapped.Add(new Models.InvoiceModel
                {
                    InvoiceId = x.Id,
                    UserId = x.UserId,
                    OrderTotal = x.OrderTotal,
                    ShippingFee = x.ShippingFee,
                    GrandTotal = x.GrandTotal,
                    PaymentStatus = x.PaymentStatus,
                    PaymentMode = x.PaymentMode,
                    OrderId = x.OrderId
                });
            });

            return mapped;
        }

        private Models.Income MapListtoIncome(List<DataAccess.Domain.AccountHead> accountHeads)
        {
            Models.Income income = new Models.Income();
            income.AccountHead = new List<Models.AccountHead>();

            accountHeads.ForEach(x =>
            {
                income.AccountHead.Add(new Models.AccountHead
                {
                    AccountHeadId = x.Id,
                    AccountHeadName = x.Type
                });
            });

            return income;
        }

        private DataAccess.Domain.Income AddIncomeMapping(Models.Income expense)
        {
            return new DataAccess.Domain.Income
            {
                AccountHeadId = expense.AccountHeadId,
                Amount = expense.Amount,
                CreationTime = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                Details = expense.Details
            };
        }

        private List<Models.Income> MappingViewIncome(List<DataAccess.Domain.Income> incomes)
        {
            List<Models.Income> mapped = new List<Models.Income>();

            incomes.ForEach(x =>
            {
                mapped.Add(new Models.Income
                {
                    AccountHeadName = x.AccountHead.Type,
                    Amount = x.Amount,
                    CreatedOn = x.CreationTime,
                    Details = x.Details
                });
            });

            return mapped;
        }

        private List<Models.LedgerModel> MappingLedger(List<DataAccess.Domain.Ledger> ledgers)
        {
            List<Models.LedgerModel> mapped = new List<Models.LedgerModel>();
            ledgers.ForEach(x =>
            {
                mapped.Add(new Models.LedgerModel
                {
                    Reference = x.Reference,
                    Income = x.Income,
                    Expense = x.Expense,
                    Balance = x.Balance,
                    CreationTime = x.CreationTime
                });
            });

            return mapped;
        }

        private List<Models.Search> MappingSearch(List<DataAccess.Domain.Expense> expenses)
        {
            List<Models.Search> mapped = new List<Models.Search>();

            expenses.ForEach(x =>
            {
                mapped.Add(new Models.Search
                {
                    AccountHeadName = x.AccountHead.Type,
                    Amount = x.Amount,
                    Description = x.Description,
                    CreatedOn = x.CreationTime
                });
            });

            return mapped;
        }

        protected void UpdateShipping(string shippingCharges)
        {
            Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection("appSettings");

            if (objAppsettings != null)
            {
                objAppsettings.Settings["ShippingCharges"].Value = shippingCharges;
                objConfig.Save();
            }
        }

        #endregion
    }
}