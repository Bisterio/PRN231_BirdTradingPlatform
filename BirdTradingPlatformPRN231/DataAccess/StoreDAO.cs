using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class StoreDAO
    {
        public static void CreateStore(Store s)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Stores.Add(s);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Store? GetStoreById(long storeId)
        {
            var store = new Store();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    store = context.Stores
                        .Include(s => s.User)
                        .SingleOrDefault(s => s.StoreId == storeId && s.Status == 1 && s.User.Status == 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return store;
        }

        public static List<Store> GetStoresAvailable()
        {
            var listStores = new List<Store>();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    listStores = context.Stores
                        .Where(s => s.Status == 1 && s.User.Status == 1)
                        .Include(s => s.User)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listStores;
        }

        public static Store? GetStoreByUserId(long userId)
        {
            var store = new Store();
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    var user = context.UserAccounts.SingleOrDefault(s => s.UserId == userId);
                    long storeId = (long)user.StoreId;
                    store = GetStoreById(storeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return store;
        }

        public static void UpdateStore(Store store)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.Entry<Store>(store).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
