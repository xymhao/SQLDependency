using QdtJXCConnector.WCFListenService.Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using System.Configuration;
using Utils;
using Models.HHModel;
using DAL;

namespace QdtJXCConnector.WCFListenService.Listening
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class LoginUserListen : IBaseListen<LoginUser>, IDisposable
    {
        public static List<SqlTableDependency<LoginUser>> dependencyList = new List<SqlTableDependency<LoginUser>>();

        public LoginUserListen(List<GraspcwZT> ls)
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
                    var sqlTableDependency = new SqlTableDependency<LoginUser>(con, "LoginUser");
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
            try
            {
                if (args.ChangeType.ToString().ToUpper() == "INSERT" || args.ChangeType.ToString().ToUpper() == "UPDATE")
                {
                    Insert(args.Entity, args.Database);
                }
                else
                {
                    Delete(args.Entity.ETypeID, args.Database);
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("TableDependency_Changed:", e.Message);
                LogUtils.Error(e.StackTrace);
            }            
        }

        //同步方法
        public void Insert(LoginUser entity, string dbName)
        {
            string sql = @" SELECT etypeid
                                  ,efullName
                                  ,eusercode  
                                  ,isStop
                              FROM Employee 
                             WHERE etypeID = @EtypeID; ";
            entity = DataBaseUtility.QueryFirst<LoginUser>(sql, dbName, new { EtypeID = entity.ETypeID });
            entity.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            entity.DbName = dbName;
            BaseInput input = new BaseInput(entity);
            HttpClientUtility.Post("MiddleWareService/LoginUserInsert", input);
        }

        //同步方法
        public void Delete(string ID, string dbName)
        {
            BaseID<string> baseInput = new BaseID<string>();
            baseInput.ID = ID;
            baseInput.DogNumber = GraspcwZTDao.GetDogNumberByTableName(dbName);
            baseInput.DbName = dbName;
            BaseInput input = new BaseInput(baseInput);
            HttpClientUtility.PostResult("MiddleWareService/DeleteUser", input);
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
