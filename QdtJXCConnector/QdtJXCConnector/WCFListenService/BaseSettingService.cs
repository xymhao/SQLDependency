using DAL;
using Models.HHModel;
using QdtJXCConnector.WCFListenService.Listening;
using Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace QdtJXCConnector.WCFListenService
{
    public class BaseSettingService : IDisposable
    {
        private static BaseSettingService listen = new BaseSettingService();
        public static string _dataSource = ConfigurationManager.AppSettings["DataSource"].ToString();
        public static string _adminID = ConfigurationManager.AppSettings["AdminID"].ToString();
        public static string _password = ConfigurationManager.AppSettings["Password"].ToString();

        public static LoginUserListen loginuser;
        public static EmployeeListen employee;
        public static BTypeListen bType;
        public static DlyndxListen dlyndx;
        public static DlySaleListen dlySale;
        public static StockListen stock;
        public static P_PriceTypeListen priceType;
        public static PTypeListen ptype;
        public static PTypePriceListen price;
        public static PTypeKPriceListen kPrice;
        public static PTypeUnitListen unit;
        public static PTypeBarCodeListen barCode;
        public static PriceRightListen priceRight;
        public static PRightListen pRight;
        public static KRightListen kRight;
        public static BakdlyListen bakdly;
        public static SysConListen sysCon;
        public static GraspcwZtListen graspceZt;

        public BaseSettingService()
        {
        }

        public static BaseSettingService GetInstance()
        {
            return listen;
        }

        public void Start()
        {
            List<GraspcwZT> ls = GraspcwZTDao.Get();

            foreach (var zt in ls)
            {
                ServiceBrokerDao.StartServiceBroker(zt.DbName, _adminID);
                //删除废弃的存储过程
                string triggerSQL = string.Format(@" SELECT Name 
                                                       FROM [sysobjects] 
                                                      WHERE xtype='TR'; ");
                var triggerList = DataBaseUtility.Query<string>(triggerSQL, zt.DbName);

                string serviceSQL = string.Format(@" SELECT name FROM [sys].services WHERE name LIKE '%dbo_%';");
                var serviceList = DataBaseUtility.Query<string>(serviceSQL, zt.DbName);

                string queueSQL = string.Format(@" SELECT name FROM [sys].service_queues WHERE name LIKE '%dbo_%';");
                var queueList = DataBaseUtility.Query<string>(queueSQL, zt.DbName);

                string contractSQL = string.Format(@" SELECT name FROM [sys].service_contracts WHERE name LIKE '%dbo_%';");
                var contractList = DataBaseUtility.Query<string>(contractSQL, zt.DbName);

                string msgTypeSQL = string.Format(@" SELECT name FROM [sys].service_message_types WHERE name LIKE '%dbo_%';");
                var msgTypeList = DataBaseUtility.Query<string>(msgTypeSQL, zt.DbName);

                Task task = new Task(() =>
                {
                    ServiceBrokerDao.DeleteTrigger(zt.DbName, triggerList);
                    ServiceBrokerDao.DeleteService(zt.DbName, serviceList);
                    ServiceBrokerDao.DeleteQueue(zt.DbName, queueList);
                    ServiceBrokerDao.DeleteContract(zt.DbName, contractList);
                    ServiceBrokerDao.DeleteMessageType(zt.DbName, msgTypeList);
                });
                task.Start();
            }

            loginuser = new LoginUserListen(ls);
            employee = new EmployeeListen(ls);
            bType = new BTypeListen(ls);
            stock = new StockListen(ls);
            priceType = new P_PriceTypeListen(ls);
            ptype = new PTypeListen(ls);
            price = new PTypePriceListen(ls);
            kPrice = new PTypeKPriceListen(ls);
            unit = new PTypeUnitListen(ls);
            barCode = new PTypeBarCodeListen(ls);
            priceRight = new PriceRightListen(ls);
            sysCon = new SysConListen(ls);
            kRight = new KRightListen(ls);
            pRight = new PRightListen(ls);
            //bakdly = new BakdlyListen(ls); 
            //graspceZt = new GraspcwZtListen();
            //dlyndx = new DlyndxListen(ls);
            //dlySale = new DlySaleListen(ls);
        }

        public void Restart()
        {
            listen.Dispose();
            List<GraspcwZT> ls = GraspcwZTDao.Get();
            loginuser = new LoginUserListen(ls);
            employee = new EmployeeListen(ls);
            bType = new BTypeListen(ls);
            stock = new StockListen(ls);
            priceType = new P_PriceTypeListen(ls);
            ptype = new PTypeListen(ls);
            price = new PTypePriceListen(ls);
            kPrice = new PTypeKPriceListen(ls);
            unit = new PTypeUnitListen(ls);
            barCode = new PTypeBarCodeListen(ls);
            priceRight = new PriceRightListen(ls);
            sysCon = new SysConListen(ls);
            kRight = new KRightListen(ls);
            pRight = new PRightListen(ls);
            kRight = new KRightListen(ls);
            //bakdly = new BakdlyListen(ls); 
            //dlyndx = new DlyndxListen(ls);
            //dlySale = new DlySaleListen(ls);
        }

        public void Dispose()
        {
            loginuser?.Dispose();
            employee?.Dispose();
            bType?.Dispose();
            stock?.Dispose();
            priceType?.Dispose();
            ptype?.Dispose();
            price?.Dispose();
            kPrice?.Dispose();
            unit?.Dispose();
            barCode?.Dispose();
            priceRight?.Dispose();
            sysCon?.Dispose();
            pRight?.Dispose();
            kRight?.Dispose();
            //dlySale.Dispose();
            //bakdly.Dispose();    
            //dlyndx.Dispose();
        }
    }
}
