using DAL;
using Models.HHModel;
using QdtJXCConnector.WCFListenService.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using TableDependency.EventArgs;
using TableDependency.SqlClient;
using Utils;

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class BakdlyListen : IBaseListen<DlySale>, IDisposable
    {
        public static List<SqlTableDependency<DlySale>> dependencyList = new List<SqlTableDependency<DlySale>>();

        public BakdlyListen(List<GraspcwZT> ls)
        {
            InitListen(ls);
        }

        public void InitListen(List<GraspcwZT> ls)
        {
            try
            {
                foreach (var zt in ls)
                {
                    string con = DataBaseUtility.GetConnectionStr(zt.DbName);
                    var sqlTableDependency = new SqlTableDependency<DlySale>(con, "bakdly");
                    sqlTableDependency.OnChanged += TableDependency_Changed;
                    sqlTableDependency.OnError += (sender, e)=> {
                        BaseSettingService.GetInstance().Restart();
                        LogUtils.Error("BakdlyListen:ErrorDependency  DataSource:{0}   Error:{1}   Message:{2}", e.Database, e.Error, e.Message);
                    };
                    sqlTableDependency.Start();
                    dependencyList.Add(sqlTableDependency);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("InitListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<DlySale> args)
        {
            try
            {
                Insert(args.Entity, args.Database);
            }
            catch (Exception e)
            {
                LogUtils.Error("LgoinUserListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(DlySale entity, string dbName)
        {
            Dlyndx dlyndx = DlyndxDao.GetDlyndxByVchcode(entity.VChcode, dbName);
            dlyndx.DetailList = DlySaleDao.GetDlySaleByVchcode(dlyndx.VChcode, dbName);
            dlyndx.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            dlyndx.DetailList = BakdlyDao.GetBakDlyByVchcode(dlyndx.VChcode, dbName);
            BaseInput input = new BaseInput(dlyndx);
            HttpClientUtility.Post("MiddleWareService/InsertSalesOrder", input);
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
