﻿using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IProductRepository
    {
        public ClientProductViewListDTO GetProductsPublic(int page, string? nameSearch, long categoryId, long priceMin, long priceMax, int orderBy);
        public ProductViewDTO? GetProductDetailPublicById(long productId);
        public List<ProductViewDTO> GetProductsPublicByStoreId(long storeId);
        public ClientProductViewListDTO GetProductsStore(int page, string? nameSearch, long categoryId, long priceMin, long priceMax, int orderBy, long currentUserId);
        public ProductViewDTO GetProductDetailStore(long id, long currentUserId);
    }
}
