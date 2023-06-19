using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDAO
    {
        public static List<Product> GetProducts()
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    listProducts = context.Products
                        .Include(c => c.Category)
                        .Include(s => s.Store)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }
        public static Product FindProductById(int ProductId)
        {
            Product p = new Product();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    p = context.Products
                        .Include(c => c.Category)
                        .Include(s => s.Store)
                        .SingleOrDefault(x => x.ProductId == ProductId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return p;
        }
        public static void SaveProduct(Product Product)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Products.Add(Product);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void UpdateProduct(Product Product)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Entry<Product>(Product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void DeleteProduct(Product Product)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    var p1 = context.Products.SingleOrDefault(p => p.ProductId == Product.ProductId);
                    context.Products.Remove(p1);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
