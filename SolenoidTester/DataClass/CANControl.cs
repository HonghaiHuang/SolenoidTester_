using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

#region CAN结构体

public struct VCI_BOARD_INFO
{
    public ushort hw_Version;
    public ushort fw_Version;
    public ushort dr_Version;
    public ushort in_Version;
    public ushort irq_Num;
    public byte can_Num;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] str_Serial_Num;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)] public byte[] str_hw_Type;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] Reserved;
}

//2.定义CAN信息帧的数据类型。
public unsafe struct VCI_CAN_OBJ //使用不安全代码
{
    public uint ID;
    public uint TimeStamp;
    public byte TimeFlag;
    public byte SendType;
    public byte RemoteFlag; //是否是远程帧
    public byte ExternFlag; //是否是扩展帧
    public byte DataLen;

    public fixed byte Data [8];

    public fixed byte Reserved [3];
}

public struct VCI_CAN_STATUS
{
    public byte ErrInterrupt;
    public byte regMode;
    public byte regStatus;
    public byte regALCapture;
    public byte regECCapture;
    public byte regEWLimit;
    public byte regRECounter;
    public byte regTECounter;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] Reserved;
}

//4.定义错误信息的数据类型。
public struct VCI_ERR_INFO
{
    public uint ErrCode;
    public byte Passive_ErrData1;
    public byte Passive_ErrData2;
    public byte Passive_ErrData3;
    public byte ArLost_ErrData;
}

//5.定义初始化CAN的数据类型
public struct VCI_INIT_CONFIG
{
    public uint AccCode;
    public uint AccMask;
    public uint Reserved;
    public byte Filter;
    public byte Timing0;
    public byte Timing1;
    public byte Mode;
}

public struct CHGDESIPANDPORT
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public byte[] szpwd;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] szdesip;

    public int desport;

    public void Init()
    {
        szpwd = new byte[10];
        szdesip = new byte[20];
    }
}

public struct VCI_FILTER_RECORD
{
    public uint ExtFrame;
    public uint Start;
    public uint End;
}

#endregion

#region 委托

/// <summary>
/// TCU连接状态委托
/// </summary>
/// <param name="connected"></param>
public delegate void CanConnectStatsEventHandler(bool connected);

#endregion

namespace SolenoidTester
{
    internal class CANControl
    {
        #region 委托事件

        /// <summary>
        /// TCU连接状态委托事件
        /// </summary>
        public event CanConnectStatsEventHandler CanConnectStats;

        #endregion

        #region 变量

        /// <summary>
        /// 设备类型号
        /// </summary>
        private static readonly uint m_devtype = 20;

        /// <summary>
        /// SetReference返回状态
        /// </summary>
        private const uint STATUS_OK = 1;

        /// <summary>
        /// 设备索引号
        /// </summary>
        private readonly uint m_devind = 0;

        /// <summary>
        /// CAN路数
        /// </summary>
        private readonly uint m_canind = 0;

        /// <summary>
        /// CAN连接状态
        /// </summary>
        private bool connected;

        /// <summary>
        /// 接收CAN上数据数组
        /// </summary>
        private string[] dataarr;

        /// <summary>
        /// 提示框
        /// </summary>
        private bool messageBoxNotice = false;

        #endregion

        /// <summary>
        ///     CAN连接
        /// </summary>
        public unsafe bool ConncetCan()
        {
            if (connected) return connected;
            var config = new VCI_INIT_CONFIG
            {
                AccCode = Convert.ToUInt32("0x00000000", 16),
                AccMask = Convert.ToUInt32("0xFFFFFFFF", 16),
                Timing0 = Convert.ToByte("0x00", 16),
                Timing1 = Convert.ToByte("0x1C", 16),
                Filter = Convert.ToByte(1),
                Mode = Convert.ToByte(0)
            };
            if (VCI_OpenDevice(m_devtype, m_devind, 0) == 0)
            {
                MessageBox.Show("CAN卡未正常连接，请连接CAN卡", "提示");
            }
            else
            {
                var baud = Convert.ToUInt32("0x060007", 16);
                if (VCI_SetReference(m_devtype, m_devind, m_canind, 0, (byte*) &baud) != STATUS_OK)
                {
                    MessageBox.Show("设置波特率错误，打开设备失败!", "错误");
                    VCI_CloseDevice(m_devtype, m_devind);
                }
                else
                {
                    VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);
                    var open = VCI_StartCAN(m_devtype, m_devind, m_canind);
                    if (open == 1)
                    {
                        connected = true;
                    }

                    if (open == 0)
                    {
                        connected = false;
                        VCI_CloseDevice(m_devtype, m_devind);
                    }
                    CanConnectStats?.Invoke(connected);
                }
            }
            return connected;
        }

        /// <summary>
        ///     断开CAN连接
        /// </summary>
        public bool DisConnectCan()
        {
            if (!connected) return connected;
            if (VCI_CloseDevice(m_devtype, m_devind) != 1) return connected;
            connected = false;
            CanConnectStats?.Invoke(connected);
            return connected;
        }

        /// <summary>
        ///     发送数据
        /// </summary>
        /// <param name="id">发送ID</param>
        /// <param name="data">发送数据</param>
        public unsafe void SendData(string id, string data)
        {
            
            if (!connected) return;
            var SendID = "0x00000" + id.ToUpper();
            var sendobj = new VCI_CAN_OBJ
            {
                SendType = 0,
                RemoteFlag = 0,
                ExternFlag = 0,
                ID = Convert.ToUInt32(SendID, 16)
            };
            var len = (data.Length + 1)/3;
            sendobj.DataLen = Convert.ToByte(len);
            var strdata = data;
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 7E2：" + strdata);
            var i = -1;
            if (i++ < len - 1)
                sendobj.Data[0] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[1] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[2] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[3] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[4] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[5] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[6] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[7] = Convert.ToByte("0x" + strdata.Substring(i*3, 2), 16);
            var nTimeOut = 3000;
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN开始发送数据");
            VCI_SetReference(m_devtype, m_devind, m_canind, 4, (byte*) &nTimeOut);
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + id + ": " + strdata);
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN结束发送数据");

            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                if (connected)
                {
                    if (!messageBoxNotice)
                    {
                        messageBoxNotice = true;
                        if (MessageBox.Show("TCU指令发送失败，请检查TCU是否正常连接", "Confirmation", MessageBoxButton.OK) == MessageBoxResult.Yes)
                        {
                            messageBoxNotice = false;
                        }
                    }
                    //MessageBox.Show("TCU指令发送失败，请检查TCU是否正常连接");
                }
            }
        }

        /// <summary>
        ///     接收数据
        /// </summary>
        /// <returns>接收到的数据</returns>
        public unsafe string[] RecviveData()
        {
            dataarr = new string[] {};
            if (!connected) return dataarr;
            var str = "";
            var saveCount = 0;
            var strarr = new string[10000];
            var dataCount = 0;

            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "              查看CAN上有没有数据");
            var res = VCI_GetReceiveNum(m_devtype, m_devind, m_canind);
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "              查看CAN上有没有数据：       "+ res);
            if (res != 0)
            {
                var conMaxlen = res;
                var pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ))*(int) conMaxlen);

                res = VCI_Receive(m_devtype, m_devind, m_canind, pt, conMaxlen, 1);

                for (uint readCanBufferNum = 0; readCanBufferNum < res; readCanBufferNum++)
                {
                    var obj =
                        (VCI_CAN_OBJ)
                            Marshal.PtrToStructure(
                                (IntPtr) ((uint) pt + readCanBufferNum*Marshal.SizeOf(typeof(VCI_CAN_OBJ))),
                                typeof(VCI_CAN_OBJ));

                    var ID = Convert.ToString((int) obj.ID, 16);
                    if (ID == "7ea" || ID == "7eb" || ID == "5ea")
                    {
                        if (obj.RemoteFlag == 0)
                        {
                            var len = (byte) (obj.DataLen%9);
                            byte j;
                            var x = 0;
                            for (j = 0; j < len; j++)
                            {
                                str += " " + Convert.ToString(obj.Data[x], 16);
                                var mun = Convert.ToUInt32(Convert.ToString(obj.Data[x], 16), 16);
                                x++;
                                if (mun < 16)
                                {
                                    var zero = str.Insert(str.Length - 1, "0");
                                    str = zero;
                                }
                            }
                            str = str.Substring(1);
                            str = str.ToUpper();
                            if (str.Length != 23)
                            {
                                var dataLength = str.Length;
                                for (var i = 0; i < (23 - dataLength)/3; i++)
                                {
                                    str = str + " AA";
                                }
                            }

                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + ID.ToUpper() + ": " +
                                            str);
                           strarr[saveCount] = str;
                            saveCount++;
                            str = "";
                            dataCount++;
                        }
                    }
                }
                dataarr = new string[dataCount];
                for (var clearArrayNum = 0; clearArrayNum < dataCount; clearArrayNum++)
                {
                    dataarr[clearArrayNum] = strarr[clearArrayNum];
                }
            }
            return dataarr;
        }

        #region CAN型号

        private const int VCI_PCI5121 = 1;
        private const int VCI_PCI9810 = 2;
        private const int VCI_USBCAN1 = 3;
        private const int VCI_USBCAN2 = 4;
        private const int VCI_USBCAN2A = 4;
        private const int VCI_PCI9820 = 5;
        private const int VCI_CAN232 = 6;
        private const int VCI_PCI5110 = 7;
        private const int VCI_CANLITE = 8;
        private const int VCI_ISA9620 = 9;
        private const int VCI_ISA5420 = 10;
        private const int VCI_PC104CAN = 11;
        private const int VCI_CANETUDP = 12;
        private const int VCI_CANETE = 12;
        private const int VCI_DNP9810 = 13;
        private const int VCI_PCI9840 = 14;
        private const int VCI_PC104CAN2 = 15;
        private const int VCI_PCI9820I = 16;
        private const int VCI_CANETTCP = 17;
        private const int VCI_PEC9920 = 18;
        private const int VCI_PCI5010U = 19;
        private const int VCI_USBCAN_E_U = 20;
        private const int VCI_USBCAN_2E_U = 21;
        private const int VCI_PCI5020U = 22;
        private const int VCI_EG20T_CAN = 23;

        #endregion CAN型号

        #region 引用dll

        [DllImport("controlcan.dll")]
        private static extern uint VCI_OpenDevice(uint DeviceType, uint DeviceInd, uint Reserved);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_CloseDevice(uint DeviceType, uint DeviceInd);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_InitCAN(uint DeviceType, uint DeviceInd, uint CANInd,
            ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_ReadBoardInfo(uint DeviceType, uint DeviceInd, ref VCI_BOARD_INFO pInfo);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_ReadErrInfo(uint DeviceType, uint DeviceInd, uint CANInd,
            ref VCI_ERR_INFO pErrInfo);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_ReadCANStatus(uint DeviceType, uint DeviceInd, uint CANInd,
            ref VCI_CAN_STATUS pCANStatus);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_GetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType,
            ref byte pData);

        [DllImport("controlcan.dll")]
        private static extern unsafe uint VCI_SetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType,
            byte* pData);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_GetReceiveNum(uint DeviceType, uint DeviceInd, uint CANInd);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_ClearBuffer(uint DeviceType, uint DeviceInd, uint CANInd);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_StartCAN(uint DeviceType, uint DeviceInd, uint CANInd);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_ResetCAN(uint DeviceType, uint DeviceInd, uint CANInd);

        [DllImport("controlcan.dll")]
        private static extern uint VCI_Transmit(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_CAN_OBJ pSend,
            uint Len);

        //[DllImport("controlcan.dll")]
        //static extern uint VCI_Receive(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_CAN_OBJ pReceive, uint Len, int WaitTime);
        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        private static extern uint VCI_Receive(uint DeviceType, uint DeviceInd, uint CANInd, IntPtr pReceive, uint Len,
            int WaitTime);

        #endregion 引用dll
    }
}