using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderDAO
    {
        public static List<Order>? GetOrders()
        {
            List<Order>? orders = new List<Order>(); 
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    orders = context.Orders
                        .Include(x => x.User)
                        .ToList();
                }
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        public static Order? GetOrderById(int id)
        {
            Order? orders = new Order();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    orders = context.Orders
                        .Include(x => x.User)
                        .SingleOrDefault(x => x.OrderId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        public static void CreateOrder(Order o)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Orders.Add(o);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateOrder(Order o)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Entry<Order>(o).State 
                        = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteOrder(Order o)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Orders.Remove(o);
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
