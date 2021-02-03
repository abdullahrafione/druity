using DataAccess.DbFactory;
using DataAccess.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DataAccess.Providers
{
    public class InvoiceProvider
    {
        public SaleInvoice GetInvoicebyOrderId(int orderId)
        {
            using(DataDbContext context = new DataDbContext())
            {
                var order = context.SaleInvoice.Where(x => x.OrderId == orderId).FirstOrDefault();

                if(order != null) { 

                return context.SaleInvoice.Where(x => x.OrderId == orderId).FirstOrDefault();

                }

                int saleinvoiceId = GenerateSaleInvoice(orderId);
                return context.SaleInvoice.Where(x => x.Id == saleinvoiceId).FirstOrDefault();
            }
        }

        public int GenerateSaleInvoice(int orderId)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var order = context.Order.Where(x => x.Id == orderId).FirstOrDefault();
                var orderobject = CreateObject(order);
                var saleinvoice = context.SaleInvoice.Add(orderobject);
                context.SaveChanges();
                return saleinvoice.Id;
            }
        }

        private SaleInvoice CreateObject (Order order)
        {
            return new SaleInvoice
            {
                CreationTime = DateTime.Now,
                ShippingFee = Convert.ToDecimal(ConfigurationManager.AppSettings["ShippingCharges"]),
                PaymentStatus = "Pending",
                PaymentMode = "COD",
                OrderTotal = order.TotalAmount,
                UserId = order.UserId,
                GrandTotal = order.TotalAmount + Convert.ToDecimal(ConfigurationManager.AppSettings["ShippingCharges"]),
                OrderId = order.Id,
                IsActive = true,
                IsDeleted = false
            };
        }
    }
}