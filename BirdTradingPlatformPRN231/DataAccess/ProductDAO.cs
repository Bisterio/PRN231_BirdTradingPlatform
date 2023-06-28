using BusinessObject.DTOs;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDAO
    {
        // Function to get publicly visible products on client page
        public static List<Product> GetProductsPublic(int page, int size, string? nameSearch, long categoryId, long priceMin, long priceMax, int orderBy)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    var products = context.Products
                        .Where(p => p.Status == 1 & p.Stock > 0 // Get available product and stock > 0
                        && (string.IsNullOrEmpty(nameSearch) || p.Name.Contains(nameSearch)) // Search by name
                        && (categoryId == 0 || p.CategoryId == categoryId) // Search by categoryId, if categoryId = 0 then show all
                        && p.UnitPrice >= priceMin // Filter by minimum price
                        && (priceMax == 0 || p.UnitPrice <= priceMax) // Filter by maximum price if maximum price != 0
                        );

                    // Sort with order by: default (0): created at desc, 1: created at asc, 2: unit price desc, 3: unit price asc
                    switch (orderBy)
                    {
                        case 1:
                            products = products.OrderBy(p => p.CreatedAt);
                            break;
                        case 2:
                            products = products.OrderByDescending(p => p.UnitPrice);
                            break;
                        case 3:
                            products = products.OrderBy(p => p.UnitPrice);
                            break;
                        default:
                            products = products.OrderByDescending(p => p.CreatedAt);
                            break;
                    }

                    // Pagination and include category and store associated with product
                    listProducts = products
                        .Skip((page - 1) * size)
                        .Take(size)
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

        // Function to get count of publicly visible products on client page
        public static int CountAllProductPublic(string? nameSearch, long categoryId, long priceMin, long priceMax)
        {
            int count = 0;
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    count = context.Products
                        .Where(p => p.Status == 1 & p.Stock > 0 // Get available product and stock > 0
                        && (string.IsNullOrEmpty(nameSearch) || p.Name.Contains(nameSearch)) // Search by name
                        && (categoryId == 0 || p.CategoryId == categoryId) // Search by categoryId, if categoryId = 0 then show all
                        && p.UnitPrice >= priceMin // Filter by minimum price
                        && (priceMax == 0 || p.UnitPrice <= priceMax)) // Filter by maximum price if maximum price != 0
                        .Count();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return count;
        }

        // Function to get product detail that has status = 1
        public static Product? GetProductDetailPublicById(long productId)
        {
            var product = new Product();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    product = context.Products
                        .Include(p => p.Store)
                        .Include(p => p.Category)
                        .SingleOrDefault(p => p.ProductId == productId && p.Status == 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        // Function to get all products by a store id (status =1,stock>0)
        public static List<Product> GetProductsPublicByStoreId(long storeId)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    listProducts = context.Products
                        .Where(p => p.Status == 1 & p.Stock > 0 // Get available product and stock > 0
                        && p.StoreId == storeId // Filter by store id
                        && p.Store.Status == 1) // Store has to be available
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

        // Function to get product detail
        public static Product? GetProductDetailById(long productId)
        {
            var product = new Product();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    product = context.Products
                        .SingleOrDefault(p => p.ProductId == productId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        // Get orders by status and currently logined store
        public static List<Product?> GetProductsByCurrentStore(int page, int size, string? nameSearch, long categoryId, long priceMin, long priceMax, int orderBy, long currentStoreUserId)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    var products = context.Products
                        .Where(p => p.Status == 1 // Get available product 
                        && (p.Store.UserId == currentStoreUserId) // Get by currently logined storeId
                        && (string.IsNullOrEmpty(nameSearch) || p.Name.Contains(nameSearch)) // Search by name
                        && (categoryId == 0 || p.CategoryId == categoryId) // Search by categoryId, if categoryId = 0 then show all
                        && p.UnitPrice >= priceMin // Filter by minimum price
                        && (priceMax == 0 || p.UnitPrice <= priceMax) // Filter by maximum price if maximum price != 0
                        );

                    // Sort with order by: default (0): created at desc, 1: created at asc, 2: unit price desc, 3: unit price asc
                    switch (orderBy)
                    {
                        case 1:
                            products = products.OrderBy(p => p.CreatedAt);
                            break;
                        case 2:
                            products = products.OrderByDescending(p => p.UnitPrice);
                            break;
                        case 3:
                            products = products.OrderBy(p => p.UnitPrice);
                            break;
                        default:
                            products = products.OrderByDescending(p => p.CreatedAt);
                            break;
                    }

                    // Pagination and include category and store associated with product
                    listProducts = products
                        .Skip((page - 1) * size)
                        .Take(size)
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

        // Function to get count of products of currently logined store
        public static int CountAllProductCurrentStore(string? nameSearch, long categoryId, long priceMin, long priceMax, long currentStoreUserId)
        {
            int count = 0;
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    count = context.Products
                        .Where(p => p.Status == 1 // Get available product
                        && (p.Store.UserId == currentStoreUserId) // Get by current logined storeId
                        && (string.IsNullOrEmpty(nameSearch) || p.Name.Contains(nameSearch)) // Search by name
                        && (categoryId == 0 || p.CategoryId == categoryId) // Search by categoryId, if categoryId = 0 then show all
                        && p.UnitPrice >= priceMin // Filter by minimum price
                        && (priceMax == 0 || p.UnitPrice <= priceMax)) // Filter by maximum price if maximum price != 0
                        .Count();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return count;
        }

        // Function to get product detail of currently logined store that has status = 1
        public static Product? GetProductDetailByCurrentStore(long productId, long currentStoreUserId)
        {
            var product = new Product();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    product = context.Products
                        .Where(p => p.Store.UserId == currentStoreUserId)
                        .Include(p => p.Store)
                        .Include(p => p.Category)
                        .SingleOrDefault(p => p.ProductId == productId && p.Status == 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
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
