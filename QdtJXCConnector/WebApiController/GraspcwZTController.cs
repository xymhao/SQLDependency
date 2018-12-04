using Filter;
using Models.HHModel;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace QdtJXCConnector.WebApiController
{
    public class GraspcwZTController : ApiController
    {
        // GET api/values/5 
        [TokenFilter]
        public IEnumerable<GraspcwZT> Post([FromBody]string dogNumber)
        {
            var ls = GraspcwZTDao.GetByDogNumber(dogNumber);
            return ls;
        }
    }
}
