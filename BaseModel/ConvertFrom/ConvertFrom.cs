using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseModel.ConvertFrom
{
    public class ConvertFrom
    {
        /// <summary>
        /// ByteArrayToHexString
        /// </summary>
        /// <param name="ByteList"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] ByteList)
        {
            string text = string.Empty;
            try
            {
                for (int i = 0; i < ByteList.Length; i++)
                {
                    //text = text + " " + Convert.ToString(ByteList[i], 16);
                    text = text + " " + string.Format("{0:X2}", ByteList[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换异常" + ex);
            }
            text = text.Trim();
            return text;
        }
        /// <summary>
        /// UintArrayToHexString
        /// </summary>
        /// <param name="ByteList"></param>
        /// <returns></returns>
        public static string UintArrayToHexString(uint[] ByteList)
        {
            string text = string.Empty;
            try
            {
                for (int i = 0; i < ByteList.Length; i++)
                {
                    text = text + " " + string.Format("{0:X2}", ByteList[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换异常" + ex);
            }
            text = text.Trim() + "\r\n";
            return text;
        }

        public static string ByteArrayToString(byte[] ByteList, Encoding encoding)
        {
            string result = string.Empty;
            try
            {
                result = encoding.GetString(ByteList);
            }
            catch (Exception ex)
            {
                throw new Exception("转换异常" + ex);

            }
            return result;
        }

        public static string HexStringToString(string HexString, Encoding encoding)
        {
            string result = string.Empty;
            try
            {
                char[] array = new char[]
                {
            '%',
            ' ',
            '$',
            '&'
                };
                HexString = HexString.Trim(array);
                string[] array2 = HexString.Split(array);
                byte[] array3 = new byte[array2.Length];
                for (int i = 0; i < array2.Length; i++)
                {
                    array3[i] = Convert.ToByte(array2[i], 16);
                }
                result = encoding.GetString(array3);
            }
            catch (Exception ex)
            {
                throw new Exception("转换异常" + ex);
            }
            return result;
        }

        public static byte[] StringToByteArray(string String, Encoding encoding)
        {
            byte[] result;
            try
            {
                result = encoding.GetBytes(String);
            }
            catch (Exception ex)
            {
                throw new Exception("转换异常" + ex);
            }
            return result;
        }

        public static string StringToHexString(string String, Encoding encoding)
        {
            string text = string.Empty;
            try
            {
                byte[] bytes = encoding.GetBytes(String);
                for (int i = 0; i < bytes.Length; i++)
                {
                    text = text + " " + Convert.ToString(bytes[i], 16);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换异常" + ex);
            }
            text = text.Trim();
            return text;
        }
    }
}
