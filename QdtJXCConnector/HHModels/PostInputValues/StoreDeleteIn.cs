using Models.HHModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PostInputValues
{
    public class StoreDeleteIn : Base
    {
        public string KTypeID
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
