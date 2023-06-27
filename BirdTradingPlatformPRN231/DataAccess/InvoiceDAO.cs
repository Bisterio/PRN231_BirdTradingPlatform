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
