using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
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
                .Select(x => Mapper.ToProductViewDTO(x))
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

            // Get categories list for filtering
            List<Category> categories = CategoryDAO.GetCategories();

            return new ClientProductViewListDTO()
            {
                ProductsPaginated = paginatedProduct,
                Page = page,
                Size = size,
                PageNumbers = pageNumbers,
                TotalCount = productCount,
                TotalPage = totalPages,
                Categories = categories,
                Category = categoryId,
                Name = nameSearch,
                Order = orderBy,
                Pmin = priceMin,
                Pmax = priceMax
            };
        }

        // Get Product Detail By product Id
        public ProductViewDTO? GetProductDetailPublicById(long productId)
        {
            Product? entity = ProductDAO.GetProductDetailPublicById(productId);
            if (entity == null) return null;
            ProductViewDTO? dto = Mapper.ToProductViewDTO(entity);

            // Get related products
            List<ProductViewDTO?> relatedProducts = ProductDAO
                .GetRelatedProductsByCategoryId(entity.CategoryId, productId)
                .Select(x => Mapper.ToProductViewDTO(x))
                .ToList();
            dto.RelatedProducts = relatedProducts;
            return dto;
        }

        // Get product list of currently logined store
        public ClientProductViewListDTO GetProductsStore(int page, string? nameSearch, long categoryId, long priceMin, long priceMax, int orderBy, long currentUserId)
        {
            // Handle query data
            int size = 12;
            page = page == 0 ? 1 : page;

            // Get current store
            Store currentStore = StoreDAO.GetStoreByUserId(currentUserId);

            if (currentStore != null)
            {
                // Get paginated list
                List<ProductViewDTO?> paginatedProduct = ProductDAO
                    .GetProductsByCurrentStore(page, size, nameSearch, categoryId, priceMin, priceMax, orderBy, currentStore.StoreId)
                    .Select(x => Mapper.ToProductViewDTO(x))
                    .ToList();

                // Get count of products by search/filter
                int productCount = ProductDAO.CountAllProductCurrentStore(nameSearch, categoryId, priceMin, priceMax, currentStore.StoreId);
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

                return new ClientProductViewListDTO()
                {
                    ProductsPaginated = paginatedProduct,
                    Page = page,
                    Size = size,
                    PageNumbers = pageNumbers,
                    TotalCount = productCount,
                    TotalPage = totalPages
                };
            }

            return null;
        }

        // Get product detail of currently logined store
        public ProductViewDTO GetProductDetailStore(long productId, long currentUserId)
        {
            // Get current store
            Store currentStore = StoreDAO.GetStoreByUserId(currentUserId);

            Product? entity = new Product();

            if (currentStore != null)
            {
                entity = ProductDAO.GetProductDetailByCurrentStore(productId, currentStore.StoreId);
                if (entity != null)
                    return Mapper.ToProductViewDTO(entity);
            }

            return null;
        }

        // Get Product List By Store Id 
        public List<ProductViewDTO?> GetProductsPublicByStoreId(long storeId)
        {
            // Get product list by store
            List<ProductViewDTO?> productsByStore = ProductDAO
                .GetProductsPublicByStoreId(storeId)
                .Select(x => Mapper.ToProductViewDTO(x))
                .ToList();
            return productsByStore;
        }

        // Add new product to the store
        public APIResult<long> AddProduct(ProductCreateDTO product, long currentUserId)
        {
            // Get current store
            Store currentStore = StoreDAO.GetStoreByUserId(currentUserId);

            if(product != null && currentStore.StoreId != null)
            {
                Product newProduct = new()
                {
                    ProductId = 0,
                    CreatedAt = DateTime.Now,
                    Description = product.Description,
                    Image = product.Image,
                    Name = product.Name,
                    Status = 1,
                    Stock = product.Stock,
                    UnitPrice = product.UnitPrice,
                    UpdatedAt = DateTime.Now,
                    CategoryId = product.CategoryId,
                    StoreId = currentStore.StoreId
                };

                ProductDAO.SaveProduct(newProduct);

                if (newProduct.ProductId != 0)
                {
                    return new APISuccessResult<long>(newProduct.ProductId);
                }
                return new APIErrorResult<long>("Can't add new product to database.");
            }            

            return new APIErrorResult<long>("Can't add new product to database.");
        }

        // Edit an existed product in the store
        public APIResult<long> EditProduct(long productId, ProductCreateDTO product, long currentUserId)
        {
            // Get current store
            Store currentStore = StoreDAO.GetStoreByUserId(currentUserId);

            Product currentProduct = ProductDAO.GetProductDetailById(productId);

            if (currentProduct != null && product != null)
            {
                if (currentProduct.StoreId != currentStore.StoreId)
                {
                    return new APIErrorResult<long>("Can't edit this product because it's in another store.");
                }
                else
                {
                    Product editProduct = new()
                    {
                        ProductId = productId,
                        CreatedAt = currentProduct.CreatedAt,
                        Description = product.Description,
                        Image = product.Image,
                        Name = product.Name,
                        Stock = product.Stock,
                        Status = 1,
                        UnitPrice = product.UnitPrice,
                        UpdatedAt = DateTime.Now,
                        CategoryId = product.CategoryId,
                        StoreId = currentProduct.StoreId
                    };

                    ProductDAO.UpdateProduct(editProduct);

                    return new APISuccessResult<long>(productId);
                }
            }

            return new APIErrorResult<long>("Can't edit this product.");
        }

        // Delete an existed product in the store by change it status
        public APIResult<bool> DeleteProduct(long productId, long currentUserId)
        {
            // Get current store
            Store currentStore = StoreDAO.GetStoreByUserId(currentUserId);

            Product currentProduct = ProductDAO.GetProductDetailById(productId);
            if (currentProduct != null)
            {
                if (currentProduct.StoreId != currentStore.StoreId)
                {
                    return new APIErrorResult<bool>("Can't delete this product because it's in another store.");
                }
                else
                {
                    Product deleteProduct = new()
                    {
                        ProductId = productId,
                        CreatedAt = currentProduct.CreatedAt,
                        Description = currentProduct.Description,
                        Image = currentProduct.Image,
                        Name = currentProduct.Name,
                        Stock = currentProduct.Stock,
                        Status = 0,
                        UnitPrice = currentProduct.UnitPrice,
                        UpdatedAt = DateTime.Now,
                        CategoryId = currentProduct.CategoryId,
                        StoreId = currentProduct.StoreId
                    };

                    ProductDAO.UpdateProduct(deleteProduct);

                    return new APISuccessResult<bool>();
                }
            }

            return new APIErrorResult<bool>("Can't delete this product.");
        }
    }
}
