using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Models.HHModel
{
    public class Base
    {
        public string DogNumber
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public string Random
        {
            get;
            set;
        }

        public string DbName
        {
            get;
            set;
        }

        public Base()
        {
            var tokens = TokenUtility.GetTokenAndRandom();
            Token = EncryptUtility.GetMD5Hash(tokens["Token"]);
            Random = tokens["Random"];
        }
    }
}
