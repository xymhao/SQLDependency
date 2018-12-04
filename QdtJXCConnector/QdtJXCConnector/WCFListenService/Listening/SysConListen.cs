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
using Models.PostInputValues;
using DAL;
using Utils;

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class SysConListen : IBaseListen<SysCon>, IDisposable
    {
        public static List<SqlTableDependency<SysCon>> dependencyList = new List<SqlTableDependency<SysCon>>();

        public SysConListen(List<GraspcwZT> ls)
        {
            InitListen(ls);
        }

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<SysCon> args)
        {
            try
            {
                if (args.ChangeType.ToString().ToUpper() == "UPDATE")
                {
                    Insert(args.Entity, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("TableDependency_Changed:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public void Insert(SysCon entity, string dbName)
        {
            SysConIn sys = new SysConIn(entity);
            sys.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            sys.DbName = dbName;
            BaseInput input = new BaseInput(sys);
            HttpClientUtility.Post("MiddleWareService/SysConUpdate", input);
        }

        public void InitListen(List<GraspcwZT> ls)
        {
            try
            {
                foreach (var zt in ls)
                {
                    string con = DataBaseUtility.GetConnectionStr(zt.DbName);
                    Expression<Func<SysCon, bool>> expression = p => (p.Order == 15 || p.Order == 92);
                    ITableDependencyFilter whereCondition = new SqlTableDependencyFilter<SysCon>(expression);
                    var sqlTableDependency = new SqlTableDependency<SysCon>(con, "SysCon", filter: whereCondition);
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
