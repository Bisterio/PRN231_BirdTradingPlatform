using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Common
{
    public class APIResult<T>
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public T? ResultObj { get; set; }
    }
}
