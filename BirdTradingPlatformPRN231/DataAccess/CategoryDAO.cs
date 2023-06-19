using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CategoryDAO
    {
        public static List<Category> GetCategories()
        {
            var listCategorys = new List<Category>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    listCategorys = context.Categories.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listCategorys;
        }
        public static Category FindCategoryById(int CategoryId)
        {
            Category f = new Category();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    f = context.Categories
                        .SingleOrDefault(x => x.CategoryId == CategoryId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return f;
        }
        public static void SaveCategory(Category Category)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Categories.Add(Category);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void UpdateCategory(Category Category)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Entry<Category>(Category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void DeleteCategory(Category Category)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    var p1 = context.Categories.SingleOrDefault(p => p.CategoryId == Category.CategoryId);
                    context.Categories.Remove(p1);
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
