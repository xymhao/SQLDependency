using Models.PostInputValues;
using Utils;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using Filter;

namespace QdtJXCConnector.WebApiController
{
    public class BaseValuesController : ApiController
    {
        // GET api/values/5 
        [TokenFilter]
        public IEnumerable<Dictionary<string, object>> Post(BaseValuesIn input)
        {
            var list = DataBaseUtility.Query<Dictionary<string, object>>(input.TSQL, input.DBName, dr =>
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (!dict.ContainsKey(dr.GetName(i)))
                    {
                        dict.Add(dr.GetName(i), dr.GetValue(i));
                    }
                }
                return dict;
            }
            );
            return list;
        }

        public string GetVersion()
        {
            return string.Format("{0}.{1}",Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor) ;
        }
    }
}
