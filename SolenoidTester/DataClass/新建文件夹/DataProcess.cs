using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

#region 委托

/// <summary>
/// 接收TCU故障码委托
/// </summary>
/// <param name="data"></param>
public delegate void RecvTcuFaultCodeEventHandler(string[] data);

/// <summary>
/// 接收TCU信息委托
/// </summary>
/// <param name="data"></param>
public delegate void RecvTcuInfoEventHandler(string[] data);

#endregion

namespace SolenoidTester
{
    public class DataProcess : Subject
    {
        #region 委托事件

        /// <summary>
        /// 接收TCU信息委托事件
        /// </summary>
        public event RecvTcuInfoEventHandler RecvTcuInfo;

        /// <summary>
        /// 接收TCU故障码委托事件
        /// </summary>
        public event RecvTcuFaultCodeEventHandler RcvTcuFaultCode;

        #endregion

        private DispatcherTimer Timer1 = new DispatcherTimer();

        #region 变量

        /// <summary>
        /// TCU数据采集指令发送计数
        /// </summary>
        private int collectOrderCount;

        /// <summary>
        /// 发送TCU数据采集指令标志位
        /// </summary>
        private bool sendCollectOeder = false;

/*        /// <summary>
        /// 发送TCU电磁阀调节指令标志位
        /// </summary>
        private bool sendTcuSolenoidOrder = false;*/

        /// <summary>
        /// 发送2703标志位
        /// </summary>
        private bool send2703 = false;

        /// <summary>
        /// 接收到67 03标志位
        /// </summary>
        private bool recv6703 = false;

        /*/// <summary>
        /// 接收到心跳标志位
        /// </summary>
        private bool sendHeartBeatOeder = false;*/

        /// <summary>
        /// CAN控制模块对象
        /// </summary>
        private CANControl CAN = new CANControl();

        /// <summary>
        /// 结构体更新线程
        /// </summary>
        private Thread thr_RecvData;

        /// <summary>
        /// 观察者对象列表
        /// </summary>
        private readonly ArrayList observers;

        /// <summary>
        /// TCU指令列表
        /// </summary>
        private ArrayList TCUOrderList;

        /// <summary>
        /// 下位机指令列表
        /// </summary>
        private ArrayList TableOrderList;

        /// <summary>
        /// C1234/C456指令存储
        /// </summary>
        private volatile string C1234C456Order;

        /// <summary>
        /// C35R/CB26指令存储
        /// </summary>
        private volatile string C35RCB26Order;

        /// <summary>
        /// TCC/EPC指令存储
        /// </summary>
        private volatile string TCCEPCOrder;

        /*/// <summary>
        /// 开关电磁阀指令存储
        /// </summary>
        private string switchSolenoidOrder;*/

        /// <summary>
        /// 换档电磁阀指令存储
        /// </summary>
        private volatile string shiftSolenoidOrder;

        /// <summary>
        /// 发送C1234/C456指令标志位
        /// </summary>
        private volatile bool sendC1234C456Order = false;

        /// <summary>
        /// 发送C35R/CB26指令标志位
        /// </summary>
        private volatile bool sendC35RCB26Order = false;

        /// <summary>
        /// 发送TCC/EPC指令标志位
        /// </summary>
        private volatile bool sendTCCEPCOrder = false;

        /*/// <summary>
        /// 发送开关电磁阀指令标志位
        /// </summary>
        private bool sendSwitchSolenoidOrder = false;*/

        /// <summary>
        /// 发送换档电磁阀指令标志位
        /// </summary>
        private volatile bool sendShiftSolenoidOrder = false;

        /// <summary>
        /// TCC调节值
        /// </summary>
        private int TCCvalue;

        /// <summary>
        /// EPC调节值
        /// </summary>
        private int EPCvalue;

        /// <summary>
        /// 发送TCU指令标志位
        /// </summary>
        private bool sendTCUOrder = true;

        /// <summary>
        /// 发送下位机指令标志位
        /// </summary>
        private bool sendTableOrder = true;

        /// <summary>
        /// 心跳间隔
        /// </summary>
        private DateTime TCUHeartBeatInterval = DateTime.Now;

        /// <summary>
        /// TCC/EPC指令间隔
        /// </summary>
        private DateTime TCCEPCInterval = DateTime.Now;

        /// <summary>
        /// 发送VIN修改信息
        /// </summary>
        private string[] sendVinCode = new string[3];

        /// <summary>
        /// 刷写VIN标志位
        /// </summary>
        private bool flashVinCode = false;

        /// <summary>
        /// 发送长报文计数
        /// </summary>
        private int sendLongMsgCount = 0;

        /// <summary>
        /// 发送第一条长报文
        /// </summary>
        private bool sendFirstLongMsg = true;

        /// <summary>
        /// 允许发送长报文
        /// </summary>
        private bool allowSengLongMsg = false;

        /// <summary>
        /// 查询TCU信息标志位
        /// </summary>
        private bool inquireTcuInfo = false;

        /// <summary>
        /// 查询TCU信息计数
        /// </summary>
        private int inquireTcuInfoCount = 0;

        /// <summary>
        /// TCU信息查询指令
        /// </summary>
        private string[] InquireTCUInfoOrder = new string[]
        {
            "02 1A CB 00 00 00 00 00", "02 1A CC 00 00 00 00 00", "02 1A A0 00 00 00 00 00",
            "02 1A B4 00 00 00 00 00", "30 00 04 00 00 00 00 00", "RcvLongMsg",
            "02 1A 90 00 00 00 00 00", "30 00 05 00 00 00 00 00", "RcvLongMsg", "RcvLongMsg", "RcvLongMsg", "RcvLongMsg"
        };

        /// <summary>
        /// TCU信息
        /// </summary>
        private string[] TCUInfo;

        /// <summary>
        /// 接收到MTC第一个报文
        /// </summary>
        private bool recvMTCFirstMsg = false;

        /// <summary>
        /// 接收到VIN第一个报文
        /// </summary>
        private bool recvVINFirst = false;

        /// <summary>
        /// TCU循环查询结果存放缓冲区
        /// </summary>
        private string[] tcuTempBuffer = new string[5] {"null", "null", "null", "null", "null"};

        /// <summary>
        /// TCU缓冲区1
        /// </summary>
        private string[] tcuBuffer1 = new string[5] {"null", "null", "null", "null", "null"};

        /// <summary>
        /// TCU缓冲区2
        /// </summary>
        private string[] tcuBuffer2 = new string[5] {"null", "null", "null", "null", "null"};

        /// <summary>
        /// TCU循环采集数据翻转标志位
        /// </summary>
        private bool tcuRcvFlagFlip = false;

        /// <summary>
        /// 实时数据更新数组
        /// </summary>
        private string[,] updateData = new string[5, 32];

        /// <summary>
        /// 实时数据更新计数
        /// </summary>
        private int updateDataNum = 0;

        /// <summary>
        /// 故障码
        /// </summary>
        private string[] faultCode = new string[1] {""};

        /// <summary>
        /// 无故障码标志位
        /// </summary>
        private bool noFaultCode = false;

        /// <summary>
        /// 查询故障码标志位
        /// </summary>
        private bool getFaultCode = false;

        /// <summary>
        /// 清空故障码标志位
        /// </summary>
        private bool clearFaultCode = false;

        /// <summary>
        /// 设备标定标志位
        /// </summary>
        private volatile bool deviceDemarcate = false;

        /// <summary>
        /// 设备标定指令
        /// </summary>
        private string[] deviceDemarcateOrder;

        /// <summary>
        /// 设备标定指令发送时间间隔
        /// </summary>
        private DateTime deviceDemarcateInterval;

        /// <summary>
        /// 压力开关电磁阀调节标志位
        /// </summary>
        private volatile bool switchSolenoidControl = false;

        /// <summary>
        /// 压力开关电磁阀调节指令
        /// </summary>
        private string switchSolenoidControlOrder = "";

        /// <summary>
        /// 油泵开关控制标志位
        /// </summary>
        private volatile bool oilPumpControl = false;

        /// <summary>
        /// 油泵开关控制指令
        /// </summary>
        private volatile string oilPumpControlOrder = "";

        /// <summary>
        /// 电热丝加热开关控制标志位
        /// </summary>
        private volatile bool heatingWireControl = false;

        /// <summary>
        /// 电热丝加热开关控制指令
        /// </summary>
        private string heatingWireControlOrder = "";

        /// <summary>
        /// TCU供电开关控制标志位
        /// </summary>
        private volatile bool tcuPowerControl = false;

        /// <summary>
        /// TCU供电开关控制指令
        /// </summary>
        private string tcuPowerControlOrder = "";

        /// <summary>
        /// 开始/停止采集下位机数据标志位
        /// </summary>
        private volatile bool collectMcuData = false;

        /// <summary>
        /// 开始/停止采集下位机数据指令
        /// </summary>
        private string collectMcuDataOrder = "";

        /// <summary>
        /// 禁用旋钮标志位
        /// </summary>
        private volatile bool disableKnob = false;

        /// <summary>
        /// 禁用旋钮指令
        /// </summary>
        private string disableKnobOrder = "";

        /// <summary>
        /// 输入转速调节标志位
        /// </summary>
        private volatile bool adjustInputSpeed = false;

        /// <summary>
        /// 输入转速调节指令
        /// </summary>
        private volatile string inputSpeedOrder = "";

        /// <summary>
        /// 输出转速调节标志位
        /// </summary>
        private volatile bool adjustOutputSpeed = false;

        /// <summary>
        /// 输出转速调节指令
        /// </summary>
        private volatile string outputSpeedOrder = "";

        /// <summary>
        /// 下位机指令下发间隔
        /// </summary>
        private DateTime tableOrderInterval = DateTime.Now;

        /// <summary>
        /// 软件调节参数记录数组
        /// </summary>
        private volatile string[] liveUpdateData = new string[8];

        /// <summary>
        /// 电磁阀调节值赋值标志位
        /// </summary>
        private bool adjustSolenoid = false;

        /// <summary>
        /// 转速调节值赋值标志位
        /// </summary>
        private bool adjustSpeed = false;

        /// <summary>
        /// 电磁阀调节值
        /// </summary>
        private string[] adjustOrderValue = new string[6] {"", "", "", "", "", "" };

        #endregion

        /// <summary>
        /// 将需要标定的值转换成指令
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        private void DiffDeviceDemarcateValue(IReadOnlyList<string> str1, IReadOnlyList<string> str2)
        {
            var diffStr = new string[] {};
            var diffCount = 0;
            for (var i = 0; i < 22; i++)
            {
                if (str1[i] != str2[i])
                {
                    diffStr[diffCount] = "02 " + ((i + 2)/2).ToString("X2") + " ";
                    if (i%2 == 0)
                    {
                        if (Convert.ToInt32(str2[i]) >= 0)
                        {
                            diffStr[diffCount] = diffStr[diffCount] + "00 ff " +
                                                 (Convert.ToInt32(str2[i])/256).ToString("X2") + " " +
                                                 (Convert.ToInt32(str2[i])%256).ToString("X2");
                        }
                        else
                        {
                            diffStr[diffCount] = diffStr[diffCount] + "01 ff " +
                                                 (Convert.ToInt32(str2[i])/256).ToString("X2") + " " +
                                                 (Convert.ToInt32(str2[i])%256).ToString("X2");
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(str2[i]) >= 0)
                        {
                            diffStr[diffCount] = diffStr[diffCount] + "02 ff " +
                                                 (Convert.ToInt32(str2[i])/256).ToString("X2") + " " +
                                                 (Convert.ToInt32(str2[i])%256).ToString("X2");
                        }
                        else
                        {
                            diffStr[diffCount] = diffStr[diffCount] + "03 ff " +
                                                 (Convert.ToInt32(str2[i])/256).ToString("X2") + " " +
                                                 (Convert.ToInt32(str2[i])%256).ToString("X2");
                        }
                    }
                    diffCount++;
                }
                DeviceDemarcate(diffStr);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataProcess()
        {
            observers = new ArrayList();

            TCUOrderList = new ArrayList();

            TableOrderList = new ArrayList();
            //实时更新数据初始化

            liveUpdateData[0] = "false";
            liveUpdateData[1] = "null";
            liveUpdateData[2] = "null";
            liveUpdateData[3] = "null";
            liveUpdateData[4] = "null";
            liveUpdateData[5] = "null";
            liveUpdateData[6] = "0";
            liveUpdateData[7] = "0";
            
            TCCvalue = 0;
            EPCvalue = 0;
        }

        /// <summary>
        /// 采集测试台架数据
        /// </summary>
        private void DataProcessThread()
        {
            var mcuData = new string[19];
            var mcuDataCount = 0;
            var collocetMcuData = false;
            var realData = new string[8] {"", "", "", "", "", "", "", ""};
            while (true)
            {
                #region 数据接收部分

                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN接收数据开始");
                var data = CAN.RecviveData();
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN接收数据结束");


                if (data.Length != 0)
                {
                    foreach (string t in data)
                    {
                        realData = t.Split(' ');

                        //TCU数据采集指令响应
                        if (realData[1] == "62")
                        {
                            sendTCUOrder = true;
                            sendCollectOeder = true;

                            #region 存储TCU采集数据

                            if (realData[2] + realData[3] == "1941")
                            {
                                tcuTempBuffer[0] =
                                    Convert.ToString(
                                        (Convert.ToInt32(
                                            realData[realData.Length - 4] + realData[realData.Length - 3],
                                            16)/
                                         8));
                                Debug.WriteLine("                                  输入转速：" + tcuTempBuffer[0]);
                            }
                            if (realData[2] + realData[3] == "1942")
                            {
                                tcuTempBuffer[1] =
                                    Convert.ToString(
                                        (Convert.ToInt32(
                                            realData[realData.Length - 4] + realData[realData.Length - 3],
                                            16)/
                                         8));
                                Debug.WriteLine("                                  输入转速：" + tcuTempBuffer[1]);
                            }
                            if (realData[2] + realData[3] == "194F")
                            {
                                if (Convert.ToInt32(realData[4], 16) - 0 == 0)
                                    tcuTempBuffer[2] = "无效档";
                                if (Convert.ToInt32(realData[4], 16) - 1 == 0)
                                    tcuTempBuffer[2] = "P档";
                                if (Convert.ToInt32(realData[4], 16) - 2 == 0)
                                    tcuTempBuffer[2] = "R档";
                                if (Convert.ToInt32(realData[4], 16) - 4 == 0)
                                    tcuTempBuffer[2] = "N档";
                                if (Convert.ToInt32(realData[4], 16) - 6 == 0)
                                    tcuTempBuffer[2] = "D档";
                                Debug.WriteLine("                                  档位：" + tcuTempBuffer[2]);
                            }
                            if (realData[2] + realData[3] == "280D")
                            {
                                tcuTempBuffer[3] = Convert.ToString((Convert.ToInt32(realData[4], 16) - 40));
                                Debug.WriteLine("                                  TCM温度：" + tcuTempBuffer[3]);
                            }
                            if (realData[2] + realData[3] == "281D")
                            {
                                var str = Convert.ToString(Convert.ToInt32(realData[4], 16), 2);
                                for (var j = str.Length; j < 8; j++)
                                {
                                    str = str.Insert(0, "0");
                                }
                                tcuTempBuffer[4] = str;
                                Debug.WriteLine("                                  压力开关状态：" + tcuTempBuffer[4]);
                                //将TCU采集的一组数存到TCU其中一个缓冲区中
                                if (!tcuRcvFlagFlip)
                                {
                                    var k = 0;
                                    for (var j = 0; j < 5; j++)
                                    {
                                        if (tcuTempBuffer[j] != null && tcuTempBuffer[j] != "")
                                        {
                                            tcuBuffer1[j] = tcuTempBuffer[j];
                                            k++;
                                        }
                                        if (k != 5) continue;
                                        tcuRcvFlagFlip = true;
                                        j = 4;
                                    }
                                }
                                else if (tcuRcvFlagFlip)
                                {
                                    var k = 0;
                                    for (var j = 0; j < 5; j++)
                                    {
                                        if (tcuTempBuffer[j] != null && tcuTempBuffer[j] != "")
                                        {
                                            tcuBuffer2[j] = tcuTempBuffer[j];
                                            k++;
                                        }
                                        if (k != 5) continue;
                                        tcuRcvFlagFlip = false;
                                        j = 4;
                                    }
                                }
                            }

                            #endregion
                        }
                        //电磁阀调节指令响应
                        if (realData[1] == "EE" || realData[1] == "7F")
                        {
                            sendTCUOrder = true;
                            sendCollectOeder = true;
                        }
                        //发送2703响应
                        if (realData[1] == "67")
                        {
                            recv6703 = true;
                            sendTCUOrder = true;
                            sendCollectOeder = true;
                        }
                        //发送心跳响应
                        if (realData[1] == "7E")
                        {
                            sendTCUOrder = true;
                            sendCollectOeder = true;
                        }
                        //产生0160
                        if (realData[0] + realData[1] == "0160")
                        {
                            send2703 = false;
                            recv6703 = false;
                        }
                        //允许发送长报文响应
                        if (realData[0] + realData[1] == "3000")
                        {
                            sendTCUOrder = true;
                            allowSengLongMsg = true;
                        }
                        //刷写VIN成功响应
                        if (realData[1] == "7B")
                        {
                            sendTCUOrder = true;
                        }

                        #region 查询TCU信息响应内容

                        //查询基础零件号响应
                        if (realData[1] + realData[2] == "5ACC")
                        {
                            sendTCUOrder = true;
                            TCUInfo[0] =
                                Convert.ToUInt32(realData[3] + realData[4] + realData[5] + realData[6], 16)
                                    .ToString();
                        }
                        //查询终端零件号响应
                        if (realData[1] + realData[2] == "5ACB")
                        {
                            sendTCUOrder = true;
                            TCUInfo[1] =
                                Convert.ToUInt32(realData[3] + realData[4] + realData[5] + realData[6], 16)
                                    .ToString();
                        }
                        //查询商用状态响应
                        if (realData[1] + realData[2] == "5AA0")
                        {
                            sendTCUOrder = true;
                            if (realData[3] == "00")
                            {
                                TCUInfo[2] = "商用状态安全";
                            }
                            else
                            {
                                TCUInfo[2] = "厂家状态未加密";
                            }
                        }

                        //查询TCU信息长报文
                        //查询制造可追踪码响应
                        if (realData[2] + realData[3] == "5AB4")
                        {
                            sendTCUOrder = true;
                            recvMTCFirstMsg = true;
                            TCUInfo[3] = ((char) (Convert.ToUInt16(realData[4], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(realData[5], 16))).ToString()
                                         + ((char) (Convert.ToUInt16(realData[6], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(realData[7], 16))).ToString();

                            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[3]);
                        }

                        //查询VIN响应
                        if (realData[2] + realData[3] == "5A90")
                        {
                            sendTCUOrder = true;
                            recvVINFirst = true;
                            TCUInfo[4] = ((char) (Convert.ToUInt16(realData[4], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(realData[5], 16))).ToString()
                                         + ((char) (Convert.ToUInt16(realData[6], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(realData[7], 16))).ToString();
                            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[4]);
                        }
                        if (realData[0] == "21")
                        {
                            if (recvMTCFirstMsg)
                            {
                                sendTCUOrder = true;
                                TCUInfo[3] = TCUInfo[3] + ((char) (Convert.ToUInt16(realData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(realData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[5], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(realData[6], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[7], 16))).ToString();
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[3]);
                            }
                            if (recvVINFirst)
                            {
                                sendTCUOrder = true;
                                TCUInfo[4] = TCUInfo[4] + ((char) (Convert.ToUInt16(realData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(realData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[5], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(realData[6], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[7], 16))).ToString();
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[4]);
                            }
                        }
                        if (realData[0] == "22")
                        {
                            if (recvMTCFirstMsg)
                            {
                                sendTCUOrder = true;
                                TCUInfo[3] = TCUInfo[3] + ((char) (Convert.ToUInt16(realData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(realData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[5], 16))).ToString();
                                recvMTCFirstMsg = false;
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[3]);
                            }
                            if (recvVINFirst)
                            {
                                sendTCUOrder = true;
                                inquireTcuInfoCount = 0;
                                inquireTcuInfo = false;
                                TCUInfo[4] = TCUInfo[4] +
                                             ((char) (Convert.ToUInt16(realData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(realData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(realData[5], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(realData[6], 16))).ToString();
                                RecvTcuInfo(TCUInfo);
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[4]);
                                recvVINFirst = false;
                            }

                            #endregion
                        }
                        //查询故障码响应
                        if (realData[0] == "81")
                        {
                            if (realData[1] + realData[2] != "0000")
                            {
                                faultCode[0] = faultCode[0] + "P" + realData[1] + realData[2];
                            }
                            else
                            {
                                if (noFaultCode)
                                {
                                    faultCode[0] = "无故障码";
                                }
                                sendTCUOrder = true;
                                RcvTcuFaultCode(faultCode);
                            }
                            noFaultCode = false;
                        }
                        //清空故障码响应
                        if (realData[0] + realData[1] == "0144")
                        {
                            //故障码已清除
                        }
                        if (realData[0] == "00")
                        {
                            if (realData[1] == "00")
                            {
                                collocetMcuData = true;
                                mcuDataCount = 0;
                            }
                            //处理下位机采集数据
                            if (collocetMcuData)
                            {
                                for (var tableDataNum = 0; tableDataNum < 3; tableDataNum++)
                                {
                                    var intdata =
                                        Convert.ToInt32(
                                            realData[tableDataNum*2 + 3] + realData[tableDataNum*2 + 2], 16);
                                    mcuData[mcuDataCount] = intdata.ToString();
                                    //处理开关状态
                                    if (mcuDataCount == 11)
                                    {
                                        var switchStatus = Convert.ToString(Convert.ToInt32(mcuData[mcuDataCount]),
                                            2);

                                        if (switchStatus.Length < 8)
                                        {
                                            for (int i = switchStatus.Length; i < 8; i++)
                                            {
                                                switchStatus = switchStatus.Insert(0, "0");
                                            }
                                        }
                                        mcuData[mcuDataCount] = switchStatus.Substring(7, 1);
                                        mcuDataCount++;
                                        mcuData[mcuDataCount] = switchStatus.Substring(6, 1);
                                        mcuDataCount++;
                                        mcuData[mcuDataCount] = switchStatus.Substring(5, 1);
                                        mcuDataCount++;
                                        mcuData[mcuDataCount] = switchStatus.Substring(4, 1);
                                        mcuDataCount++;
                                        mcuData[mcuDataCount] = switchStatus.Substring(3, 1);
                                        mcuDataCount++;
                                        mcuData[mcuDataCount] = switchStatus.Substring(2, 1);
                                        mcuDataCount++;
                                        mcuData[mcuDataCount] = switchStatus.Substring(1, 1);
                                        mcuDataCount++;
                                        mcuData[mcuDataCount] = switchStatus.Substring(0, 1);
                                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                        "                           接收到一组数");
                                    }
                                    mcuDataCount++;
                                    if (mcuDataCount == 19)
                                    {
                                        collocetMcuData = false;
                                        mcuDataCount = 0;
                                        //将下位机数据存到更新数据数组中
                                        for (int saveTableDataNum = 0; saveTableDataNum < 19; saveTableDataNum++)
                                        {
                                            updateData[updateDataNum, saveTableDataNum] = mcuData[saveTableDataNum];
                                        }
                                        for (int saveTcuDataNum = 19; saveTcuDataNum < 24; saveTcuDataNum++)
                                        {
                                            updateData[updateDataNum, saveTcuDataNum] = tcuRcvFlagFlip
                                                ? tcuBuffer1[saveTcuDataNum - 19]
                                                : tcuBuffer2[saveTcuDataNum - 19];
                                        }
                                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                        "                线程读取参数");
                                        updateData[updateDataNum, 24] = liveUpdateData[0];
                                        updateData[updateDataNum, 25] = liveUpdateData[1];
                                        updateData[updateDataNum, 26] = liveUpdateData[2];
                                        updateData[updateDataNum, 27] = liveUpdateData[3];
                                        updateData[updateDataNum, 28] = liveUpdateData[4];
                                        updateData[updateDataNum, 29] = liveUpdateData[5];
                                        updateData[updateDataNum, 30] = liveUpdateData[6];
                                        updateData[updateDataNum, 31] = liveUpdateData[7];
                                        updateDataNum++;
                                        if (updateDataNum == 5)
                                        {
                                            updateDataNum = 0;
                                            NotifyObservers();
                                            //清空数据更新数组里的数据
                                            /*for (int i = 0; i < 5; i++)
                                            {
                                                for (int j = 0; j < 32; j++)
                                                {
                                                    updateData[i, j] = "";
                                                }
                                            }*/
                                        }
                                        //清空下位机采集数组里的数据
                                        for (var b = 0; b < 19; b++)
                                        {
                                            mcuData[b] = "";
                                        }
                                    }
                                }
                            }
                        }

                        for (var a = 0; a < realData.Length; a++)
                        {
                            realData[a] = "";
                        }
                    }
                }

                #endregion

                #region 数据处理部分

                #region 将需要发送给TCU的指令放到List中

                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                线程处理数据并发送开始");
                //发送2703
                if (!send2703)
                {
                    send2703 = true;
                    sendCollectOeder = false;
                    TCUOrderList.Add("02 27 03");
                }

                if (DateTime.Now > TCCEPCInterval && recv6703)
                {
                    sendCollectOeder = false;
                    TCCEPCInterval = DateTime.Now.AddMilliseconds(97);
                    AdjustTCCEPCPressure(TCCvalue, EPCvalue);
                }
                //TCU心跳(3秒)
                if (DateTime.Now > TCUHeartBeatInterval && recv6703)
                {
                    sendCollectOeder = false;
                    TCUHeartBeatInterval = DateTime.Now.AddMilliseconds(2500);
                    TCUOrderList.Add("01 3E");
                }
                //刷写VIN
                if (flashVinCode && sendLongMsgCount == 2)
                {
                    sendTCUOrder = true;
                }
                if (flashVinCode && sendTCUOrder)
                {
                    sendCollectOeder = false;
                    if (sendFirstLongMsg)
                    {
                        sendTCUOrder = false;
                        sendFirstLongMsg = false;
                        CAN.SendData("7E2", sendVinCode[sendLongMsgCount]);
                        sendLongMsgCount++;
                    }
                    if (allowSengLongMsg)
                    {
                        sendTCUOrder = false;
                        CAN.SendData("7E2", sendVinCode[sendLongMsgCount]);
                        sendLongMsgCount++;
                    }

                    if (sendLongMsgCount == 3)
                    {
                        sendLongMsgCount = 0;
                        flashVinCode = false;
                        sendFirstLongMsg = true;
                        sendCollectOeder = true;
                    }
                }
                //查询TCU信息
                if (inquireTcuInfo && sendTCUOrder)
                {
                    sendTCUOrder = false;
                    sendCollectOeder = false;
                    if (InquireTCUInfoOrder[inquireTcuInfoCount] == "RcvLongMsg")
                    {
                    }
                    else
                    {
                        CAN.SendData("7E2", InquireTCUInfoOrder[inquireTcuInfoCount]);
                    }
                    inquireTcuInfoCount++;
                    if (inquireTcuInfoCount == 13)
                    {
                        inquireTcuInfoCount = 0;
                        inquireTcuInfo = false;
                        sendCollectOeder = true;
                    }
                }
                //查询故障码
                if (getFaultCode && sendTCUOrder)
                {
                    //sendCollectOeder = false;
                    TCUOrderList.Add("03 A9 81 5A");
                    getFaultCode = false;
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                查询故障码");
                }
                //清除故障码
                if (clearFaultCode && sendTCUOrder)
                {
                    sendTCUOrder = false;
                    //sendCollectOeder = false;
                    clearFaultCode = false;
                    CAN.SendData("101", "FE 01 04 00 00 00 00 00");
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                清空故障码");
                }

                #region TCU指令赋值

                if (sendC1234C456Order)
                {
                    sendCollectOeder = false;
                    TCUOrderList.Add(C1234C456Order);
                    sendC1234C456Order = false;
                }

                if (sendC35RCB26Order)
                {
                    //sendCollectOeder = false;
                    TCUOrderList.Add(C35RCB26Order);
                    sendC35RCB26Order = false;
                }
                if (sendTCCEPCOrder)
                {
                    //sendCollectOeder = false;
                    TCUOrderList.Add(TCCEPCOrder);
                    sendTCCEPCOrder = false;
                }
                if (sendShiftSolenoidOrder)
                {
                    //sendCollectOeder = false;
                    TCUOrderList.Add(shiftSolenoidOrder);
                    sendShiftSolenoidOrder = false;
                }
                if (recv6703 && sendCollectOeder && sendTCUOrder)
                {
                    sendCollectOeder = false;
                    for (int i = 0; i < TCUOrderList.Count; i++)
                    {
                        if (TCUOrderList[i].ToString().Substring(0, 5) == "03 22")
                        {
                            sendCollectOeder = true;
                        }
                    }
                    if (!sendCollectOeder)
                    {
                        TCUOrderList.Add(SendCollectOrder());
                    }
                }

                #endregion

                #region 台架指令赋值

                if (deviceDemarcate)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    foreach (var t in deviceDemarcateOrder)
                    {
                        TableOrderList.Add(t);
                    }
                }

                if (adjustInputSpeed)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(inputSpeedOrder);
                    adjustInputSpeed = false;
                }

                if (adjustOutputSpeed)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(outputSpeedOrder);
                    adjustOutputSpeed = false;
                }

                if (collectMcuData)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(collectMcuDataOrder);
                    collectMcuData = false;
                }

                if (disableKnob)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(disableKnobOrder);
                    disableKnob = false;
                }

                if (heatingWireControl)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(heatingWireControlOrder);
                    heatingWireControl = false;
                }

                if (oilPumpControl)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(oilPumpControlOrder);
                    oilPumpControl = false;
                }

                if (switchSolenoidControl)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(switchSolenoidControlOrder);
                    switchSolenoidControl = false;
                }

                if (tcuPowerControl)
                {
                    sendTableOrder = true;
                    //sendCollectOeder = false;
                    TableOrderList.Add(tcuPowerControlOrder);
                    tcuPowerControl = false;
                }

                #endregion

                #endregion

                //发送下位机指令List中的指令
                if (sendTableOrder && sendTCUOrder && !flashVinCode && !inquireTcuInfo && !clearFaultCode)
                {
                    if (deviceDemarcate)
                    {
                        if (DateTime.Now > deviceDemarcateInterval)
                        {
                            if (TableOrderList.Count > 0)
                            {
                                deviceDemarcateInterval = DateTime.Now.AddMilliseconds(20);
                                CAN.SendData("7EC", TableOrderList[0].ToString());
                                sendTableOrderCount++;
                                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                "                           向下位机发送标定指令：" + sendTableOrderCount);
                                TableOrderList.RemoveAt(0);
                            }
                            else
                            {
                                deviceDemarcate = false;
                                sendTableOrderCount = 0;
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now > tableOrderInterval)
                        {
                            if (TableOrderList.Count > 0)
                            {
                                CAN.SendData("7EC", TableOrderList[0].ToString());
                                if (adjustInputSpeed||adjustSpeed)
                                {
                                    liveUpdateData[6] = adjustOrderValue[4];
                                    adjustOrderValue[4]="null";
                                    adjustSpeed = false;
                                }
                                if (adjustOutputSpeed || adjustSpeed)
                                {
                                    liveUpdateData[7] = adjustOrderValue[5];
                                    adjustOrderValue[5]="null";
                                    adjustSpeed = false;
                                }
                                tableOrderInterval = DateTime.Now.AddMilliseconds(10);
                                TableOrderList.RemoveAt(0);
                            }
                            else
                            {
                                sendTableOrder = false;
                            }
                        }
                    }
                }

                //发送TCU指令List中的指令
                if (sendTCUOrder && !flashVinCode && !inquireTcuInfo && !clearFaultCode && !sendTableOrder)
                {
                    if (TCUOrderList.Count > 0)
                    {
                        sendTCUOrder = false;

                        CAN.SendData("7E2", TCUOrderList[0].ToString());
                        //将电磁阀调节值更新到数据库中
                        if (TCUOrderList[0].ToString().Substring(0, 5) == "07 AE" && adjustSolenoid)
                        {
                            for (var i = 0; i < 2; i++)
                            {
                                liveUpdateData[i + 2] = adjustOrderValue[i];
                                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                油压指令："+ adjustOrderValue[i]);
                                adjustOrderValue[i]="null";
                            }
                            adjustSolenoid = false;
                        }
                        TCUOrderList.RemoveAt(0);
                    }
                }


                /*//发送下位机指令List中的指令
                if (deviceDemarcate && DateTime.Now > deviceDemarcateInterval)
                {
                    if (TableOrderList.Count > 0)
                    {
                        deviceDemarcateInterval = DateTime.Now.AddMilliseconds(20);
                        CAN.SendData("7EC", TableOrderList[0].ToString());
                        sendTableOrderCount++;
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           向下位机发送标定指令："+ sendTableOrderCount);
                        TableOrderList.RemoveAt(0);
                    }
                    else
                    {
                        deviceDemarcate = false;
                        sendTableOrderCount = 0;
                    }
                }*/
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                线程处理数据并发送结束");

                #endregion
            }
        }

        /// <summary>
        /// 向下位机发送标定指令计数
        /// </summary>
        private int sendTableOrderCount = 0;

        /// <summary>
        /// TCU数据采集指令
        /// </summary>
        /// <returns>指令</returns>
        private string SendCollectOrder()
        {
            string data = "";
            if (collectOrderCount == 0)
            {
                data = "03 22 19 41";
            }
            if (collectOrderCount == 1)
            {
                data = "03 22 19 42";
            }
            if (collectOrderCount == 2)
            {
                data = "03 22 19 4F";
            }
            if (collectOrderCount == 3)
            {
                data = "03 22 28 0D";
            }
            if (collectOrderCount == 4)
            {
                data = "03 22 28 1D";
            }
            collectOrderCount++;
            if (collectOrderCount == 5)
            {
                collectOrderCount = 0;
            }
            return data;
        }

        #region 外部接口

        /// <summary>
        /// 连接CAN卡
        /// </summary>
        /// <returns></returns>
        public bool ConnectCan()
        {
            CAN.CanConnectStats += new CanConnectStatsEventHandler(GetCanConnectStatus);
            if (CAN.ConncetCan())
            {
                thr_RecvData = new Thread(DataProcessThread) {IsBackground = true};
                thr_RecvData.Priority = ThreadPriority.Highest;
                thr_RecvData.Start();
            }
            return Convert.ToBoolean(liveUpdateData[0]);
        }

        /// <summary>
        /// 获取CAN连接状态
        /// </summary>
        /// <param name="status">连接状态</param>
        private void GetCanConnectStatus(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入TCU连接状态");
            liveUpdateData[0] = Convert.ToString(status);
        }

        /// <summary>
        /// 断开CAN
        /// </summary>
        public void DisConnectCan()
        {
            if (thr_RecvData != null)
            {
                if (CAN != null)
                {
                    CAN.SendData("7EC", "03 04 00");
                    CAN.DisConnectCan();
                    CAN.CanConnectStats -= new CanConnectStatsEventHandler(GetCanConnectStatus);
                    CAN = null;
                }
                thr_RecvData.Abort();
                thr_RecvData = null;
            }
        }

        /// <summary>
        /// 记录测试项目编号
        /// </summary>
        /// <param name="number">编号</param>
        public void TextItemNumber(string number)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入测试编号接口");
            liveUpdateData[1] = number;
        }

        #region 下位机接口

        /// <summary>
        /// 压力开关电磁阀调节
        /// </summary>
        /// <param name="status1">开关1状态</param>
        /// <param name="status2">开关2状态</param>
        /// <param name="status3">开关3状态</param>
        /// <param name="status4">开关4状态</param>
        public void SwitchSolenoidControl(bool status1, bool status2, bool status3, bool status4)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                调节压力开关接口");
            var switch1 = status1 ? "1" : "0";
            var switch2 = status2 ? "1" : "0";
            var switch3 = status3 ? "1" : "0";
            var switch4 = status4 ? "1" : "0";
            switchSolenoidControlOrder = "03 00 " + $"{Convert.ToInt32(switch4 + switch3 + switch2 + switch1, 2):x2}";
            switchSolenoidControl = true;
        }

        /// <summary>
        /// 油泵开关控制
        /// </summary>
        /// <param name="status">开关状态</param>
        public void OilPumpControl(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                控制油泵接口");
            oilPumpControlOrder = status ? "03 01 ff" : "03 01 00";
            oilPumpControl = true;
        }


        /// <summary>
        /// 电热丝加热开关控制
        /// </summary>
        /// <param name="status">电热丝开关</param>
        public void HeatingWireControl(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                控制电热丝接口");
            heatingWireControlOrder = status ? "03 02 ff" : "03 02 00";
            heatingWireControl = true;
        }

        /// <summary>
        /// TCU供电开关控制
        /// </summary>
        /// <param name="status">开关状态</param>
        public void TCUPowerControl(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                控制TCU供电接口");
            tcuPowerControlOrder = status ? "03 03 ff" : "03 03 00";
            tcuPowerControl = true;
        }


        /// <summary>
        /// 开始/停止采集下位机数据
        /// </summary>
        /// <param name="status">true:开始 false:停止</param>
        public void CollectMcuData(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                采集下位机数据接口");
            collectMcuDataOrder = status ? "03 04 FF" : "03 04 00";
            collectMcuData = true;
        }

        /// <summary>
        /// 禁用旋钮
        /// </summary>
        /// <param name="status">true:禁用 false:启用</param>
        public void DisableKnob(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                禁用旋钮接口");
            disableKnobOrder = status ? "03 05 ff" : "03 05 00";
            disableKnob = true;
        }

        /// <summary>
        /// 记录并调节输入转速
        /// </summary>
        /// <param name="speed">输入转速值</param>
        public void AdjustInputSpeed(int speed)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                调节输入转速接口");
            //liveUpdateData[6] = Convert.ToString(speed);
            adjustOrderValue[4] = Convert.ToString(speed);
            inputSpeedOrder = "03 06 " + (speed%256).ToString("X2") + " " + (speed/256).ToString("X2");
            adjustInputSpeed = true;
            adjustSpeed = true;
        }


        /// <summary>
        /// 记录并调节输出转速
        /// </summary>
        /// <param name="speed">输出转速值</param>
        public void AdjustOutputSpeed(int speed)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                调节输出转速接口");
            //liveUpdateData[7] = Convert.ToString(speed);
            adjustOrderValue[5] = Convert.ToString(speed);
            outputSpeedOrder = "03 07 " + (speed%256).ToString("X2") + " " + (speed/256).ToString("X2");
            adjustOutputSpeed = true;
            adjustSpeed = true;
        }

        /// <summary>
        /// 设备标定
        /// </summary>
        /// <param name="data">标定值</param>
        public void DeviceDemarcate(string[] data)
        {
            deviceDemarcateOrder = data;
            foreach (var t in data)
            {
                TableOrderList.Add(t);
            }
            deviceDemarcateInterval = DateTime.Now;
            deviceDemarcate = true;
        }

        #endregion

        #region TCU接口

        /// <summary>
        /// 调节C1234/C456电磁阀油压
        /// </summary>
        /// <param name="data1">C1234油压</param>
        /// <param name="data2">C456油压</param>
        public void AdjustC1234C456Pressure(int data1, int data2)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                调节C1234C456接口");
            var b4 = Convert.ToString(data1*16, 16);
            var b6 = Convert.ToString(data2*16, 16);
            for (var i = b4.Length; i < 4; i++)
            {
                b4 = b4.Insert(0, "0");
            }
            for (var i = b6.Length; i < 4; i++)
            {
                b6 = b6.Insert(0, "0");
            }
            var b5 = b4.Substring(2, 2);
            b4 = b4.Substring(0, 2);
            var b7 = b6.Substring(2, 2);
            b6 = b6.Substring(0, 2);
            C1234C456Order = "07 AE 3A C0 " + b4 + " " + b5 + " " + b6 + " " + b7;
            sendC1234C456Order = true;
        }

        /// <summary>
        /// 调节C35R/CB26电磁阀油压
        /// </summary>
        /// <param name="data1">C35R油压</param>
        /// <param name="data2">CB26油压</param>
        public void AdjustC35RCB26Pressure(int data1, int data2)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                调节C35RCB26接口");
            var b4 = Convert.ToString(data1*16, 16);
            var b6 = Convert.ToString(data2*16, 16);
            for (var i = b4.Length; i < 4; i++)
            {
                b4 = b4.Insert(0, "0");
            }
            for (var i = b6.Length; i < 4; i++)
            {
                b6 = b6.Insert(0, "0");
            }
            var b5 = b4.Substring(2, 2);
            b4 = b4.Substring(0, 2);
            var b7 = b6.Substring(2, 2);
            b6 = b6.Substring(0, 2);
            C35RCB26Order = "07 AE 3B C0 " + b4 + " " + b5 + " " + b6 + " " + b7;
            sendC35RCB26Order = true;
        }

        /// <summary>
        /// 调节TCC/EPC电磁阀油压
        /// </summary>
        /// <param name="data1">TCC油压</param>
        /// <param name="data2">EPC油压</param>
        public void AdjustTCCEPCPressure(int data1, int data2)
        {
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                调节TCCEPC接口");
            TCCvalue = data1*16;
            EPCvalue = data2*16;
            var b4 = Convert.ToString(data1*16, 16);
            var b6 = Convert.ToString(data2*16, 16);
            for (var i = b4.Length; i < 4; i++)
            {
                b4 = b4.Insert(0, "0");
            }
            for (var i = b6.Length; i < 4; i++)
            {
                b6 = b6.Insert(0, "0");
            }
            var b5 = b4.Substring(2, 2);
            b4 = b4.Substring(0, 2);
            var b7 = b6.Substring(2, 2);
            b6 = b6.Substring(0, 2);
            TCCEPCOrder = "07 AE 38 C0 " + b4 + " " + b5 + " " + b6 + " " + b7;
            sendTCCEPCOrder = true;
        }

        /// <summary>
        /// 调节换档电磁阀
        /// </summary>
        /// <param name="status">电磁阀状态</param>
        public void AdjustShiftSolenoid(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                调节换档电磁阀接口");
            shiftSolenoidOrder = status ? "04 AE 34 00 3C" : "04 AE 34 00 2C";
            sendShiftSolenoidOrder = true;
        }

        /// <summary>
        /// 电磁阀一键压力控制
        /// </summary>
        /// <param name="status">压力状态 true:全压;false:无压</param>
        public void SolenoidPressureControl(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                一键调节电磁阀压力接口");
            if (status)
            {
                C1234C456Order = "07 AE 3A C0 7D 00";
                sendC1234C456Order = true;
                C35RCB26Order = "07 AE 3B C0 7D 00";
                sendC35RCB26Order = true;
                TCCEPCOrder = "07 AE 38 C0 7D 00";
                sendTCCEPCOrder = true;
                shiftSolenoidOrder = "04 AE 34 00 3C";
                sendShiftSolenoidOrder = true;
            }
            else
            {
                C1234C456Order = "07 AE 3A C0 7D 00";
                sendC1234C456Order = true;
                C35RCB26Order = "07 AE 3B C0 7D 00";
                sendC35RCB26Order = true;
                TCCEPCOrder = "07 AE 38 C0 7D 00";
                sendTCCEPCOrder = true;
                shiftSolenoidOrder = "04 AE 34 00 3C";
                sendShiftSolenoidOrder = true;
            }
        }

        /// <summary>
        /// 刷写VIN
        /// </summary>
        /// <param name="vin">VIN</param>
        public void FlashVin(string vin)
        {
            if (vin.Length == 17)
            {
                var ACSvinCode = new byte[17];
                var vinCode = new string[17];
                ACSvinCode = System.Text.Encoding.ASCII.GetBytes(vin);
                for (var vinCodeNum = 0; vinCodeNum < 17; vinCodeNum++)
                {
                    vinCode[vinCodeNum] = Convert.ToUInt16(ACSvinCode[vinCodeNum]).ToString("X2");
                }
                sendVinCode[0] = "10 13 3B 90 " + vinCode[0] + " " + vinCode[1] + " " + vinCode[2] + " " + vinCode[3];
                sendVinCode[1] = "21 " + vinCode[4] + " " + vinCode[5] + " " + vinCode[6] + " " + vinCode[7] + " " +
                                 vinCode[8] + " " + vinCode[9] + " " + vinCode[10];
                sendVinCode[2] = "22 " + vinCode[11] + " " + vinCode[12] + " " + vinCode[13] + " " + vinCode[14] + " " +
                                 vinCode[15] + " " + vinCode[16];
                sendLongMsgCount = 0;
                flashVinCode = true;
                sendFirstLongMsg = true;
                allowSengLongMsg = false;
            }
            else
            {
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           VIN不为17个");
            }
        }

        /// <summary>
        /// 清空VIN
        /// </summary>
        public void ClearVin()
        {
            sendVinCode[0] = "10 13 3B 90 FF FF FF FF";
            sendVinCode[1] = "21 FF FF FF FF FF FF FF";
            sendVinCode[2] = "22 FF FF FF FF FF FF";
            sendLongMsgCount = 0;
            flashVinCode = true;
            sendFirstLongMsg = true;
            allowSengLongMsg = false;
        }

        /// <summary>
        /// 电磁阀油压调节指令值
        /// </summary>
        /// <param name="solenoid">电磁阀</param>
        /// <param name="data">油压值</param>
        public void AdjustSolenoid(string solenoid, int data)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入油压调节接口");
            adjustOrderValue[0] = solenoid;
            adjustOrderValue[1] = data.ToString();
            adjustOrderValue[2] = "null";
            adjustOrderValue[3] = "null";
            adjustSolenoid = true;
        }

        /// <summary>
        /// 电磁阀油压调节指令值
        /// </summary>
        /// <param name="solenoid1">电磁阀1</param>
        /// <param name="data1">油压值1</param>
        /// <param name="solenoid2">电磁阀2</param>
        /// <param name="data2">油压值2</param>
        public void AdjustSolenoid(string solenoid1, int data1, string solenoid2, int data2)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入油压调节接口");
            adjustOrderValue[0] = solenoid1;
            adjustOrderValue[1] = data1.ToString();
            adjustOrderValue[2] = solenoid2;
            adjustOrderValue[3] = data2.ToString();
            adjustSolenoid = true;
        }


        /// <summary>
        /// 查询TCU信息
        /// </summary>
        public void InquireTCUInfo()
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                  读取TCU信息");
            TCUInfo = new string[5];
            sendFirstLongMsg = true;
            inquireTcuInfo = true;
            inquireTcuInfoCount = 0;
        }

        /// <summary>
        /// 查询故障码
        /// </summary>
        public void GetFaultCode()
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                  读取故障码");
            faultCode[0] = "";
            noFaultCode = true;
            getFaultCode = true;
        }

        /// <summary>
        /// 清除故障码
        /// </summary>
        public void ClearFaultCode()
        {
            clearFaultCode = true;
        }

        #endregion

        #endregion

        #region 实现接口

        public void RegisterObserver(Observer o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(Observer o)
        {
            //throw new NotImplementedException();
            var i = observers.IndexOf(o);
            if (i > 0)
            {
                observers.Remove(i);
            }
        }

        public void NotifyObservers()
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           向界面更新数据");
            //throw new NotImplementedException();
            foreach (var t in observers)
            {
                var observer = (Observer) t;
                observer.Update(updateData);
            }
        }

        #endregion
    }
}