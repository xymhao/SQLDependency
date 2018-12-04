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
    public class P_PriceTypeListen : IBaseListen<P_PriceType>, IDisposable
    {
        public static List<SqlTableDependency<P_PriceType>> dependencyList = new List<SqlTableDependency<P_PriceType>>();
        public static readonly List<string> _systemPrTypeIDs = new List<string>() { "0001", "0002", "0003", "0004" };

        public P_PriceTypeListen(List<GraspcwZT> ls)
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
                    var sqlTableDependency = new SqlTableDependency<P_PriceType>(con, "XW_P_PriceType");
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

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<P_PriceType> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper().Equals("INSERT") || args.ChangeType.ToString().ToUpper().Equals("INSERT"))
                {
                    if ((args.Entity.IsVisible.Equals(1) && args.Entity.IsSystem.Equals(0)) 
                      || _systemPrTypeIDs.Contains(args.Entity.PrTypeid))
                    {
                        Insert(args.Entity, args.Database);
                    }
                }
                else
                {
                    if ((args.Entity.IsVisible.Equals(1) && args.Entity.IsSystem.Equals(0))
                      || _systemPrTypeIDs.Contains(args.Entity.PrTypeid))
                        Delete(args.Entity.PrTypeid, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("P_PriceTypeListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(P_PriceType entity, string dbName)
        {
            entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            entity.DbName = dbName;
            BaseInput input = new BaseInput(entity);
            HttpClientUtility.Post("MiddleWareService/PriceTypeInsert", input);
        }

        //同步方法
        public void Delete(string ID, string dbName)
        {
            BaseID<string> baseInput = new BaseID<string>();
            baseInput.ID = ID;
            baseInput.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            baseInput.DbName = dbName;
            BaseInput input = new BaseInput(baseInput);
            HttpClientUtility.Post("MiddleWareService/DeletePriceType", input);
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
