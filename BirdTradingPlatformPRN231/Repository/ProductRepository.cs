using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProductRepository : IProductRepository
    {
        // Get product list to be show on client page
        public ClientProductViewListDTO GetProductsPublic(int page, string? nameSearch, long categoryId, long priceMin, long priceMax, int orderBy)
        {
            // Handle query data
            int size = 12;
            page = page == 0 ? 1 : page;

            // Get paginated list
            List<ProductViewDTO?> paginatedProduct = ProductDAO
                .GetProductsPublic(page, size, nameSearch, categoryId, priceMin, priceMax, orderBy)
                .Select(x => ToProductViewDTO(x))
                .ToList();

            // Get count of products by search/filter
            int productCount = ProductDAO.CountAllProductPublic(nameSearch, categoryId, priceMin, priceMax);
            int totalPages = (int)Math.Ceiling((double)productCount / size);
            List<int> pageNumbers = new List<int>();
            if (totalPages > 0)
            {
                int start = Math.Max(1, page - 2);
                int end = Math.Min(page + 2, totalPages);

                if (totalPages > 5)
                {
                    if (end == totalPages) start = end - 4;
                    else if (start == 1) end = start + 4;
                }
                else
                {
                    start = 1;
                    end = totalPages;
                }
                pageNumbers = Enumerable.Range(start, end - start + 1).ToList();
            }

            return new ClientProductViewListDTO() {
                ProductsPaginated = paginatedProduct,
                Page = page,
                Size = size,
                PageNumbers = pageNumbers,
                TotalCount = productCount,
                TotalPage = totalPages
            };
        }

        // Get Product Detail By product Id
        public ProductViewDTO? GetProductDetailPublicById(long productId)
        {
            Product? entity = ProductDAO.GetProductDetailPublicById(productId);
            return ToProductViewDTO(entity);
        }

        // Get Product List By Store Id 
        public List<ProductViewDTO?> GetProductsPublicByStoreId(long storeId)
        {
            // Get product list by store
            List<ProductViewDTO?> productsByStore = ProductDAO
                .GetProductsPublicByStoreId(storeId)
                .Select(x => ToProductViewDTO(x))
                .ToList();
            return productsByStore;
        }

        // Map ProductEntity to ProductViewDTO
        public static ProductViewDTO? ToProductViewDTO(Product? entity)
        {
            if (entity == null) return null;

            return new ProductViewDTO()
            {
                ProductId = entity.ProductId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Description = entity.Description,
                Image = entity.Image,
                Name = entity.Name,
                Status = entity.Status,
                Stock = entity.Stock,
                UnitPrice = entity.UnitPrice,
                CategoryName = entity.Category?.Name,
                StoreName = entity.Store?.Name,
                StoreAddress = entity.Store?.Address
            };
        }
    }
}
