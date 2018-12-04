using DAL;
using Models.HHModel;
using Models.PostInputValues;
using QdtJXCConnector.WCFListenService.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using TableDependency;
using TableDependency.EventArgs;
using TableDependency.SqlClient;
using Utils;

namespace QdtJXCConnector.WCFListenService.Listening
{
    public class BTypeListen : IBaseListen<BType>, IDisposable
    {
        public static List<SqlTableDependency<BType>> dependencyList = new List<SqlTableDependency<BType>>();

        public BTypeListen(List<GraspcwZT> ls)
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
                    var mapper = new ModelToTableMapper<BType>();
                    var sqlTableDependency = new SqlTableDependency<BType>(con, "BType");
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

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<BType> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper() == "INSERT")
                {
                    Insert(args.Entity, args.Database);
                }
                else if (args.ChangeType.ToString().ToUpper().Equals("UPDATE"))
                {
                    Update(args.Entity, args.Database);
                }
                else
                {
                    Delete(args.Entity.BTypeID, args.Entity.Leveal, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("LgoinUserListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(BType entity, string dbName)
        {
            if (entity.IsStop == 1 || entity.Deleted == 1)
            {
                Delete(entity.BTypeID, entity.Leveal, dbName);
            }
            else
            {
                entity.Level = entity.Leveal;
                entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
                entity.DbName = dbName;
                BaseInput input = new BaseInput(entity);
                HttpClientUtility.Post("MiddleWareService/BTypeInsert", input);
            }
        }

        public void Update(BType entity, string dbName)
        {
            if (entity.IsStop == 1 || entity.Deleted == 1)
            {
                Delete(entity.BTypeID, entity.Leveal, dbName);
            }
            else
            {
                entity.Level = entity.Leveal;
                entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
                entity.DbName = dbName;
                BaseInput input = new BaseInput(entity);
                HttpClientUtility.PostResult("MiddleWareService/BTypeInsert", input);
            }
        }

        public void Delete(string ID, int level, string dbName)
        {
            BTypeDeleteIn baseInput = new BTypeDeleteIn();
            baseInput.BTypeID = ID;
            baseInput.Level = level;
            baseInput.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            baseInput.DbName = dbName;
            BaseInput input = new BaseInput(baseInput);
            HttpClientUtility.Post("MiddleWareService/DeleteBType", input);
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
