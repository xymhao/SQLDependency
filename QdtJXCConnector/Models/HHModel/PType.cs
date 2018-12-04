using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class PType : Base
    {
        /// <summary>
        /// 商品主键ID
        /// </summary>
        public string PTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParID
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
        /// <summary>
        /// 层级
        /// </summary>
        public int Leveal
        {
            get;
            set;
        }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level
        {
            get { return Leveal; }
        }

        /// <summary>
        /// 子节点数量
        /// </summary>
        public int PSonNum
        {
            get;
            set;
        }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string PUserCode
        {
            get;
            set;
        }

        /// <summary>
        /// 商品全称
        /// </summary>
        public string PFullName
        {
            get;
            set;
        }

        /// <summary>
        /// 商品规格
        /// </summary>
        public string Standard
        {
            get;
            set;
        }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// 商品价格信息
        /// </summary>
        public List<PTypePrice> PTypePriceList
        {
            get;
            set;
        }

        /// <summary>
        /// 商品仓库价格信息
        /// </summary>
        public List<PTypeKPrice> PTypeKPriceList
        {
            get;
            set;
        }

        /// <summary>
        /// 商品单位信息
        /// </summary>
        public List<PTypeUnit> PTypeUnitList
        {
            get;
            set;
        }
    }
}
