using Filter;
using Models.HHModel;
using Models.Input;
using DAL;
using System.Collections.Generic;
using System.Web.Http;

namespace QdtJXCConnector.WebApiController
{
    [TokenFilter]
    public class RightController : ApiController
    {
        public List<T_Right> Post([FromBody]RightIn input)
        {
            List<T_Right> ls = new List<T_Right>();
            if (input.RightType.Equals(1))
            {
                ls = RightDao.GetPRightsByETypeIDs(input.ETypeIDs, input.DbName);
            }
            else
            {
                ls = RightDao.GetKRightsByETypeIDs(input.ETypeIDs, input.DbName);
            }
            return ls;
        }
    }
}
