using BusinessObject.Common;
using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserAccountRepository
    {
        APIResult<string> AuthenticateCustomer(LoginDTO request);
        APIResult<string> AuthenticateStore(LoginDTO request);
        APIResult<bool> RegisterCustomer(RegisterCustomerDTO request);
        APIResult<bool> RegisterStore(RegisterStoreDTO request);
        APIResult<UserProfileViewDTO> GetCurrentCustomer(long currentUserId);
        APIResult<bool> UpdateProfile(long currentUserId, UserProfileUpdateDTO profile);
        APIResult<bool> ChangePassword(long currentUserId, UserPasswordUpdateDTO password);
    }
}
