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
    public class PTypeKPriceListen : IBaseListen<PTypeKPrice>, IDisposable
    {
        public static List<SqlTableDependency<PTypeKPrice>> dependencyList = new List<SqlTableDependency<PTypeKPrice>>();

        public PTypeKPriceListen(List<GraspcwZT> ls)
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
                    var sqlTableDependency = new SqlTableDependency<PTypeKPrice>(con, "XW_P_PTypeKPrice");
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

        public void Insert(PTypeKPrice kPrice, string dbName)
        {
            PType entity = PTypeDao.GetPTypeByID(kPrice.PTypeID, dbName);
            entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            //entity.PTypePriceList = PTypePriceDal.GetPTypePriceByID(entity.PTypeID, dbName);
            //entity.PTypeKPriceList = PTypeKPriceDal.GetPTypeKPriceByID(entity.PTypeID, dbName);
            //entity.PTypeUnitList = PTypeUnitDal.GetPTypeUnitByID(entity.PTypeID, dbName);
            entity.DbName = dbName;
            BaseInput input = new BaseInput(entity);
            HttpClientUtility.Post("MiddleWareService/PTypeInsert", input);
        }

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<PTypeKPrice> args)
        {
            try
            {
                Insert(args.Entity, args.Database);
            }
            catch (Exception e)
            {
                LogUtils.Error("TableDependency_Changed:", e.Message);
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
