using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class InvoiceDAO
    {
        // Get invoices by current logined user
        public static List<Invoice?> GetInvoicesByCurrentUser(long currentUserId)
        {
            List<Invoice> invoices = new List<Invoice>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    invoices = context.Invoices
                        .Where(i => i.UserId == currentUserId)
                        .OrderByDescending(o => o.CreatedAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return invoices;
        }

        // Get invoice by orderid and current logined user
        public static Invoice? GetInvoiceByIdAndUserId(long invoiceId, long userId)
        {
            Invoice? invoice = new Invoice();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    invoice = context.Invoices
                        .Include(x => x.Orders)
                        .ThenInclude(x => x.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                        .Include(x => x.Orders)
                        .ThenInclude(x => x.Store)
                        .SingleOrDefault(i => i.InvoiceId == invoiceId && i.UserId == userId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return invoice;
        }

        public static void CreateInvoice(Invoice i)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Invoices.Add(i);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateInvoice(Invoice i)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Entry<Invoice>(i).State 
                        = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteInvoice(Invoice i)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Invoices.Remove(i);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
