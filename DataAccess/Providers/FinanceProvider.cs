using DataAccess.DbFactory;
using DataAccess.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccess.Providers
{
    public class FinanceProvider
    {
        public void AddAccountHead(AccountHead accountHead)
        {
            using (DataDbContext context = new DataDbContext())
            {
                context.AccountHead.Add(accountHead);
                context.SaveChanges();
            }
        }

        public List<AccountHead> GetAccountHead()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.AccountHead.ToList();
            }
        }

        public void AddExpense(Expense expense)
        {
            using (DataDbContext context = new DataDbContext())
            {
                context.Expense.Add(expense);
                context.SaveChanges();
            }
        }

        public void AddIncome(Income income)
        {
            using(DataDbContext context = new DataDbContext())
            {
                context.Income.Add(income);
                context.SaveChanges();
            }
        }

        public List<Income> GetIncome()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.Income.Include("AccountHead").OrderByDescending(x=>x.CreationTime).ToList();
            }
        }

        public List<Expense> GetExpenses()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.Expense.Include("AccountHead").OrderByDescending(x => x.CreationTime).ToList();
            }
        }

        public List<SaleInvoice> GetPendingStatus()
        {
            using(DataDbContext context = new DataDbContext())
            {
                return context.SaleInvoice.Where(x => x.PaymentStatus == "Pending").ToList();
            }
        }

        public void InvoicePaymentStatus(int invoiceId)
        {
            using(DataDbContext context = new DataDbContext())
            {
                var row = context.SaleInvoice.Where(x => x.Id == invoiceId).FirstOrDefault();
                row.PaymentStatus = "Received";
                context.SaveChanges();

                var reference = "Received against Invoice # " + invoiceId;
                EntryinLedger(reference, row.OrderTotal, null);
            }
        }

        public List<Ledger> GetLedger()
        {
            using(DataDbContext context = new DataDbContext())
            {
                return context.Ledger.OrderByDescending(x=>x.CreationTime).ToList();
            }
        }

        public List<DataAccess.Domain.Expense> GetExpensesByAccountId(int accountId)
        {
            using(DataDbContext context = new DataDbContext())
            {
                return context.Expense.Include("AccountHead").Where(x => x.AccountHeadId == accountId).OrderByDescending(x => x.CreationTime).ToList();
            }
        }

        #region Ledger

        public void EntryinLedger(string reference, decimal? income, decimal? expense)
        {
            using (DataDbContext context = new DataDbContext())
            {
                if (income != null)
                {
                    Domain.Ledger ledger = new Ledger();
                    ledger.Reference = reference;
                    ledger.Income = (decimal)income;
                    ledger.Expense = 0;
                    ledger.Balance = GetUpdatedLedgerBalance(income, expense);
                    ledger.CreationTime = DateTime.Now;
                    ledger.IsActive = true;
                    ledger.IsDeleted = false;

                    context.Ledger.Add(ledger);
                    context.SaveChanges();
                }

                else
                {
                    Domain.Ledger ledger = new Ledger();
                    ledger.Reference = reference;
                    ledger.Income = 0;
                    ledger.Expense = (decimal)expense;
                    ledger.Balance = GetUpdatedLedgerBalance(income, expense);
                    ledger.CreationTime = DateTime.Now;
                    ledger.IsActive = true;
                    ledger.IsDeleted = false;

                    context.Ledger.Add(ledger);
                    context.SaveChanges();
                }
            }
        }

        public decimal GetUpdatedLedgerBalance(decimal? income, decimal? expense)
        {
            using (DataDbContext context = new DataDbContext())
            {
                if(income != null)
                {
                    var row = context.Ledger.OrderByDescending(x => x.CreationTime).FirstOrDefault();
                    var updateBalance = row.Balance + income;
                    return (decimal)updateBalance;
                }
                else
                {
                    var row = context.Ledger.OrderByDescending(x => x.CreationTime).FirstOrDefault();
                    var updateBalance = row.Balance - expense;
                    return (decimal)updateBalance;
                }
            }
        }

        #endregion
    }
}