using QdtJXCConnector.WCFListenService.Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using System.Configuration;
using System.Linq.Expressions;
using TableDependency.Abstracts;
using TableDependency.SqlClient.Where;
using Models.HHModel;
using DAL;
using Utils;

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PTypeBarCodeListen : IBaseListen<PTypeBarCode>, IDisposable
    {
        public static List<SqlTableDependency<PTypeBarCode>> dependencyList = new List<SqlTableDependency<PTypeBarCode>>();

        public PTypeBarCodeListen(List<GraspcwZT> ls)
        {
            InitListen(ls);
        }

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<PTypeBarCode> args)
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

        public void Insert(PTypeBarCode entity, string dbName)
        {
            entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            entity.DbName = dbName;
            BaseInput input = new BaseInput(entity);
            HttpClientUtility.Post("MiddleWareService/PTypeBarCodeInsert", input);
        }

        public void Delete(PTypeBarCode barCode, string dbName)
        {
            barCode.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            barCode.DbName = dbName;
            BaseInput input = new BaseInput(barCode);
            HttpClientUtility.Post("MiddleWareService/DeletePTypeBarCode", input);
        }

        public void InitListen(List<GraspcwZT> ls)
        {
            try
            {
                foreach (var zt in ls)
                {
                    string con = DataBaseUtility.GetConnectionStr(zt.DbName);
                    Expression<Func<PTypeBarCode, bool>> expression = p => (p.UnitID == 0 && p.Ordid == 0);
                    ITableDependencyFilter whereCondition = new SqlTableDependencyFilter<PTypeBarCode>(expression);
                    var sqlTableDependency = new SqlTableDependency<PTypeBarCode>(con, "XW_PTypeBarCode",filter: whereCondition);
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
