using BusinessObject.DTOs;
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
    public class StoreRepository : IStoreRepository
    {
        // Get store detail and store's product
        public ClientStoreDetailViewDTO? GetPublicStoreDetailById(long storeId)
        {
            Store? storeEntity = StoreDAO.GetStoreById(storeId);
            if (storeEntity == null) return null;

            ClientStoreDetailViewDTO? storeModel = Mapper.ToClientStoreDetailViewDTO(storeEntity);
            List<Product> storeProducts = ProductDAO.GetProductsPublicByStoreId(storeId);
            storeModel.ProductList = ProductDAO
                .GetProductsPublicByStoreId(storeId)
                .Select(x => Mapper.ToProductViewDTO(x))
                .ToList();

            return storeModel;
        }

        // Get list of all stores
        public List<ClientStoreDetailViewDTO?> GetStoresPublic()
        {
            return StoreDAO.GetStoresAvailable()
                .Select(x => Mapper.ToClientStoreDetailViewDTO(x))
                .ToList();
        }
    }
}
