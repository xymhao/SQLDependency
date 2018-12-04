using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utils
{
	public static class EncryptUtility
	{
        public static string secretKey = "1qdtsoft";
		/// <summary>
		/// MD5加密
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string GetMD5Hash(String input)
		{
			return GetMD5Hash(Encoding.Default.GetBytes(input));
		}

		public static string GetMD5Hash(List<byte> bytes)
		{
			return GetMD5Hash(bytes.ToArray());
		}

		public static string GetMD5Hash(byte[] bytes)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] res = md5.ComputeHash(bytes, 0, bytes.Length);
			StringBuilder sbuilder = new StringBuilder();
			for (int i = 0; i < res.Length; i++)
			{
				sbuilder.Append(res[i].ToString("x2"));
			}
			return sbuilder.ToString();
		}

		public static string GetMd5Str(byte[] source)
		{
			string result = "";
			MD5 m = new MD5CryptoServiceProvider();
			//byte[] s = m.ComputeHash(UnicodeEncoding.UTF8.GetBytes(source));
			//byte[] ss = new byte[30];
			//for (int i = 0; i < 30; i++)
			//{
			//    ss[i] = source[i];
			//}
			byte[] s = m.ComputeHash(source);
			result = BitConverter.ToString(s).Replace("-", "");
			m.Clear();
			return result.ToUpper();
		}

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string pToEncrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = Encoding.ASCII.GetBytes(secretKey);
            des.IV = Encoding.ASCII.GetBytes(secretKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// MD5解密
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string MD5Decrypt(string pToDecrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = Encoding.ASCII.GetBytes(secretKey);
            des.IV = Encoding.ASCII.GetBytes(secretKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// 创建Key
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DES.Create();
            return Encoding.ASCII.GetString(desCrypto.Key);
        }
    }
}
