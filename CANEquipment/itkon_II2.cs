using AbstractEquipment;
using BaseModel.ConvertFrom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CANEquipment
{
    public class itkon_II2 : CANEquipmentAbstract
    {
        protected struct VCI_INIT_CONFIG
        {
            public UInt32 AccCode;//验收代码
            public UInt32 AccMask;//屏蔽代码
            public UInt32 Reserved;//保留位
            public byte Filter;//滤波方式，1 表示单滤波，0 表示双滤波
            public byte Timing0;
            public byte Timing1;
            //CAN 波特率 定时器 0 定时器 1 
            //5Kbps 0xBF 0xFF
            //10Kbps 0x31 0x1C
            //20Kbps 0x18 0x1C
            //40Kbps 0x87 0xFF
            //50Kbps 0x09 0x1C
            //80Kbps 0x83 0xFF
            //100Kbps 0x04 0x1C
            //125Kbps 0x03 0x1C
            //200Kbps 0x81 0xFA
            //250Kbps 0x01 0x1C
            //400Kbps 0x80 0xFA
            //500Kbps 0x00 0x1C
            //666Kbps 0x80 0xB6
            //800Kbps 0x00 0x16
            //1000Kbps 0x00 0x14
            public byte Mode;//模式，0 表示正常模式，1 表示只听模式
        }

        unsafe protected struct VCI_CAN_OBJ  //使用不安全代码
        {
            public uint ID;
            public uint TimeStamp;
            public byte TimeFlag;
            public byte SendType;
            public byte RemoteFlag;//是否是远程帧
            public byte ExternFlag;//是否是扩展帧
            public byte DataLen; //数据长度DLC(<=8)，即CAN帧Data有几个字节。约束了后面Data[8] 中的有效字节。

            public fixed byte Data[8];

            public fixed byte Reserved[3];

        }

        #region DLL 
        [DllImport("controlcan.dll")]
        static protected extern UInt32 VCI_OpenDevice(UInt32 DevicesType, UInt32 DevicesIndex, UInt32 Reserved);

        [DllImport("controlcan.dll")]
        static protected extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceIndex);
        [DllImport("controlcan.dll")]
        static protected extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceIndex, UInt32 CANIndex, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport("controlcan.dll")]
        static protected extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DevicesIndex, UInt32 CANIndex);
        [DllImport("controlcan.dll")]
        static protected extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceIndex, UInt32 CANIndex);
        [DllImport("controlcan.dll")]
        static protected extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceIndex, UInt32 CANIndex);

        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static protected extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);

        [DllImport("controlcan.dll")]
        static protected extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceIndex, UInt32 CANIndex, ref VCI_CAN_OBJ pSend, UInt32 Len);
        #endregion

        unsafe protected string ReceiveData(UInt32 FilterID, int timeout, uint deviceType, uint deviceIndex, uint cANIndex)
        {
            string dataStrList = "读取失败！";
            List<string> dataStringList = new List<string>();
            var cancelTokenSource = new CancellationTokenSource(timeout);
            if (IsOpen) //是否打开CAN
            {
                List<uint> Data_Frame = new List<uint>();
                while (!cancelTokenSource.IsCancellationRequested)//设置读取超时
                {
                    //uint num = VCI_GetReceiveNum(deviceType, deviceIndex, cANIndex);
                    uint con_maxlen = 100;
                    int size = Marshal.SizeOf(typeof(VCI_CAN_OBJ));//获取结构体占用空间大小
                    IntPtr pt = Marshal.AllocHGlobal(size * (Int32)con_maxlen);//声明一个同样大小的空间的指针或句柄
                    UInt32 result = VCI_Receive(deviceType, deviceIndex, cANIndex, pt, con_maxlen, 100);//返回最大100幁数据
                    for (UInt32 i = 0; i < result; i++)
                    {
                        VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((uint)pt + i * size), typeof(VCI_CAN_OBJ));//将结构体放到这个空间中
                        if (obj.ID == FilterID)//判断是否为过滤的ID
                        {
                            if (obj.RemoteFlag == 0)//判断是否为远程幁
                            {
                                //Data_Frame.Add(obj.ID);
                                byte len = (byte)(obj.DataLen % 9);
                                byte j = 0;
                                if (j++ < len)
                                {
                                    for (int d = 0; d < len; d++)
                                    {
                                        Data_Frame.Add(obj.Data[d]);
                                    }
                                }
                            }
                            dataStrList = ConvertFrom.UintArrayToHexString(Data_Frame.ToArray());
                        }
                    }
                    #region old
                    //dataStr = "帧ID: ";
                    //dataStr = Convert.ToString(obj.ID, 16);
                    //    if (obj.ID == FilterID)
                    //    {
                    //        if (obj.RemoteFlag == 0)
                    //        {
                    //            //dataStr += " 数据: ";
                    //            byte len = (byte)(obj.DataLen % 9);
                    //            byte j = 0;
                    //            if (j++ < len)
                    //            {
                    //                for (int d = 0; d < len; d++)
                    //                {
                    //                    dataStr += " " + string.Format("{0:X2}", obj.Data[d]);//01 00 00 00 00 00 00 08
                    //                }
                    //            }
                    //            dataStringList.Add(dataStr.ToUpper());
                    //        }
                    //    }
                    //    //Console.WriteLine(dataStr.ToUpper());
                    //}


                    //if (dataStringList.Count > 0)
                    //{
                    //    foreach (var item in dataStringList)
                    //    {
                    //        dataStrList += item + "\r\n";
                    //    }
                    //    return dataStrList;
                    //}//判断是否有 指定的数据
                    //else
                    //{
                    //    dataStringList = null;
                    //    dataStrList = "";
                    //    //ReceiveData(FilterID, timeout);
                    //    return $"CAN1 读取帧ID: {Convert.ToString(FilterID, 16).ToUpper()} 失败！";
                    //}
                    #endregion
                }
                return dataStrList;
            }

            return dataStrList;
            //return $"CAN1 读取帧ID: {Convert.ToString(FilterID, 16).ToUpper()} 失败！";
        }

        unsafe protected bool TransmitData(string Data, uint deviceType, uint deviceIndex, uint cANIndex, uint frameid)
        {
            bool IsTransmitSuccess = false;
            if (IsOpen)
            {
                VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ()
                {
                    SendType = 2,//=0时为正常发送，=1时为单次发送，=2时为自发自收，=3时为单次自发自收，只在此帧为发送帧时有意义。
                    ID = frameid,//帧ID
                    DataLen = 8,//数据长度
                    RemoteFlag = 0,//是否是远程帧
                    ExternFlag = 0,//是否是扩展帧。
                    TimeFlag = 1//是否使用时间标识，为 1 时 TimeStamp 有效，TimeFlag 和 TimeStamp 只在此帧为接收帧时有意义
                };
                VCI_CAN_OBJ[] vCI_CAN_OBJ = new VCI_CAN_OBJ[]
                {
                    new VCI_CAN_OBJ()
                {
                    SendType = 2,//=0时为正常发送，=1时为单次发送，=2时为自发自收，=3时为单次自发自收，只在此帧为发送帧时有意义。
                    ID = frameid,//帧ID
                    DataLen = 8,//数据长度
                    RemoteFlag = 0,//是否是远程帧
                    ExternFlag = 0,//是否是扩展帧。
                    TimeFlag = 1//是否使用时间标识，为 1 时 TimeStamp 有效，TimeFlag 和 TimeStamp 只在此帧为接收帧时有意义
                },
                    new VCI_CAN_OBJ()
                {
                    SendType = 2,//=0时为正常发送，=1时为单次发送，=2时为自发自收，=3时为单次自发自收，只在此帧为发送帧时有意义。
                    ID = frameid,//帧ID
                    DataLen = 8,//数据长度
                    RemoteFlag = 0,//是否是远程帧
                    ExternFlag = 0,//是否是扩展帧。
                    TimeFlag = 1//是否使用时间标识，为 1 时 TimeStamp 有效，TimeFlag 和 TimeStamp 只在此帧为接收帧时有意义
                },
            };
                string strdata = Data;
                int len = (strdata.Length + 1) / 3;
                List<byte> bytelist = new List<byte>();
                for (int t = 0; t < len; t++)
                {
                    bytelist.Add(System.Convert.ToByte("0x" + strdata.Substring(t * 3, 2), 16));
                    sendobj.Data[t] = bytelist[t];

                    //vCI_CAN_OBJ[0].Data[t] = bytelist[t];
                }
                if (VCI_Transmit(deviceType, deviceIndex, cANIndex, ref sendobj, 2) == 1)
                {
                    IsTransmitSuccess = true;
                }
                return IsTransmitSuccess;
            }
            return IsTransmitSuccess;

        }


        public override bool CancelCAN(uint DeviceType, uint DeviceIndex, uint CANIndex)
        {
            if (IsOpen)
            {
                VCI_CloseDevice(DeviceIndex, DeviceIndex);
                uint i = VCI_ResetCAN(DeviceType, DeviceIndex, CANIndex);
                if (i!=0)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool initializeCAN(uint DeviceType, uint DeviceIndex, uint CANIndex, byte baudratio)
        {
            try
            {
                VCI_OpenDevice(DeviceType, DeviceIndex, 0);
                VCI_INIT_CONFIG _CONFIG = new VCI_INIT_CONFIG()
                {
                    AccCode = 0,             //验收代码
                    AccMask = 0xffffffff,   //屏蔽代码
                    Reserved = 0,           //保留位
                    Filter = 1,             //滤波方式，1 表示单滤波，0 表示双滤波
                    Mode = 0,               //模式，0 表示正常模式，1 表示只听模式
                    Timing0 = 0,
                    Timing1 = baudratio          //设置波特率
                };
                UInt32 ss = VCI_InitCAN(DeviceType, DeviceIndex, CANIndex, ref _CONFIG);
                if (ss != 0)
                {
                    VCI_StartCAN(DeviceType, DeviceIndex, CANIndex);
                    IsOpen = true;
                }
                else
                {
                    return IsOpen;
                }
                return IsOpen;
            }
            catch (Exception ex)
            {
                throw new Exception(ex + "初始化CAN 失败");
            }
        }

        public override string Query(string command, uint filterID, uint deviceType, uint deviceIndex, uint cANIndex, uint frameid)
        {
            TransmitData(command, deviceType, deviceIndex, cANIndex, frameid);

            return ReceiveData(filterID, 1000, deviceType, deviceIndex, cANIndex);
        }

        public override string Read(uint command, uint deviceType, uint deviceIndex, uint cANIndex)
        {
            return ReceiveData(command, 500, deviceType, deviceIndex, cANIndex);
        }

        public override bool Write(string command, uint deviceType, uint deviceIndex, uint cANIndex, uint frameid)
        {
           return TransmitData(command, deviceType, deviceIndex, cANIndex, frameid);
        }
    }
}
