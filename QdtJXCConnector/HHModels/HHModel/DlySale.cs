using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class DlySale
    {
        public double VChcode
        {
            get;
            set;
        }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public string BTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 经手人ID
        /// </summary>
        public string ETypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string KTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string PTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 数量
        /// </summary>
        public double Qty
        {
            get;
            set;
        }

        /// <summary>
        /// 折扣
        /// </summary>
        public double Discount
        {
            get;
            set;
        }

        /// <summary>
        /// 折扣单价
        /// </summary>
        public double DiscountPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 日期
        /// </summary>
        public string Date
        {
            get;
            set;
        }

        /// <summary>
        /// 商品单位
        /// </summary>
        public int Unit
        {
            get;
            set;
        }

        /// <summary>
        /// 单价
        /// </summary>
        public double Price
        {
            get;
            set;
        }

        /// <summary>
        /// 金额
        /// </summary>
        public double Total
        {
            get;
            set;
        }

        /// <summary>
        /// 折后总金额
        /// </summary>
        public double DisCountTotal
        {
            get;
            set;
        }

        public int VChType
        {
            get
            {
                return 11;
            }
            set
            {
                VChType = value;
            }
        }
    }
}
