using BusinessObject.Common;
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

        // Update store information
        public APIResult<bool> UpdateStore(long currentStoreStaffId, StoreInformationUpdateDTO info)
        {
            if (info == null) return new APIErrorResult<bool>("The new information cannot be empty.");

            var store = StoreDAO.GetStoreByUserId(currentStoreStaffId);

            if (store == null) return new APIErrorResult<bool>("This store is not existed.");
            else
            {
                store.Address = info.Address;
                store.CoverImage = info.CoverImage;
                store.Description = info.Description;
                store.LogoImage = info.LogoImage;
                store.Name = info.Name;
                store.UpdatedAt = DateTime.Now;

                StoreDAO.UpdateStore(store);
                return new APISuccessResult<bool>();
            }
        }
    }
}
