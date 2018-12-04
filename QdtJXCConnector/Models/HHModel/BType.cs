using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class BType : Base
    {
        public string BTypeID
        {
            get;
            set;
        }

        public string ParID
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public int Leveal
        {
            get;
            set;
        }

        public int Deleted
        {
            get;
            set;
        }

        public int IsStop
        {
            get;
            set;
        }

        public int BSonNum
        {
            get;
            set;
        }

        public string BUserCode
        {
            get;
            set;
        }

        public string BFullName
        {
            get;
            set;
        }

        /// <summary>
        /// 累计应收款
        /// </summary>
        public decimal ARTotal
        {
            get;
            set;
        }

        /// <summary>
        /// 累计应付款
        /// </summary>
        public decimal APTotal
        {
            get;
            set;
        }

        /// <summary>
        /// 累计预收款
        /// </summary>
        public decimal YRTotal
        {
            get;
            set;
        }

        /// <summary>
        ///累计预付款 
        /// </summary>
        public decimal YPTotal
        {
            get;
            set;
        }
    }
}
