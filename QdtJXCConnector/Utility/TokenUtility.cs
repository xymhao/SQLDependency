using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public static class TokenUtility
    {
        private readonly static string _kkeyString = @"8ffbdf2835e68d62a3830e7baec26ddb";
        private readonly static string _kalphabet = @"abcdefghigklmnopkrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
		/// 返回一个指定长度的随机数
		/// </summary>
		/// <param name="length">随机数长度</param>
		/// <returns></returns>
        public static Dictionary<string,string> GetTokenAndRandom()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < 6; i++)
            {
                newRandom.Append(_kalphabet[rd.Next(62)]);
            }
            dict.Add("Random", newRandom.ToString());
            dict.Add("Token", _kkeyString.Insert(10, newRandom.ToString()));
            return dict;
        }

        public static bool ValidateToken(string token, string random)
        {
            bool right = false;
            string rightToken = EncryptUtility.GetMD5Hash(_kkeyString.Substring(0, 10) + random + _kkeyString.Substring(10, _kkeyString.Length - 10));

            if (rightToken.ToUpper().Equals(token.ToUpper()))
            {
                right = true;
            }
            return right;
        }
    }
}
