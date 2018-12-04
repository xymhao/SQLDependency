using Filter;
using Models.PostInputValues;
using DAL;
using System.Collections.Generic;
using System.Web.Http;

namespace QdtJXCConnector.WebApiController
{
    public class DlyndxController : ApiController
    {
        [TokenFilter]
        public IEnumerable<Dictionary<string, object>> Post([FromBody]DlyndxIn dly)
        {
            var dict = DlyndxDao.PutDlyndx(dly.DbName, dly.Dlyndx);
            IEnumerable<Dictionary<string, object>> list = new List<Dictionary<string, object>>() { dict };
            return list;
        }
    }
}
 