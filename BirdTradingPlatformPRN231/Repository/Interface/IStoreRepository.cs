using BusinessObject.Common;
using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IStoreRepository
    {
        public ClientStoreDetailViewDTO GetPublicStoreDetailById(long storeId);
        public List<ClientStoreDetailViewDTO> GetStoresPublic();
        APIResult<bool> UpdateStore(long currentStoreStaffId, StoreInformationUpdateDTO info);
    }
}
