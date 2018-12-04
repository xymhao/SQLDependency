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

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PriceRightListen : IBaseListen<T_Right>, IDisposable
    {
        public static List<SqlTableDependency<T_Right>> dependencyList = new List<SqlTableDependency<T_Right>>();

        public PriceRightListen(List<GraspcwZT> ls)
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
                    var sqlTableDependency = new SqlTableDependency<T_Right>(con, "T_PriceRight");
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

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<T_Right> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper() == "INSERT" || args.ChangeType.ToString().ToUpper() == "UPDATE")
                {
                    Insert(args.Entity, args.Database);
                }
                else
                {
                    Delete(args.Entity, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("TableDependency_Changed:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(T_Right entity, string dbName)
        {
            entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            entity.RightType = 3;
            entity.DbName = dbName;
            BaseInput input = new BaseInput(entity);
            HttpClientUtility.PostResult("MiddleWareService/RightInsert", input);
        }


        public void Delete(T_Right entity, string dbName)
        {
            entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            entity.RightType = 3;
            entity.DbName = dbName;
            BaseInput input = new BaseInput(entity);
            HttpClientUtility.PostResult("MiddleWareService/DeleteRight", input);
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
