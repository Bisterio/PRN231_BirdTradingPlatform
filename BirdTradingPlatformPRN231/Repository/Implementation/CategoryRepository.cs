using BusinessObject.Models;
using DataAccess;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAllCategories()
        {
            return CategoryDAO.GetCategories();
        }
    }
}
