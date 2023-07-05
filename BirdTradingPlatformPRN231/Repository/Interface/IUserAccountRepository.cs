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
        ClientUserViewListDTO GetAllUsers(int page, string? roleSearch);
        APIResult<string> AuthenticateCustomer(LoginDTO request);
        APIResult<string> AuthenticateStore(LoginDTO request);
        APIResult<string> AuthenticateAdmin(LoginDTO request);
        APIResult<bool> RegisterCustomer(RegisterCustomerDTO request);
        APIResult<bool> RegisterStore(RegisterStoreDTO request);
        public APIResult<string> ChangeStatus(long userId);
        APIResult<UserProfileViewDTO> GetCurrentCustomer(long currentUserId);
        APIResult<UserDetailViewDTO> GetUserDetail(long userId);
        APIResult<bool> UpdateProfile(long currentUserId, UserProfileUpdateDTO profile);
        APIResult<bool> ChangePassword(long currentUserId, UserPasswordUpdateDTO password);
    }
}
