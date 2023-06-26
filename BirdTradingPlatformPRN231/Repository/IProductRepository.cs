using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IProductRepository
    {
        public ClientProductViewListDTO GetProductsPublic(int page, string? nameSearch, long categoryId, long priceMin, long priceMax, int orderBy);
        public ProductViewDTO? GetProductDetailPublicById(long productId);
        public List<ProductViewDTO> GetProductsPublicByStoreId(long storeId);
    }
}
