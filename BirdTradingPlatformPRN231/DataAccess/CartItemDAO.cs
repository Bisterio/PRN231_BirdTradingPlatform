using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CartItemDAO
    {
        public static List<CartItem>? GetCartItems()
        {
            List<CartItem>? cartItems = new List<CartItem>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    cartItems = context.CartItems
                        .Include(x => x.Product)
                        .Include(x => x.User)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return cartItems;
        }

        public static CartItem? GetCartItemById(int productId, int userId)
        {
            CartItem? cartItems = new CartItem();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    cartItems = context.CartItems
                        .Include(x => x.Product)
                        .Include(x => x.User)
                        .SingleOrDefault(x => x.ProductId == productId && x.UserId == userId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return cartItems;
        }

        public static void CreateCartItem(CartItem o)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.CartItems.Add(o);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateCartItem(CartItem o)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Entry<CartItem>(o).State
                        = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteCartItem(CartItem c)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.CartItems.Remove(c);
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
