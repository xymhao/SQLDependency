using Models.HHModel;
using QdtJXCConnector.WCFListenService.CallBackContracts;
using System.Collections.Generic;
using System.ServiceModel;

namespace QdtJXCConnector.WCFListenService.Contracts
{
    [ServiceContract(CallbackContract = typeof(ILoginUserListenCallBack))]
    interface ILoginUserListen 
    {
        [OperationContract]
        IList<LoginUser> GetAll();

        [OperationContract]
        void LoginUserChange(string item, string name, decimal price);
    }
}
