using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public static class DlySaleDao
    {
        public static List<DlySale> GetDlySaleByVchcode(double vChcode, string dbName)
        {
            List<DlySale> ls = new List<DlySale>();
            string TSQL = @" SELECT VChcode
                                   ,BTypeID
                                   ,ETypeID
                                   ,KTypeID
                                   ,PTypeID
                                   ,Qty
                                   ,Discount
                                   ,DiscountPrice
                                   ,Date
                                   ,Unit
                                   ,Price
                                   ,Total
                                   ,DisCountTotal 
                               FROM DlySale
                              WHERE VChcode = @VChcode ";
            ls = DataBaseUtility.Query<DlySale>(TSQL,dbName, new { VChcode = vChcode });
            return ls;
        }
    }
}
