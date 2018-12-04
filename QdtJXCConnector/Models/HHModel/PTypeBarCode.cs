using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class PTypeBarCode : Base
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

        public string BarCode
        {
            get;
            set;
        }

        public int Ordid
        {
            get;
            set;
        }
    }
}
