using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public class PTypePriceDao
    {
        public static List<PTypePrice> GetPTypePriceByID(string pTypeID, string dbName)
        {
            List<PTypePrice> ls = new List<PTypePrice>();
            string TSQL = @" SELECT PTypeID
                                   ,UnitID
                                   ,PrTypeID
                                   ,Price
                               FROM XW_P_PtypePrice
                              WHERE PTypeID= @PTypeID; ";
            ls = DataBaseUtility.Query<PTypePrice>(TSQL, dbName, new { PTypeID = pTypeID });
            return ls;
        }
    }
}
