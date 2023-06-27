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
        APIResult<bool> Register(RegisterDTO request);
    }
}
