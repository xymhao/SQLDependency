using Models.HHModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PostInputValues
{
    public class BTypeDeleteIn : Base
    {
        public string BTypeID
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }
    }
}
