using Dapper;
using Models.HHModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utils;

namespace DAL
{
    public static class DlyndxDao
    {
        public static Dlyndx GetDlyndxByVchcode(double vChcode, string dbName)
        {
            Dlyndx dly = new Dlyndx();
            string TSQL = @" SELECT Vchcode
                                   ,Number
                                   ,Date
                                   ,BTypeID
                                   ,ETypeID
                                   ,KTypeID
                                   ,InputNo
                                   ,IfCheck
                                   ,Total
                                   ,DefDisCount  
                                   ,SaveDate
                                   ,Draft
                               FROM Dlyndx 
                              WHERE Vchcode= @Vchcode 
                                AND VchType = 11;";
            dly = DataBaseUtility.QueryFirst<Dlyndx>(TSQL, dbName, new { VChcode = vChcode });
            return dly;
        }

        public static Dictionary<string, object> PutDlyndx(string dbName, Dlyndx dly)
        {

            string TSQL = string.Empty;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["result"] = 1;
            try
            {
                string IsExistSql = string.Format(@" SELECT 1 
                                                       FROM Dlyndx
                                                      WHERE Number = '{0}'", dly.Number);
                var IsExist = DataBaseUtility.QueryFirst<int>(IsExistSql, dbName);
                if (IsExist == 1)
                {
                    return dict;
                }
                #region Z_Insertdlyndx 存储过程
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@Date", dly.Date);
                dp.Add("@number", dly.Number);
                dp.Add("@VchType", 11);
                dp.Add("@btypeid", dly.BTypeID);
                dp.Add("@etypeid", dly.ETypeID);
                dp.Add("@ktypeid", dly.KTypeID);
                dp.Add("@ifcheck", "");
                dp.Add("@inputno", dly.InputNo);
                dp.Add("@draft", dly.Draft);
                dp.Add("@nTotal", dly.Total);
                dp.Add("@DefDiscount", dly.DefDisCount);
                dp.Add("@Summary", dly.Summary);
                dp.Add("@Comment", dly.Comment);
                dp.Add("@gatherbtypeid", "");
                dp.Add("@zctypeid", "");
                dp.Add("@ktypeid2", "");
                dp.Add("@Atypeid", "");
                dp.Add("@checke", "");
                dp.Add("@Period", "");
                dp.Add("@accounte", "");
                dp.Add("@inputno1", "");
                dp.Add("@szPostNo", "");
                dp.Add("@redword", "");
                dp.Add("@redword", "");
                dp.Add("@nAttach", "");
                dp.Add("@nBillType", "");
                dp.Add("@szGatheringDate", "");
                dp.Add("@szProjectid", "");
                dp.Add("@PRTypeid", "");
                dp.Add("@VchtypeState", "");
                dp.Add("@VIPCardID", "");


                dp.Add("@net", 0, DbType.Int32, ParameterDirection.ReturnValue);
                int id = DataBaseUtility.QueryFirst<int>("Z_Insertdlyndx", dbName, dp, commandType: CommandType.StoredProcedure);
                id = dp.Get<int>("@net");
                #endregion
                var bakdlyList = dly.DetailList;
                string szRowId = string.Empty;
                string usedtype = string.Empty;
                for (int i = 1; i <= bakdlyList.Count; i++)
                {
                    szRowId += "ǎǒǜ" + i;
                    usedtype += "ǎǒǜ1";
                }
                #region Z_InsertBakdly 存储过程
                if (id > 0)
                {
                    DynamicParameters dy = new DynamicParameters();
                    dy.Add("@szRowId", szRowId);
                    dy.Add("@vchcode", id);
                    dy.Add("@ColRowNo", "ǎǒǜ");
                    dy.Add("@atypeid", "ǎǒǜ");
                    dy.Add("@ktypeid2", "ǎǒǜ");
                    dy.Add("@kwtypeid", "ǎǒǜ");
                    dy.Add("@btypeid", Segmentation(dly.DetailList.Select(p => p.BTypeID)));
                    dy.Add("@etypeid", Segmentation(dly.DetailList.Select(p => p.ETypeID)));
                    dy.Add("@ktypeid", Segmentation(dly.DetailList.Select(p => p.KTypeID)));
                    dy.Add("@PtypeId", Segmentation(dly.DetailList.Select(p => p.PTypeID)));
                    dy.Add("@Qty", Segmentation(dly.DetailList.Select(p => p.Qty)));
                    dy.Add("@discount", Segmentation(dly.DetailList.Select(p => p.Discount)));
                    dy.Add("@DiscountPrice", Segmentation(dly.DetailList.Select(p => p.DiscountPrice)));
                    dy.Add("@unit", Segmentation(dly.DetailList.Select(p => p.Unit)));
                    dy.Add("@total", Segmentation(dly.DetailList.Select(p => p.Total)));
                    dy.Add("@date", Segmentation(dly.DetailList.Select(p => p.Date)));
                    dy.Add("@Price", Segmentation(dly.DetailList.Select(p => p.Price)));
                    dy.Add("@discounttotal", Segmentation(dly.DetailList.Select(p => p.DisCountTotal)));

                    dy.Add("@CostTotal", "ǎǒǜ0");
                    dy.Add("@CostPrice", "ǎǒǜ0");
                    dy.Add("@Blockno", "ǎǒǜ");
                    dy.Add("@goodsno", "ǎǒǜ");
                    dy.Add("@Prodate", "ǎǒǜ");
                    dy.Add("@TaxPrice", "ǎǒǜ");
                    dy.Add("@TaxTotal", "ǎǒǜ0");
                    dy.Add("@Comment", "ǎǒǜ");
                    dy.Add("@usedtype", usedtype);
                    dy.Add("@period", "ǎǒǜ");
                    dy.Add("@Tax_total", Segmentation(dly.DetailList.Select(p => p.DisCountTotal)));
                    dy.Add("@tax", "ǎǒǜ0");
                    dy.Add("@dAssQty", Segmentation(dly.DetailList.Select(p => p.Qty)));
                    dy.Add("@dAssPrice", Segmentation(dly.DetailList.Select(p => p.Price)));
                    dy.Add("@dAssDiscountPrice", Segmentation(dly.DetailList.Select(p => p.DiscountPrice)));
                    dy.Add("@dAssTaxPrice", Segmentation(dly.DetailList.Select(p => p.DiscountPrice)));
                    dy.Add("@dUnitRate", "ǎǒǜ");
                    dy.Add("@vchtype", "11");
                    dy.Add("@nCostMode", "ǎǒǜ");
                    dy.Add("@nOrderCode", "ǎǒǜ");
                    dy.Add("@nOrderDlyCode", "ǎǒǜ");
                    dy.Add("@nOrderVchtype", "ǎǒǜ");
                    dy.Add("@dInvoceTotal", "ǎǒǜ0");
                    dy.Add("@szProjectid", "ǎǒǜ");
                    dy.Add("@SideQty", "ǎǒǜ0");
                    dy.Add("@RetailPrice", "ǎǒǜ0");

                    var rvid = DataBaseUtility.QueryFirst<int>("Z_InsertBakdly", dbName, dy, commandType: CommandType.StoredProcedure);
                    if (rvid == -1)
                    {
                        dict["result"] = 0;
                    }
                    #endregion
                }
                else
                {
                    dict["result"] = 1;
                }
            }
            catch (Exception e)
            {
                LogUtils.Error("InitListen:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
            return dict;
        }

        public static string Segmentation(IEnumerable<string> strs)
        {
            var str = "ǎǒǜ" + string.Join("ǎǒǜ", strs);
            return "ǎǒǜ" + string.Join("ǎǒǜ", strs);
        }

        public static string Segmentation(IEnumerable<double> strs)
        {
            return "ǎǒǜ" + string.Join("ǎǒǜ", strs);
        }

        public static string Segmentation(IEnumerable<int> strs)
        {
            return "ǎǒǜ" + string.Join("ǎǒǜ", strs);
        }
    }
}
