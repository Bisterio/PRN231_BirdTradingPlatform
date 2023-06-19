using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Common
{
    public class APISuccessResult<T> : APIResult<T>
    {
        public APISuccessResult(T resultObj)
        {
            IsSuccess = true;
            ResultObj = resultObj;
        }

        public APISuccessResult()
        {
            IsSuccess = true;
        }
    }
}
