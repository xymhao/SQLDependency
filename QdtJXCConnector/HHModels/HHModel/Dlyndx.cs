using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class Dlyndx : Base
    {
        public double VChcode
        {
            get;
            set;
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string Number
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
        /// 过账状态，1表示草稿，2表示过账
        /// </summary>
        public int Draft
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
        /// 经手人
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
        /// 制单人
        /// </summary>
        public string InputNo
        {
            get;
            set;
        }

        public string SaveDate
        {
            get;
            set;
        }

        /// <summary>
        /// 审批状态
        /// </summary>
        public int IfCheck
        {
            get;
            set;
        }

        /// <summary>
        /// 销售单总金额
        /// </summary>
        public double Total
        {
            get;
            set;
        }

        /// <summary>
        /// 整单折扣
        /// </summary>
        public double DefDisCount
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }

        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// 过账数据
        /// </summary>
        public List<DlySale> DetailList
        {
            get;
            set;
        }
    }
}
