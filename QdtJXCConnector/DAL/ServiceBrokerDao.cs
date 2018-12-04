using Models.HHModel;
using Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    public static class ServiceBrokerDao
    {
        public static readonly List<string> triggerNames = new List<string>{ "TR_DBO_LOGINUSER", "TR_DBO_EMPLOYEE", "TR_DBO_BTYPE", "TR_DBO_STOCK", "TR_DBO_XW_P_PRICETYPE", "TR_DBO_PTYPE", "TR_DBO_XW_P_PTYPEPRICE", "TR_DBO_XW_P_PTYPEKPRICE", "TR_DBO_XW_PTYPEUNIT", "TR_DBO_XW_PTYPEBARCODE", "TR_DBO_T_PRICERIGHT", "TR_DBO_SYSCON", "TR_DBO_T_KRIGHT", "TR_DBO_T_PRIGHT" };
        public static void DeleteServiceBroker(string dBName)
        {
            try
            {
                string SQL = string.Format(@" DECLARE @userId varchar(100)
                                          DECLARE @count int
                                          DECLARE @sql varchar(100)
                                          SET @count = 0
                                          
	                                         SELECT @count = count(*) FROM [sys].services WHERE name LIKE '%dbo_%';
	                                         SELECT @count;
                                             SELECT top 1 @userId = name FROM [sys].services WHERE name LIKE '%dbo_%';
                                             SELECT @userId;
	                                         WHILE @count > 0
                                          BEGIN
                                              SELECT top 1 @userId = name FROM [sys].services WHERE name LIKE '%dbo_%';
                                              SET @sql = 'DROP SERVICE [' + @userId + ']';
                                              SELECT @userId;
                                              exec(@sql);
                                              SET @count = @count -1; 
                                          END                                           
                                          
                                          SELECT @count = count(*) FROM [sys].service_queues WHERE name LIKE '%dbo_%';
                                          WHILE @count > 0
                                          BEGIN
                                              SELECT top 1 @userId = name FROM [sys].service_queues WHERE name LIKE '%dbo_%';
                                              SET @sql = 'DROP QUEUE [' + @userId + ']';
                                              SELECT @userId;
                                              exec(@sql);
                                              SET @count = @count -1; 
                                          END   
                                          
                                          SELECT @count = count(*) FROM [sys].service_contracts WHERE name LIKE '%dbo_%';
                                          WHILE @count > 0
                                          BEGIN
                                              SELECT top 1 @userId = name FROM [sys].service_contracts WHERE name LIKE '%dbo_%';
                                              SELECT @userId;
                                              SET @sql = 'DROP CONTRACT [' + @userId + ']';
                                              exec(@sql);
                                              SET @count = @count -1; 
                                          END   
                                          
                                          SELECT @count = count(*) FROM [sys].service_message_types WHERE name LIKE '%dbo_%';
                                          WHILE @count > 0
                                          BEGIN
                                              SELECT top 1 @userId = name FROM [sys].service_message_types WHERE name LIKE '%dbo_%';
                                              SET @sql = 'DROP MESSAGE TYPE [' + @userId + ']';
                                              SELECT @userId;
                                              exec(@sql);
                                              SET @count = @count -1; 
                                          END    ");
                DataBaseUtility.Execute(SQL, dBName);
            }
            catch(Exception e)
            {
                LogUtils.Error("DeleteServiceBroker:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public static void StartServiceBroker(string dBName, string admin)
        {
            try
            {
                string brokerSQL = string.Format(@" SELECT name AS DbName
                                                      ,is_broker_enabled AS IsBroker
                                                  FROM sys.databases ");
                var brokerList = DataBaseUtility.Query<SystemBroker>(brokerSQL, "master");

                var broker = brokerList.Where(p => p.DbName.Equals(dBName)).Last();
                if (broker.IsBroker.Equals(0))
                {
                    string startSQL = string.Format(@" ALTER DATABASE {0} SET NEW_BROKER WITH ROLLBACK IMMEDIATE;
                                                       ALTER DATABASE {0} SET ENABLE_BROKER;", dBName);
                    DataBaseUtility.Execute(startSQL, dBName);
                }
                string sql = string.Format(@"ALTER AUTHORIZATION ON DATABASE::[{0}] TO [{1}];                                              
                                               SET ANSI_NULLS ON;
                                               SET ANSI_PADDING ON;
                                               SET ANSI_WARNINGS ON;
                                               SET CONCAT_NULL_YIELDS_NULL ON;
                                               SET QUOTED_IDENTIFIER ON;
                                               SET NUMERIC_ROUNDABORT OFF;
                                               SET ARITHABORT ON;", dBName, admin);
                DataBaseUtility.Execute(sql, dBName);
            }
            catch (Exception e)
            {
                LogUtils.Error("StartServiceBroker:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public static void DeleteTrigger(string dBName, List<string> triggerList)
        {
            try
            {
                StringBuilder deleteSQL = new StringBuilder();
                foreach (var name in triggerList)
                {
                    if (name.Length > 37)
                    {
                        var length = name.Length - 37;
                        var tableName = name.Substring(0, length);
                        if (triggerNames.Contains(tableName.ToUpper()))
                        {
                            string dropSQL = string.Format(@" DROP TRIGGER [dbo].[{0}]; ", name);
                            deleteSQL.Append(dropSQL);
                        }
                    }
                }
                if (deleteSQL.ToString().Length > 0)
                    DataBaseUtility.Execute(deleteSQL.ToString(), dBName);
            }
            catch (Exception e)
            {
                LogUtils.Error("DeleteTrigger:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public static void DeleteService(string dbName, List<string> servicesList)
        {
            try
            {
                StringBuilder deleteSQL = new StringBuilder();

                foreach (var name in servicesList)
                {
                    string TSQL = string.Format(@" DROP SERVICE [{0}]; ", name);
                    deleteSQL.Append(TSQL);
                }
                if (deleteSQL.ToString().Length > 0)
                    DataBaseUtility.Execute(deleteSQL.ToString(), dbName);
            }
            catch (Exception e)
            {
                LogUtils.Error("DeleteService:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public static void DeleteQueue(string dbName, List<string> queueList)
        {
            try
            {
                StringBuilder deleteSQL = new StringBuilder();

                foreach (var name in queueList)
                {
                    string TSQL = string.Format(@" DROP QUEUE [{0}]; ", name);
                    deleteSQL.Append(TSQL);
                }
                if (deleteSQL.ToString().Length > 0)
                    DataBaseUtility.Execute(deleteSQL.ToString(), dbName);
            }
            catch (Exception e)
            {
                LogUtils.Error("DeleteQueue:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public static void DeleteContract(string dbName, List<string> contractList)
        {
            try
            {
                StringBuilder deleteSQL = new StringBuilder();

                foreach (var name in contractList)
                {
                    string TSQL = string.Format(@" DROP CONTRACT [{0}]; ", name);
                    deleteSQL.Append(TSQL);
                }
                if (deleteSQL.ToString().Length > 0)
                    DataBaseUtility.Execute(deleteSQL.ToString(), dbName);
            }
            catch (Exception e)
            {
                LogUtils.Error("DeleteContract:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }

        public static void DeleteMessageType(string dbName, List<string> msgTypeList)
        {
            try
            {
                StringBuilder deleteSQL = new StringBuilder();

                foreach (var name in msgTypeList)
                {
                    string TSQL = string.Format(@" DROP MESSAGE TYPE [{0}]; ", name);
                    deleteSQL.Append(TSQL);
                }
                if (deleteSQL.ToString().Length > 0)
                    DataBaseUtility.Execute(deleteSQL.ToString(), dbName);
            }
            catch (Exception e)
            {
                LogUtils.Error("DeleteMessageType:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }
    }
}
