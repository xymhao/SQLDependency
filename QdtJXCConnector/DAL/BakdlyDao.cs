using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public class BakdlyDao
    {
        public static List<DlySale> GetBakDlyByVchcode(double vChcode, string dbName)
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
                               FROM Bakdly
                              WHERE VChcode = @VChcode ";
            ls = DataBaseUtility.Query<DlySale>(TSQL, dbName, new { VChcode = vChcode });
            return ls;
        }
    }
}
