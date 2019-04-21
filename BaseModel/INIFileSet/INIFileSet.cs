using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseModel.INIFileSet
{
    public class ctlINI
    {
        private string FileName;    //必须是绝对的路径名

        //构造函数，定义INI文件的路径名
        public void SetFile(string filename)
        {
            FileName = filename;
        }

        //根据FileName、Section、Key，读取value值（int）
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(
           string WSection,  // 指向包含 Section 名称的字符串地址 
           string WKey,  // 指向包含 Key 名称的字符串地址 
           int WDefaultValue,    // 如果 Value 值没有找到，则返回缺省的值是多少 
           string WFileName
           );

        //根据FileName、Section、Key，读取value值（string）[指定地址和长度]
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
           string WSection, // 指向包含 Section 名称的字符串地址 
           string WKey, // 指向包含 Key 名称的字符串地址 
           string WDefaultValue,    // 如果 Value 值没有找到，则返回缺省的字符串的地址 
           StringBuilder WReturnedString,   // 返回字符串的缓冲区地址 
           int WSize,   // 缓冲区的长度 
           string WFileName
           );

        //根据FileName、Section、Key，写入value值（string）
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(
           string WSection, // 指向包含 Section 名称的字符串地址 
           string WKey, // 指向包含 Key 名称的字符串地址 
           string WValue,   // 要写的字符串地址 
           string WFileName
           );

        //根据FileName，读取Section的数量[指定字节数组和数组长度]
        [DllImport("kernel32.dll")]
        internal extern static int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string WFileName);

        //根据FileName、Section，读取Key的数量
        [DllImport("kernel32.dll")]
        internal extern static int GetPrivateProfileStringA(string segName, string keyName, string sDefault, byte[] buffer, int iLen, string WFileName); // ANSI版本

        //根据FileName、Section、Key，读取value值（int）
        internal int GetInt(string section, string key, int def)
        {
            return GetPrivateProfileInt(section, key, def, FileName);
        }

        //根据FileName、Section、Key，读取value值（string）
        public string GetString(string section, string key, string def)
        {
            StringBuilder temp = new StringBuilder(1024);   //指定要存储的字符串和长度
            GetPrivateProfileString(section, key, def, temp, 1024, FileName);
            return temp.ToString();
        }

        //根据FileName、Section、Key，写入value值（int）
        internal void WriteInt(string section, string key, int iVal)
        {
            WritePrivateProfileString(section, key, iVal.ToString(), FileName);
        }
        //根据FileName、Section、Key，写入value值（string）
        internal void WriteString(string section, string key, string strVal)
        {
            WritePrivateProfileString(section, key, strVal, FileName);
        }
        //根据FileName、Section、Key，删除Key
        internal void DelKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, FileName);
        }
        //根据FileName、Section，删除Section
        internal void DelSection(string section)
        {
            WritePrivateProfileString(section, null, null, FileName);
        }

        //根据FileName，读取所有的Section
        internal ArrayList ReadSections()
        {
            byte[] buffer = new byte[65535];    //指定要存储字节数组和数组长度
            int rel = GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), FileName);
            int iCnt, iPos;
            ArrayList arrayList = new ArrayList();  //使用到集合
            string tmp;
            if (rel > 0)
            {
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)  //循环每个
                {
                    if (buffer[iCnt] == 0x00)   //
                    {
                        /* 
                         * 将字节数组中一个字节取出转换成字符串
                         * 按照ASCII编码
                         * 调用System.Text的名称空间
                         * 去掉前后多余的空格
                         * */
                        tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                        iPos = iCnt + 1;
                        if (tmp != "")
                            arrayList.Add(tmp);
                    }
                }
            }
            return arrayList;
        }

        //根据FileName、Section，读取Key的数量
        public ArrayList ReadKeys(string sectionName)
        {
            byte[] buffer = new byte[5120]; //指定要存储字节数组和数组长度
            int rel = GetPrivateProfileStringA(sectionName, null, "", buffer, buffer.GetUpperBound(0), FileName);
            int iCnt, iPos;
            ArrayList arrayList = new ArrayList();
            string tmp;
            if (rel > 0)
            {
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)
                {
                    if (buffer[iCnt] == 0x00)   //
                    {
                        /* 
                         * 将字节数组中一个字节取出转换成字符串
                         * 按照ASCII编码
                         * 调用System.Text的名称空间
                         * 去掉前后多余的空格
                         * */
                        tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                        iPos = iCnt + 1;
                        if (tmp != "")
                            arrayList.Add(tmp);
                    }
                }
            }
            return arrayList;
        }
    }
}
