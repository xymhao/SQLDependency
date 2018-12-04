using System;
using System.Collections.Generic;
using System.ServiceModel;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using System.Configuration;
using Utils;
using Models.HHModel;
using DAL;

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class GraspcwZtListen : IDisposable
    {
        public static List<SqlTableDependency<GraspcwZT>> dependencyList = new List<SqlTableDependency<GraspcwZT>>();

        public GraspcwZtListen()
        {
            InitListen();
        }

        public void InitListen()
        {
            string con = DataBaseUtility.GetConnectionStr("GraspcwZt");
            var sqlTableDependency = new SqlTableDependency<GraspcwZT>(con, "GraspcwZt");
            sqlTableDependency.OnChanged += TableDependency_Changed;
            sqlTableDependency.OnError += (sender, e) => {
                BaseSettingService.GetInstance().Restart();
                LogUtils.Error("BakdlyListen:ErrorDependency  DataSource:{0}   Error:{1}   Message:{2}", e.Database, e.Error, e.Message);
            };
            sqlTableDependency.Start();
            dependencyList.Add(sqlTableDependency);
        }

        public void Insert(GraspcwZT entity, string dbName)
        {
            throw new NotImplementedException();
        }

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<GraspcwZT> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper() == "INSERT")
                {
                    BaseSettingService.GetInstance().Restart();
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("LgoinUserListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Dispose()
        {
            foreach (var de in dependencyList)
            {
                try
                {
                    de.Stop();
                }
                catch (Exception e)
                {
                    LogUtils.Error("Dispose:", e.Message);
                    LogUtils.Error(e.StackTrace);
                }
            }
        }
    }
}
