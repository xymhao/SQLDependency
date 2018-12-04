using QdtJXCConnector.WCFListenService.Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using System.Configuration;
using Models.HHModel;
using DAL;
using Utils;
using Models.PostInputValues;

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class StockListen : IBaseListen<Stock>, IDisposable
    {
        public static List<SqlTableDependency<Stock>> dependencyList = new List<SqlTableDependency<Stock>>();

        public StockListen(List<GraspcwZT> ls)
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
                    var sqlTableDependency = new SqlTableDependency<Stock>(con, "Stock");
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

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<Stock> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper() == "INSERT" || args.ChangeType.ToString().ToUpper() == "UPDATE")
                {
                    Insert(args.Entity, args.Database);
                }
                else
                {
                    Delete(args.Entity.KTypeID, args.Entity.Leveal, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("LgoinUserListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(Stock entity, string dbName)
        {
            if (entity.IsStop == 1 || entity.Deleted == 1)
            {
                Delete(entity.KTypeID, entity.Leveal, dbName);
            }
            else
            {
                entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
                entity.DbName = dbName;
                BaseInput input = new BaseInput(entity);
                HttpClientUtility.Post("MiddleWareService/StockInsert", input);
            }
        }

        //同步方法
        public void Delete(string ID, int level, string dbName)
        {
            StoreDeleteIn del = new StoreDeleteIn();
            del.KTypeID = ID;
            del.Level = level;
            del.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            del.DbName = dbName;
            BaseInput input = new BaseInput(del);
            HttpClientUtility.Post("MiddleWareService/DeleteStock", input);
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
