using Filter;
using Models.HHModel;
using Models.Input;
using DAL;
using System.Collections.Generic;
using System.Web.Http;

namespace QdtJXCConnector.WebApiController
{
    public class PTypeController : ApiController
    {
        [TokenFilter]
        public List<PType> Post([FromBody]PTypeIn input)
        {
            var ls =  PTypeDao.GetPTypeByIDs(input.PTypeIDs, input.DbName);
            PType pType = new PType();
            foreach (var p in ls)
            {
                p.DogNumber = GraspcwZTDao.GetDogNumberByTableName(input.DbName);
                p.PTypePriceList = PTypePriceDao.GetPTypePriceByID(p.PTypeID, input.DbName);
                p.PTypeKPriceList = PTypeKPriceDao.GetPTypeKPriceByID(p.PTypeID, input.DbName);
                p.PTypeUnitList = PTypeUnitDao.GetPTypeUnitByID(p.PTypeID, input.DbName);
                p.DbName = input.DbName;
            }
            return ls;
        }
    }
}
