﻿using BusinessObject.Models;
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
        // Get orders by status and current logined user
        public static List<Order?> GetOrdersByCurrentUser(int page, int size, byte status, long currentUserId)
        {
            List<Order> orders = new List<Order>(); 
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    orders = context.Orders
                        .Where(o => o.Invoice.UserId == currentUserId
                        && (status == 0 || o.Status == status))
                        .Include(o => o.Store)
                        .Include(o => o.Invoice).ThenInclude(o => o.User)
                        .OrderByDescending(o => o.CreatedAt)
                        .Skip((page - 1) * size)
                        .Take(size)
                        .ToList();
                }
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        // Function to get count of orders of a logined user by status
        public static int CountOrdersByCurrentUser(byte status, long currentUserId)
        {
            int count = 0;
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    count = context.Orders
                        .Where(o => o.Invoice.UserId == currentUserId
                        && (status == 0 || o.Status == status))
                        .Count();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return count;
        }

        // Get Order by orderid and current logined user
        public static Order? GetOrderByIdAndUserId(long orderId, long userId)
        {
            Order? order = new Order();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    order = context.Orders
                        .Include(x => x.Store)
                        .Include(x => x.Invoice)
                        .Include(x => x.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category)
                        .SingleOrDefault(o => o.OrderId == orderId && o.Invoice.UserId == userId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
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
