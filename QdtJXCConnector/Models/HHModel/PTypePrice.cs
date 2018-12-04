using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    /// <summary>
    /// 商品价格表
    /// </summary>
    public class PTypePrice
    {
        public string PTypeID
        {
            get;
            set;
        }

        public int UnitID
        {
            get;
            set;
        }

        public string PrTypeID
        {
            get;
            set;
        }

        public double Price
        {
            get;
            set;
        }
    }
}
