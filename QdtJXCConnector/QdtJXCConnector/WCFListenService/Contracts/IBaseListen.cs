using Models.HHModel;
using System.Collections.Generic;
using TableDependency.EventArgs;

namespace QdtJXCConnector.WCFListenService.Contracts
{
    /// <summary>
    /// SQLDependency基本方法
    /// </summary>
    interface IBaseListen<T> where T : class
    {
        void TableDependency_Changed(object sender, RecordChangedEventArgs<T> args);
        void Insert(T entity, string dbName);
        void InitListen(List<GraspcwZT> ls);
    }
}
