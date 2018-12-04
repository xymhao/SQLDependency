using DAL;
using Models.HHModel;
using QdtJXCConnector.WCFListenService.Contracts;
using Utils;
using System;
using System.Collections.Generic;
using TableDependency;
using TableDependency.EventArgs;
using TableDependency.SqlClient;

namespace QdtJXCConnector.WCFListenService.Listening
{
    /// <summary>
    /// 监听人员名称变化
    /// </summary>
    public class EmployeeListen : IBaseListen<LoginUser>, IDisposable
    {
        public static List<SqlTableDependency<LoginUser>> dependencyList = new List<SqlTableDependency<LoginUser>>();

        public EmployeeListen(List<GraspcwZT> ls)
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
                    var mapper = new ModelToTableMapper<LoginUser>();
                    mapper.AddMapping(c => c.EFullName, "EFullName");
                    mapper.AddMapping(c => c.EUserCode, "EUserCode");
                    mapper.AddMapping(c => c.IsStop, "IsStop");

                    var sqlTableDependency = new SqlTableDependency<LoginUser>(con, "Employee");
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

        public void TableDependency_Changed(object sender, RecordChangedEventArgs<LoginUser> args)
        {
            if (args.ChangeType.ToString().ToUpper() == "UPDATE")
            {
                Insert(args.Entity, args.Database);
            }
        }

        public void Insert(LoginUser user, string dbName)
        {
            string sql = @" SELECT 1 FROM LoginUser WHERE etypeID = @EtypeID; ";
            int isLoginUser = DataBaseUtility.QueryFirst<int>(sql, dbName, new { EtypeID = user.ETypeID });
            if (isLoginUser == 1)
            {
                user.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
                user.DbName = dbName;
                BaseInput input = new BaseInput(user);
                HttpClientUtility.Post("MiddleWareService/LoginUserInsert", input);
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
