using BusinessObject.Common;
using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUserAccountRepository
    {
        APIResult<String> Authenticate(LoginDTO request);
        APIResult<bool> Register(RegisterDTO request);
    }
}
