using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderItemDAO
    {
        public static List<OrderItem>? GetOrderItems()
        {
            List<OrderItem>? orderItems = new List<OrderItem>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    orderItems = context.OrderItems
                        .Include(x => x.Order)
                        .Include(x => x.Product)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderItems;
        }

        public static OrderItem? GetOrderItemById(int orderId, int productId)
        {
            OrderItem? orderItems = new OrderItem();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    orderItems = context.OrderItems
                        .Include(x => x.Order)
                        .Include(x => x.Product)
                        .SingleOrDefault(x => x.OrderId == orderId && x.ProductId == productId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderItems;
        }

        public static void CreateOrderItem(OrderItem o)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.OrderItems.Add(o);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateOrderItem(OrderItem o)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Entry<OrderItem>(o).State
                        = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteOrderItem(OrderItem ot)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.OrderItems.Remove(ot);
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
