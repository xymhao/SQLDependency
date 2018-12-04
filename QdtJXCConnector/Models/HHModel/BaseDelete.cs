using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class BaseDelete<T> : Base
    {
        public T ID
        {
            get;
            set;
        }

        public string TableName
        {
            get;
            set;
        }

    }
}
