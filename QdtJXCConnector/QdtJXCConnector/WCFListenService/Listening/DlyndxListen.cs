using DAL;
using Models.HHModel;
using Models.PostInputValues;
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
    public class DlyndxListen : IBaseListen<Dlyndx>, IDisposable
    {
        public static List<SqlTableDependency<Dlyndx>> dependencyList = new List<SqlTableDependency<Dlyndx>>();

        public DlyndxListen(List<GraspcwZT> ls)
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
                    var sqlTableDependency = new SqlTableDependency<Dlyndx>(con, "Dlyndx");
                    sqlTableDependency.OnChanged += TableDependency_Changed;
                    sqlTableDependency.OnError += (sender, e) => {
                        BaseSettingService.GetInstance().Restart();
                        LogUtils.Error("ErrorDependency  DataSource:{0}   Error:{1}   Message:{2}", e.Database, e.Error, e.Message);
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

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<Dlyndx> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper().Equals("DELETE"))
                {
                    Delete(args.Entity, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("LgoinUserListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(Dlyndx entity, string dbName)
        {
            entity.DetailList = DlySaleDao.GetDlySaleByVchcode(entity.VChcode,dbName);
            entity.DetailList = BakdlyDao.GetBakDlyByVchcode(entity.VChcode, dbName);
            entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);          
            BaseInput input = new BaseInput(entity);
            HttpClientUtility.Post("MiddleWareService/InsertSalesOrder", input);
        }

        //同步方法
        public void Delete(Dlyndx entity, string dbName)
        {
            DlyndxDeleteIn del = new DlyndxDeleteIn();
            del.BTypeID = entity.BTypeID;
            del.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            del.ETypeID = entity.ETypeID;
            del.InputNo = entity.InputNo;
            del.KTypeID = entity.KTypeID;
            del.OrderDate = entity.Date;
            BaseInput input = new BaseInput(del);
            HttpClientUtility.Post("MiddleWareService/DeleteSalesOrder", input);
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
