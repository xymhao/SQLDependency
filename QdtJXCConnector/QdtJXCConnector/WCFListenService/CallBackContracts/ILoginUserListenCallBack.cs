using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace QdtJXCConnector.WCFListenService.CallBackContracts
{
    public interface ILoginUserListenCallBack
    {
        [OperationContract]
        void LoginUserChange(string ID);
    }
}
