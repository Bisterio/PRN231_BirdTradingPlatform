using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UserAccountDAO
    {
        public static UserAccount? FindUserByEmail(String email)
        {
            UserAccount? user;
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    user = context.UserAccounts
                        .Include(x => x.Store)
                        .SingleOrDefault(x => x.Email == email);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }

        public static void CreateUser(UserAccount u)
        {
            try
            {
                using (var context = new BirdTradingPlatformContext())
                {
                    context.UserAccounts.Add(u);
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
