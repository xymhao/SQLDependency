using QdtJXCConnector.WCFListenService.Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using System.Configuration;
using Models.HHModel;
using DAL;
using Models.PostInputValues;
using Utils;

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PTypeListen : IBaseListen<PType>, IDisposable
    {
        public static List<SqlTableDependency<PType>> dependencyList = new List<SqlTableDependency<PType>>();

        public PTypeListen(List<GraspcwZT> ls)
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
                    var sqlTableDependency = new SqlTableDependency<PType>(con, "PType");
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

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<PType> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper() == "INSERT" || args.ChangeType.ToString().ToUpper() == "UPDATE")
                {
                    Insert(args.Entity, args.Database);
                }
                else
                {
                    Delete(args.Entity.PTypeID, args.Entity.Leveal, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("TableDependency_Changed:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(PType entity, string dbName)
        {
            if (entity.IsStop == 1 || entity.Deleted == 1)
            {
                Delete(entity.PTypeID, entity.Leveal, dbName);
            }
            else
            {
                entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
                //entity.PTypePriceList = PTypePriceDal.GetPTypePriceByID(entity.PTypeID, dbName);
                //entity.PTypeKPriceList = PTypeKPriceDal.GetPTypeKPriceByID(entity.PTypeID, dbName);
                //entity.PTypeUnitList = PTypeUnitDal.GetPTypeUnitByID(entity.PTypeID, dbName);
                entity.DbName = dbName;
                BaseInput input = new BaseInput(entity);
                HttpClientUtility.Post("MiddleWareService/PTypeInsert", input);
            }
        }

        //同步方法
        public void Delete(string pTypeID, int level, string dbName)
        {
            PTypeDeleteIn del = new PTypeDeleteIn();
            del.PTypeID = pTypeID;
            del.Level = level;
            del.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            del.DbName = dbName;
            BaseInput input = new BaseInput(del);
            HttpClientUtility.Post("MiddleWareService/DeletePType", input);
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
