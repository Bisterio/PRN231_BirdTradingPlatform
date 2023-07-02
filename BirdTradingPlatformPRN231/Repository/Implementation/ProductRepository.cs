using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly HttpClient client = null;
        private string GoogleMapApi = "";

        public ProductRepository()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            GoogleMapApi = "https://maps.googleapis.com/maps/api/distancematrix/json";
        }

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

            if (product != null && currentStore.StoreId != null)
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

        public async Task<APIResult<CheckoutViewDTO>> CheckShippingCost(CartAddressDTO request)
        {
            CheckoutViewDTO resultDTO = new CheckoutViewDTO();
            List<ShippingCalculatedCartItemDTO> cart = new List<ShippingCalculatedCartItemDTO>();

            // Check for valid product (product status == 1, quantity does not exceed stock)
            foreach (OrderItemCartDTO cartItem in request.CartItems)
            {
                // Get each product and check for status and stock
                Product? p = ProductDAO.GetProductDetailById(cartItem.ProductId);
                if (p == null || p.Status == 0)
                    return new APIErrorResult<CheckoutViewDTO>("Your cart contain one or more item that is unavailable. Please update your cart!");
                if (cartItem.Quantity <= 0)
                    return new APIErrorResult<CheckoutViewDTO>($"Quantity for product '{p.Name}' cannot be lower than 0. Please update the quantity of this product!");
                if (cartItem.Quantity > p.Stock)
                    return new APIErrorResult<CheckoutViewDTO>($"Quantity for product '{p.Name}' has exceeded the current units in stock for this product. Please update the quantity of this product!");
            }

            // Separate cart items by storeid
            var cartItemsGroupedByStore =
                request.CartItems.GroupBy(c => c.StoreId)
                .Select(g => new
                {
                    StoreId = g.Key,
                    CartItems = g.Select(cartItem => new
                    {
                        cartItem.ProductId,
                        cartItem.Quantity
                    })
                });

            foreach (var storeOrder in cartItemsGroupedByStore.OrderBy(g => g.StoreId))
            {
                Store currentStore = StoreDAO.GetStoreById(storeOrder.StoreId);

                // GET distance
                HttpResponseMessage response = await client.GetAsync(GoogleMapApi + $"?origins={request.ShippingAddress}&destinations={currentStore.Address}&key=AIzaSyDNHSAw4yrVYpCRmZQg43S50ffsoZwqqD8");
                if (!response.IsSuccessStatusCode) return new APIErrorResult<CheckoutViewDTO>("Cannot get shipping cost from store to user");

                string strData = await response.Content.ReadAsStringAsync();
                DistanceMatrixResult? data = JsonSerializer.Deserialize<DistanceMatrixResult>(strData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                long distance = (data.Rows[0].Elements[0].Distance.Value) / 1000;
                decimal shippingCost = 0;
                if (distance <= 2)
                {
                    shippingCost = 10000;
                }
                else
                {
                    shippingCost = 10000 + (distance - 2) * 3000;
                }

                // Add shipping cost
                foreach (var cartItem in storeOrder.CartItems)
                {
                    cart.Add(new ShippingCalculatedCartItemDTO()
                    {
                        Product = Mapper.ToProductViewDTO(ProductDAO.GetProductDetailById(cartItem.ProductId)),
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        ShippingCost = shippingCost,
                        StoreId = storeOrder.StoreId,
                    });
                }
            }
            resultDTO.ShippingAddress = request.ShippingAddress;
            resultDTO.CartItems = cart;
            return new APISuccessResult<CheckoutViewDTO>(resultDTO);
        }
    }
}
