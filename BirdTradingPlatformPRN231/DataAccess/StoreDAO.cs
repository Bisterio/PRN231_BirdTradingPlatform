using BusinessObjects.Models;
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
    }
}
