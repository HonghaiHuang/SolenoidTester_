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

/// <summary>
/// 测试完成委托
/// </summary>
/// <param name="data"></param>
public delegate void SolenoidTestEventHandler(string data);

/// <summary>
/// 显示当前测试步骤委托
/// </summary>
/// <param name="data"></param>
public delegate void ShowTestStepEventHandler(string data);

/// <summary>
/// TCU刷写完成委托
/// </summary>
public delegate void TcuFlashFinishEventHandler();

/// <summary>
/// TCU刷写中断委托
/// </summary>
public delegate void TcuFlashInterruptEventHandler(); //TCU刷写中断

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

        /// <summary>
        /// 测试完成委托时间
        /// </summary>
        public event SolenoidTestEventHandler SolenoidTest;

        /// <summary>
        /// 显示当前测试步骤委托
        /// </summary>
        public event ShowTestStepEventHandler ShowTestStep;

        /// <summary>
        /// TCU刷写完成委托事件
        /// </summary>
        public event TcuFlashFinishEventHandler TcuFlashFinish;

        /// <summary>
        /// TCU刷写中断委托事件
        /// </summary>
        public event TcuFlashInterruptEventHandler TcuFlashInterrupt;

        #endregion

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
        private volatile ArrayList observers;

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
        private volatile bool sendTcuOrder = true;

        /// <summary>
        /// 发送下位机指令标志位
        /// </summary>
        private volatile bool sendTableOrder = true;

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
            "02 1A CB 00 00 00 00 00", "02 1A B5 00 00 00 00 00", "02 1A A0 00 00 00 00 00",
            "02 1A B4 00 00 00 00 00", "30 00 05 00 00 00 00 00", "RcvLongMsg",
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
        private string[] tcuTempBuffer = new string[5] {"0", "0", "0", "0", "0"};

        /// <summary>
        /// TCU缓冲区1
        /// </summary>
        private string[] tcuBuffer1 = new string[5] {"0", "0", "0", "0", "0"};

        /// <summary>
        /// TCU缓冲区2
        /// </summary>
        private string[] tcuBuffer2 = new string[5] {"0", "0", "0", "0", "0"};

        /// <summary>
        /// TCU循环采集数据翻转标志位
        /// </summary>
        private volatile bool tcuRcvFlagFlip = false;

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
        private volatile string heatingWireControlOrder = "";

        /// <summary>
        /// TCU供电开关控制标志位
        /// </summary>
        private volatile bool tcuPowerControl = false;

        /// <summary>
        /// TCU供电开关控制指令
        /// </summary>
        private volatile string tcuPowerControlOrder = "";

        /// <summary>
        /// 开始/停止采集下位机数据标志位
        /// </summary>
        private volatile bool collectMcuData = false;

        /// <summary>
        /// 开始/停止采集下位机数据指令
        /// </summary>
        private volatile string collectMcuDataOrder = "";

        /// <summary>
        /// 禁用旋钮标志位
        /// </summary>
        private volatile bool disableKnob = false;

        /// <summary>
        /// 禁用旋钮指令
        /// </summary>
        private volatile string disableKnobOrder = "";

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
        private volatile string[] liveUpdateData = new string[8] { "0","0", "0", "0", "0", "0", "0", "0" };

        /// <summary>
        /// 电磁阀调节值赋值标志位
        /// </summary>
        private volatile bool adjustSolenoid = false;

        /// <summary>
        /// 转速调节值赋值标志位
        /// </summary>
        private volatile bool adjustSpeed = false;

        /// <summary>
        /// 电磁阀调节值
        /// </summary>
        private volatile string[] adjustOrderValue = new string[6] {"", "", "", "", "", ""};

        /// <summary>
        /// 电磁阀阶跃测试标志位
        /// </summary>
        private bool solenoidStepTest = false;

        /// <summary>
        /// 步长
        /// </summary>
        private int stepSize = 0;

        /// <summary>
        /// 起始压力值
        /// </summary>
        private int beginPress = 0;

        /// <summary>
        /// 最终压力值
        /// </summary>
        private int endPress = 0;

        /// <summary>
        /// 保持时间
        /// </summary>
        private int keepTime = 0;

        /// <summary>
        /// 测试完成后电磁阀状态
        /// </summary>
        private int testEndStatus = 0;

        /// <summary>
        /// 电磁阀调节指令
        /// </summary>
        private string solenoidAdjustOrder = "";

        /// <summary>
        /// 阶跃测试时间间隔
        /// </summary>
        private DateTime SolenoidStepInverval = new DateTime();

        /// <summary>
        /// 测试电磁阀名称
        /// </summary>
        private string solenoidName = "";

        /// <summary>
        /// TCCEPC周期指令标志位
        /// </summary>
        private bool sendTccEpcCycleOrder = false;

        /// <summary>
        /// 成功接收2703时间判断
        /// </summary>
        private DateTime Recv2703Timer = DateTime.Now;


        /// <summary>
        /// 测试项指令发送标志位
        /// </summary>
        private bool sendAdjustTestOrder = true;

        #endregion

        #region TCU刷写变量

        /// <summary>
        /// 刷写类型
        /// </summary>
        private string flashType = "";

        /// <summary>
        /// 刷写TCU标志位
        /// </summary>
        private bool startFlashTcu = false;

        /// <summary>
        /// 刷写TCU准备阶段指令
        /// </summary>
        private string[] flashTcuPrepareOrder = new string[]
        {
            "FE 01 3E", "FE 02 1A B0", "FE 02 10 02", "FE 01 28", "FE 01 3E", "FE 01 A2", "FE 01 3E", "FE 02 A5 01",
            "FE 01 3E", "FE 02 A5 03", "FE 01 3E", "FE 01 3E"
        };

        /// <summary>
        /// 确保TCU刷写指令返回
        /// </summary>
        private bool flashTcuOrderResponse = true;

        /// <summary>
        /// TCU刷写心跳
        /// </summary>
        private DateTime flashTcuHeartBeat = new DateTime();

        /// <summary>
        /// 刷写TCU准备指令计数
        /// </summary>
        private int flashTcuCount = 0;

        /// <summary>
        /// 请求SEED
        /// </summary>
        private string[] requestSeed = new string[] {"02 27 01", "04 27 02 ", "05 34 00 00 0C 20", "1C 26 36 00"};

        /// <summary>
        /// 刷写T76_ReFlash指令
        /// </summary>
        private string[] flsahT76_ReFlash = new string[] {"04 34 00 04 00", "14 06 36 00"};

        /// <summary>
        /// 发送KEY标志位
        /// </summary>
        private bool SendKeySusses = false;

        /// <summary>
        /// 由seed算出来的key值
        /// </summary>
        private string keyValue = "";

        /// <summary>
        /// TCU刷写缓冲区0
        /// </summary>
        private string[] TCUflashBuffer0;

        /// <summary>
        /// TCU刷写缓冲区1
        /// </summary>
        private string[] TCUflashBuffer1;

        /// <summary>
        /// TCU刷写缓冲区2
        /// </summary>
        private string[] TCUflashBuffer2;

        /// <summary>
        /// TCU刷写缓冲区3
        /// </summary>
        private string[] TCUflashBuffer3;

        /// <summary>
        /// TCU刷写缓冲区4
        /// </summary>
        private string[] TCUflashBuffer4;


        /// <summary>
        /// TCU刷写长报文前缀起始位
        /// </summary>
        private int tcuFlashLongPacketNum = 1;

        /// <summary>
        /// TCU刷写长报文前缀
        /// </summary>
        private string[] tcuFlashLongPacket = new string[]
        {"20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F"};

        /// <summary>
        /// 刷写Buffer指令
        /// </summary>
        private string[] flashBootOrder = new string[] {"04 34 00 0F F6", "11 06 36 00 00 3F C0 00"};

        private string[] flashBootOrder2 = new string[] {"11 06 36 00 00 3F C0 00"};

        private string[] flashBootOrder3 = new string[] {"1F F6 36 00 00 3F C0 00"};

        private string[] flashBootOrder4 = new string[]
        {"04 34 00 04 00", "06 36 80 00 3F BC 00", "FE 01 3E", "06 36 80 00 3F BC 00", "FE 01 3E", "FE 01 20"};

        private string[] flashmainOrder = new string[] {"02 1A C1", "04 34 00 0F F6", "11 06 36 00 00 3F C0 00"};

        private string[] flashmainOrder1 = new string[] {"1A 16 36 00 00 3F C0 00"};

        private string[] flashmainOrder2 = new string[] {"1F F6 36 00 00 3F C0 00"};

        private string[] flashmainOrder3 = new string[] {"04 34 00 0F F6", "1F F6 36 00 00 3F C0 00"};

        private string[] flashmainOrder4 = new string[] {"1F F6 36 00 00 3F C0 00"};

        private string[] flashmainOrder5 = new string[] {" 36 00 00 3F C0 00"};

        private string[] flashmainOrder6 = new string[]
        {
            "FE 01 20", "FE 02 1A B0", "10 13 3B 90 FF FF FF FF", "21 FF FF FF FF FF FF FF", "22 FF FF FF FF FF FF AA",
            "10 0C 3B 98 00 00 00 00", "21 00 00 00 00 00 00", "06 3B 99 ",
            "06 3B CB ", "03 AE 28 40", "03 AE 28 40"
        };

        /// <summary>
        /// 刷写SetToZero标志位
        /// </summary>
        private bool SetToZeroFlag = false;

        /// <summary>
        /// 刷写TCU中断标志位
        /// </summary>
        private bool flashTcuInterrupt = false;

        /// <summary>
        /// 刷写中断计时
        /// </summary>
        private DateTime interruptTime = DateTime.Now;

        /// <summary>
        /// TCCEPC和心跳计时器
        /// </summary>
        //public System.Timers.Timer CheckTimer = new System.Timers.Timer(200);

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
            liveUpdateData[1] = "0";
            liveUpdateData[2] = "0";
            liveUpdateData[3] = "0";
            liveUpdateData[4] = "0";
            liveUpdateData[5] = "0";
            liveUpdateData[6] = "0";
            liveUpdateData[7] = "0";

            TCCvalue = 0;
            EPCvalue = 0;
        }

        private DateTime TthrSleepTime = new DateTime();
        private TimeSpan sleepTime = new TimeSpan();

        /// <summary>
        /// 采集测试台架数据
        /// </summary>
        private void DataProcessThread()
        {
            var mcuData = new string[19];
            var mcuDataCount = 0;
            var collocetMcuData = false;
            var recvData = new string[8] {"", "", "", "", "", "", "", ""};
            while (true)
            {
                #region 数据接收部分

                TthrSleepTime = DateTime.Now.AddMilliseconds(5);
                //Thread.Sleep(5);
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN接收数据开始");
                var data = CAN.RecviveData();
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                CAN接收数据结束");


                if (data.Length != 0)
                {
                    foreach (string t in data)
                    {
                        recvData = t.Split(' ');

                        //TCU数据采集指令响应
                        if (recvData[1] == "62")
                        {
                            sendTcuOrder = true;
                            sendCollectOeder = true;

                            #region 存储TCU采集数据

                            if (recvData[2] + recvData[3] == "1941")
                            {
                                tcuTempBuffer[0] =
                                    Convert.ToString(
                                        (Convert.ToInt32(
                                            recvData[recvData.Length - 4] + recvData[recvData.Length - 3],
                                            16)/
                                         8));
                                Debug.WriteLine("                                  输入转速：" + tcuTempBuffer[0]);
                            }
                            if (recvData[2] + recvData[3] == "1942")
                            {
                                tcuTempBuffer[1] =
                                    Convert.ToString(
                                        (Convert.ToInt32(
                                            recvData[recvData.Length - 4] + recvData[recvData.Length - 3],
                                            16)/
                                         8));
                                Debug.WriteLine("                                  输出转速：" + tcuTempBuffer[1]);
                            }
                            if (recvData[2] + recvData[3] == "194F")
                            {
                                if (Convert.ToInt32(recvData[4], 16) - 0 == 0)
                                    tcuTempBuffer[2] = "无效档";
                                if (Convert.ToInt32(recvData[4], 16) - 1 == 0)
                                    tcuTempBuffer[2] = "P档";
                                if (Convert.ToInt32(recvData[4], 16) - 2 == 0)
                                    tcuTempBuffer[2] = "R档";
                                if (Convert.ToInt32(recvData[4], 16) - 4 == 0)
                                    tcuTempBuffer[2] = "N档";
                                if (Convert.ToInt32(recvData[4], 16) - 6 == 0)
                                    tcuTempBuffer[2] = "D档";
                                Debug.WriteLine("                                  档位：" + tcuTempBuffer[2]);
                            }
                            if (recvData[2] + recvData[3] == "280D")
                            {
                                tcuTempBuffer[3] = Convert.ToString((Convert.ToInt32(recvData[4], 16) - 40));
                                Debug.WriteLine("                                  TCM温度：" + tcuTempBuffer[3]);
                            }
                            if (recvData[2] + recvData[3] == "281D")
                            {
                                var str = Convert.ToString(Convert.ToInt32(recvData[4], 16), 2);
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
                        if (recvData[1] == "EE" || recvData[1] == "7F")
                        {
                            sendTcuOrder = true;
                            sendCollectOeder = true;
                            if (recvData[1] == "7F")
                            {
                                //MessageBox.Show(recvData[1]+ recvData[2] + recvData[3]);
                            }
                        }
                        //发送2703响应
                        if (recvData[1] == "67")
                        {
                            recv6703 = true;
                            sendTcuOrder = true;
                            sendCollectOeder = true;
                            TCCEPCInterval = DateTime.Now;
                        }
                        if (recvData[1] + recvData[2] == "7F27")
                        {
                            //MessageBox.Show("2703执行失败！");
                        }
                        //发送心跳响应
                        if (recvData[1] == "7E")
                        {
                            sendTcuOrder = true;
                            sendCollectOeder = true;
                        }
                        //产生0160
                        if (recvData[0] + recvData[1] == "0160")
                        {
                            send2703 = false;
                            recv6703 = false;
                            sendTcuOrder = true;
                            sendCollectOeder = true;
                            //MessageBox.Show("出现0160！");
                        }
                        if (recvData[1] + recvData[2] == "5AA0")
                        {
                            flashTcuOrderResponse = true;
                        }
                        //允许发送长报文响应
                        if (recvData[0] + recvData[1] == "3000")
                        {
                            sendTcuOrder = true;
                            allowSengLongMsg = true;
                        }
                        //刷写VIN成功响应
                        if (recvData[1] == "7B")
                        {
                            sendTcuOrder = true;
                        }

                        #region 查询TCU信息响应内容

                        //查询基础零件号响应
                        if (recvData[1] + recvData[2] == "5AB5")
                        {
                            sendTcuOrder = true;
                            TCUInfo[0] = ((char) (Convert.ToUInt16(recvData[3], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(recvData[4], 16))).ToString()
                                         + ((char) (Convert.ToUInt16(recvData[5], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(recvData[6], 16))).ToString();
                        }
                        //查询终端零件号响应
                        if (recvData[1] + recvData[2] == "5ACB")
                        {
                            sendTcuOrder = true;
                            TCUInfo[1] =
                                Convert.ToUInt32(recvData[3] + recvData[4] + recvData[5] + recvData[6], 16)
                                    .ToString();
                        }
                        //查询商用状态响应
                        if (recvData[1] + recvData[2] == "5AA0" && !SetToZeroFlag)
                        {
                            sendTcuOrder = true;
                            if (recvData[3] == "00")
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
                        if (recvData[2] + recvData[3] == "5AB4")
                        {
                            sendTcuOrder = true;
                            recvMTCFirstMsg = true;
                            TCUInfo[3] = ((char) (Convert.ToUInt16(recvData[4], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(recvData[5], 16))).ToString()
                                         + ((char) (Convert.ToUInt16(recvData[6], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(recvData[7], 16))).ToString();

                            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[3]);
                        }

                        //查询VIN响应
                        if (recvData[2] + recvData[3] == "5A90")
                        {
                            sendTcuOrder = true;
                            recvVINFirst = true;
                            TCUInfo[4] = ((char) (Convert.ToUInt16(recvData[4], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(recvData[5], 16))).ToString()
                                         + ((char) (Convert.ToUInt16(recvData[6], 16))).ToString() +
                                         ((char) (Convert.ToUInt16(recvData[7], 16))).ToString();
                            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[4]);
                        }
                        if (recvData[0] == "21")
                        {
                            if (recvMTCFirstMsg)
                            {
                                sendTcuOrder = true;
                                TCUInfo[3] = TCUInfo[3] + ((char) (Convert.ToUInt16(recvData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(recvData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[5], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(recvData[6], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[7], 16))).ToString();
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[3]);
                            }
                            if (recvVINFirst)
                            {
                                sendTcuOrder = true;
                                TCUInfo[4] = TCUInfo[4] + ((char) (Convert.ToUInt16(recvData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(recvData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[5], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(recvData[6], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[7], 16))).ToString();
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[4]);
                            }
                        }
                        if (recvData[0] == "22")
                        {
                            if (recvMTCFirstMsg)
                            {
                                sendTcuOrder = true;
                                TCUInfo[3] = TCUInfo[3] + ((char) (Convert.ToUInt16(recvData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(recvData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[5], 16))).ToString();
                                recvMTCFirstMsg = false;
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[3]);
                            }
                            if (recvVINFirst)
                            {
                                sendTcuOrder = true;
                                inquireTcuInfoCount = 0;
                                inquireTcuInfo = false;
                                TCUInfo[4] = TCUInfo[4] +
                                             ((char) (Convert.ToUInt16(recvData[1], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[2], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(recvData[3], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[4], 16))).ToString() +
                                             ((char) (Convert.ToUInt16(recvData[5], 16))).ToString()
                                             + ((char) (Convert.ToUInt16(recvData[6], 16))).ToString();
                                RecvTcuInfo(TCUInfo);
                                //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           接收到" + TCUInfo[4]);
                                recvVINFirst = false;
                                GetFaultCode();
                            }

                            #endregion
                        }
                        //查询故障码响应
                        if (recvData[0] == "81")
                        {
                            if (recvData[1] + recvData[2] != "0000")
                            {
                                noFaultCode = true;
                                faultCode[0] = faultCode[0] + "P" + recvData[1] + recvData[2];
                            }
                            else
                            {
                                if (!noFaultCode)
                                {
                                    faultCode[0] = "无故障码";
                                }
                                sendTcuOrder = true;
                                RcvTcuFaultCode(faultCode);
                            }
                        }
                        //清空故障码响应
                        if (recvData[0] + recvData[1] == "0144")
                        {
                            //故障码已清除
                            faultCode[0] = "故障码已清除";
                            RcvTcuFaultCode(faultCode);
                            sendTcuOrder = true;
                        }
                        if (recvData[0] == "00")
                        {
                            if (recvData[1] == "00")
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
                                            recvData[tableDataNum*2 + 3] + recvData[tableDataNum*2 + 2], 16);
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

                        #region TCU刷写响应内容

                        //刷写准备指令响应
                        if (recvData[1] + recvData[2] == "5AB0")
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " Request Diag Addr");
                            flashTcuOrderResponse = true;
                        }
                        if (recvData[1] == "50")
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " Disable DTCs");
                            flashTcuOrderResponse = true;
                        }
                        if (recvData[1] == "68")
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " Stop Normal Comm");
                            flashTcuOrderResponse = true;
                        }
                        if (recvData[1] == "E2")
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " Request Prog State");
                            flashTcuOrderResponse = true;
                        }
                        if (recvData[1] == "E5")
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                            " Request Prog Low Speed");
                            flashTcuOrderResponse = true;
                        }
                        //seed请求响应
                        if (recvData[1] + recvData[2] == "6701")
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                            " Unlock - request key");
                            if (recvData[3] + recvData[4] != "0000")
                            {
                                SendKeySusses = true;
                                keyValue = Arithmetic(recvData[3] + recvData[4]).Substring(0, 2) + " " +
                                           Arithmetic(recvData[3] + recvData[4]).Substring(2, 2);
                                flashTcuOrderResponse = true;
                            }
                            if (recvData[3] + recvData[4] == "0000")
                            {
                                SendKeySusses = true;
                                keyValue = "NOSEED";
                                flashTcuOrderResponse = true;
                            }
                        }

                        /*if (recvData[0] + recvData[1] == "0176")
                        {
                            flashTcuOrderResponse = true;
                            tcuRtn0176 = true;
                        }
                        if (recvData[0] + recvData[1] == "0174")
                        {
                            tcuRtn0174 = true;
                            flashTcuOrderResponse = true;
                        }*/

                        #endregion

                        for (var a = 0; a < recvData.Length; a++)
                        {
                            recvData[a] = "";
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
                    recv6703 = false;
                    sendCollectOeder = false;
                    TCUOrderList.Add("02 27 03");
                    Recv2703Timer = DateTime.Now.AddMilliseconds(100);
                }

                //100毫秒内没有收到2703的应答，继续发送2703
                if (DateTime.Now > Recv2703Timer && !recv6703)
                {
                    send2703 = false;
                }

                //阶跃测试
                if (solenoidStepTest && DateTime.Now > SolenoidStepInverval && sendAdjustTestOrder)
                {
                    if (beginPress != endPress + stepSize)
                    {
                        if (beginPress < 0)
                        {
                            AdjustSolenoid(solenoidName, -beginPress);
                            TCUOrderList.Add(SolenoidAdjustOrder(solenoidName, -beginPress));
                        }
                        else
                        {
                            AdjustSolenoid(solenoidName, beginPress);
                            TCUOrderList.Add(SolenoidAdjustOrder(solenoidName, beginPress));
                        }

                        beginPress += stepSize;
                        SolenoidStepInverval = DateTime.Now.AddMilliseconds(keepTime);
                        sendCollectOeder = false;
                        sendAdjustTestOrder = false;
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                发送阶跃测试指令");
                    }
                    else
                    {
                        TCUOrderList.Add(SolenoidAdjustOrder(solenoidName, testEndStatus));
                        solenoidStepTest = false;
                        SolenoidTest(liveUpdateData[1]);
                    }
                }

                //换档性能测试
                if (shiftPerformanceTest)
                {
                    if (solenoid1_StepOrder < 0 && !solenoid1TestFinish)
                    {
                        solenoid1_FinallName = solenoid1_Name;
                        solenoid1_StepOrder = 9999;
                        solenoid1_Name = "空";
                        solenoid1TestFinish = true;
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                        "                换档性能测试第一个电磁阀调节完成");
                    }
                    if (solenoid2_StepOrder > 2000 && solenoid2_StepOrder != 9999 && !solenoid2TestFinish)
                    {
                        solenoid2_StepOrder = 9999;
                        solenoid2_Name = "空";
                        solenoid2TestFinish = true;
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                        "                换档性能测试第二个电磁阀调节完成");
                    }
                    if (solenoid1TestFinish && solenoid2TestFinish)
                    {
                        //solenoid1TestFinish = false;
                        //solenoid2TestFinish = false;
                        shiftPerformanceTest = false;
                        sendfirstOrder = false;
                        AdjustShiftSolenoid(true);
                        SolenoidTest(liveUpdateData[1]);
                        SolenoidPressureControl(true);
                        sendCollectOeder = true;
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                换档性能测试完成");
                    }

                    if (DateTime.Now > sendOrderInvervalTime && shiftPerformanceTest)
                    {
                        var orderStr = new string[1];

                        if (!sendfirstOrder)
                        {
                            sendfirstOrder = true;
                            shiftInvervalTime = DateTime.Now.AddMilliseconds(shiftSolenoidInverval);
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                发送第一条指令");
                        }
                        if (DateTime.Now > shiftInvervalTime && !shiftInvervalFlag)
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                            "                可以调节第二个电磁阀");
                            solenoid2_Name = lastsolenoid_Name;
                            solenoid2_StepOrder = 0;
                            shiftInvervalFlag = true;
                        }
                        if (solenoid1_StepOrder != 9999 && !solenoid1TestFinish)
                        {
                            orderStr = ShiftPerformOrder(solenoid1_Name, solenoid1_StepOrder, solenoid2_Name,
                                solenoid2_StepOrder);
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                            "                第一个电磁阀动作指令");
                            solenoid1_StepOrder -= solenoid1_Step;
                            sendOrderInvervalTime = DateTime.Now.AddMilliseconds(sendOrderTime);
                        }

                        //当到换档时间间隔后，开始赋值
                        if (solenoid2_StepOrder != 9999 && !solenoid2TestFinish)
                        {
                            orderStr = ShiftPerformOrder(solenoid1_Name, solenoid1_StepOrder, solenoid2_Name,
                                solenoid2_StepOrder);
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                            "                第二个电磁阀动作指令");
                            solenoid2_StepOrder += solenoid2_Step;
                            sendOrderInvervalTime = DateTime.Now.AddMilliseconds(sendOrderTime);
                        }

                        for (int i = 0; i < orderStr.Length; i++)
                        {
                            if (orderStr[i] != "" && orderStr[i] != null)
                            {
                                if (solenoid1_StepOrder != 9999 && !solenoid1TestFinish)
                                {
                                    AdjustSolenoid(solenoid1_Name, solenoid1_StepOrder + solenoid1_Step);
                                }
                                if (solenoid2_StepOrder != 9999 && !solenoid2TestFinish)
                                {
                                    AdjustSolenoid(solenoid2_Name, solenoid2_StepOrder - solenoid2_Step);
                                }
                                TCUOrderList.Add(orderStr[i]);
                                sendAdjustTestOrder = false;
                                //ShowTestStep(PerformTestName(liveUpdateData[1]) + orderStr[i]);
                                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                "                发送性能测试指令");
                            }
                        }
                        sendCollectOeder = false;
                    }
                }

                //刷写TCU
                if (startFlashTcu)
                {
                    #region 刷写reset

                    if (flashType == "reset")
                    {
                        SendTcuFlashHeartBeat(DateTime.Now);
                        if (flashTcuCount >= 0 && flashTcuCount < 12 && startFlashTcu)
                        {
                            if (flashTcuCount == 0)
                            {
                                interruptTime = DateTime.Now.AddSeconds(10);
                            }
                            if (DateTime.Now > interruptTime)
                            {
                                SendKeySusses = true;
                                startFlashTcu = false;
                                tcuFlashLongPacketNum = 1;
                                TcuFlashInterrupt();
                            }
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                SendDataToTcu(flashTcuPrepareOrder[flashTcuCount]);
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 01 3E")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 A5 01")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 A5 03")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        if (flashTcuCount >= 12 && flashTcuCount < 16 && startFlashTcu)
                        {
                            if (flashTcuCount == 12)
                            {
                                interruptTime = DateTime.Now.AddSeconds(5);
                            }
                            if (DateTime.Now > interruptTime)
                            {
                                SendKeySusses = true;
                                startFlashTcu = false;
                                tcuFlashLongPacketNum = 1;
                                TcuFlashInterrupt();
                            }
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                if (flashTcuCount == 12)
                                {
                                    keyValue = "";
                                    SendKeySusses = false;
                                    SendDataToTcu(requestSeed[flashTcuCount - 12]);
                                }
                                if (flashTcuCount == 13)
                                {
                                    if (keyValue != "NOSEED" && keyValue != "")
                                    {
                                        SendDataToTcu(requestSeed[flashTcuCount - 12] + keyValue);
                                        flashTcuOrderResponse = true;
                                        //Thread.Sleep(10);
                                    }
                                    if (keyValue == "NOSEED")
                                    {
                                        flashTcuOrderResponse = true;
                                    }
                                }
                                if (flashTcuCount == 14)
                                {
                                    SendDataToTcu(requestSeed[flashTcuCount - 12]);
                                    WaitTcuRtn0174();
                                }
                                if (flashTcuCount == 15)
                                {
                                    SendDataToTcu(requestSeed[flashTcuCount - 12] + " " +
                                                  TCUflashBuffer0[0] +
                                                  " " + TCUflashBuffer0[1] + " " + TCUflashBuffer0[2] + " " +
                                                  TCUflashBuffer0[3]);
                                }
                                /*if (requestSeed[flashTcuCount - 12] == "05 34 00 00 0C 20")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }*/
                                if (requestSeed[flashTcuCount - 12].Length > 9)
                                {
                                    if (requestSeed[flashTcuCount - 12].Substring(0, 11) == "1C 26 36 00")
                                    {
                                        flashTcuOrderResponse = true;
                                        Thread.Sleep(10);
                                    }
                                }
                            }
                            tcuFlashLongPacketNum = 1;
                        }
                        DateTime waitOneSecond = new DateTime();
                        if (flashTcuCount == 16 && startFlashTcu)
                        {
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 6] +
                                                  " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 7] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 8]
                                                  + " " + TCUflashBuffer0[(i - 1)*7 + 9] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 10] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 11] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 12]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                    i--;
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            for (int y = 0; y < 27; y++)
                            {
                                for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                {
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 428)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 111] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 112] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 113]
                                                          + " " + TCUflashBuffer0[y*16*7 + (i)*7 + 114] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 115] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 116] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 117]);
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 111] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 112] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 113] + " AA AA AA AA");
                                            WaitTcuRtn0176();
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                        i--;
                                    }
                                }
                            }
                        }
                        if (flashTcuCount >= 17 && flashTcuCount < 19 && startFlashTcu)
                        {
                            SendTcuFlashHeartBeat(DateTime.Now);

                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                if (flashTcuCount == 18)
                                {
                                    SendDataToTcu(flsahT76_ReFlash[flashTcuCount - 17] + " " +
                                                  TCUflashBuffer0[3110] +
                                                  " " + TCUflashBuffer0[3111] + " " + TCUflashBuffer0[3112] + " " +
                                                  TCUflashBuffer0[3113]);
                                    WaitAllowSendLongMsg();
                                }
                                else
                                {
                                    SendDataToTcu(flsahT76_ReFlash[flashTcuCount - 17]);
                                    WaitTcuRtn0174();
                                }
                            }
                        }
                        if (flashTcuCount == 19 && startFlashTcu)
                        {
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3116] +
                                                  " " + TCUflashBuffer0[(i - 1)*7 + 3117]
                                                  + " " + TCUflashBuffer0[(i - 1)*7 + 3118] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3119] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3120] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3121] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3122]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    i--;
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            for (int y = 0; y < 9; y++)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 131)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3221] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3222]
                                                          + " " + TCUflashBuffer0[y*16*7 + (i)*7 + 3223] +
                                                          " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3224] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3225] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3226] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3227]);

                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3221] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3222] + " AA AA AA AA AA");
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                            WaitTcuRtn0176();
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                        i--;
                                    }
                                }
                            }
                        }
                        if (flashTcuCount == 20 && startFlashTcu)
                        {
                            SendDataToTcu("06 36 80 00 3F BC 00");
                        }
                        if (flashTcuCount == 21 && startFlashTcu)
                        {
                            SendDataToTcu("FE 01 3E");
                        }
                        if (flashTcuCount == 22 && startFlashTcu)
                        {
                            SendDataToTcu("06 36 80 00 3F BC 00");
                        }
                        if (flashTcuCount == 23 && startFlashTcu)
                        {
                            SendDataToTcu("FE 01 20");
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") +
                                            "                                   Reset刷写完毕");
                            //MessageBox.Show("Reset刷写完毕！");
                            TcuFlashFinish();
                            send2703 = false;
                            recv6703 = false;
                            sendTcuOrder = true;
                            sendCollectOeder = true;
                            //CheckTimer.Start();

                            flashTcuCount = 41;
                        }
                    }

                    #endregion

                    #region 刷写settozero

                    if (flashType == "zero")
                    {
                        if (flashTcuCount >= 0 && flashTcuCount < 12 && startFlashTcu)
                        {
                            if (flashTcuCount == 0)
                            {
                                interruptTime = DateTime.Now.AddSeconds(5);
                            }
                            if (DateTime.Now > interruptTime)
                            {
                                SendKeySusses = true;
                                startFlashTcu = false;
                                tcuFlashLongPacketNum = 1;
                                TcuFlashInterrupt();
                            }
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                SendDataToTcu(flashTcuPrepareOrder[flashTcuCount]);
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 01 3E")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 A5 03")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 10 02")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 01 A2")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        if (flashTcuCount == 12 && startFlashTcu)
                        {
                            SendDataToTcu("FE 02 1A B0");
                        }
                        if (flashTcuCount == 13 && startFlashTcu)
                        {
                            SendDataToTcu("FE 01 3E");
                        }
                        if (flashTcuCount == 14 && startFlashTcu)
                        {
                            SetToZeroFlag = true;
                            SendDataToTcu("02 1A A0");
                            Thread.Sleep(10);
                        }
                        if (flashTcuCount == 15 && startFlashTcu)
                        {
                            flashTcuOrderResponse = false;
                            SendDataToTcu("03 3B A0 00");
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") +
                                            "                                   ZERO刷写完毕");
                            TcuFlashFinish();
                            send2703 = false;
                            recv6703 = false;
                            sendTcuOrder = true;
                            sendCollectOeder = true;
                            flashTcuCount = 41;
                            SetToZeroFlag = false;
                            //CheckTimer.Start();
                        }
                    }

                    #endregion

                    #region 刷写boot

                    if (flashType == "boot")
                    {
                        if (flashTcuCount >= 0 && flashTcuCount < 12 && startFlashTcu)
                        {
                            if (flashTcuCount == 0)
                            {
                                interruptTime = DateTime.Now.AddSeconds(5);
                            }
                            if (DateTime.Now > interruptTime)
                            {
                                SendKeySusses = true;
                                startFlashTcu = false;
                                tcuFlashLongPacketNum = 1;
                                TcuFlashInterrupt();
                            }
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                SendDataToTcu(flashTcuPrepareOrder[flashTcuCount]);
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 01 3E")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 A5 03")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 10 02")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 01 A2")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        if (flashTcuCount >= 12 && flashTcuCount < 16 && startFlashTcu)
                        {
                            if (flashTcuCount == 12)
                            {
                                interruptTime = DateTime.Now.AddSeconds(5);
                            }
                            if (DateTime.Now > interruptTime)
                            {
                                SendKeySusses = true;
                                startFlashTcu = false;
                                tcuFlashLongPacketNum = 1;
                                TcuFlashInterrupt();
                            }
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                if (flashTcuCount == 12)
                                {
                                    keyValue = "";
                                    SendKeySusses = false;
                                    SendDataToTcu(requestSeed[flashTcuCount - 12]);
                                }
                                if (flashTcuCount == 13)
                                {
                                    if (keyValue != "NOSEED" && keyValue != "")
                                    {
                                        SendDataToTcu(requestSeed[flashTcuCount - 12] + keyValue);
                                        flashTcuOrderResponse = true;
                                        //Thread.Sleep(10);
                                    }
                                    if (keyValue == "NOSEED")
                                    {
                                        flashTcuOrderResponse = true;
                                    }
                                }
                                if (flashTcuCount == 14)
                                {
                                    SendDataToTcu(requestSeed[flashTcuCount - 12]);
                                    WaitTcuRtn0174();
                                }
                                if (flashTcuCount == 15)
                                {
                                    SendDataToTcu(requestSeed[flashTcuCount - 12] + " " +
                                                  TCUflashBuffer0[0] +
                                                  " " + TCUflashBuffer0[1] + " " + TCUflashBuffer0[2] + " " +
                                                  TCUflashBuffer0[3]);
                                }
                                /*if (requestSeed[flashTcuCount - 12] == "05 34 00 00 0C 20")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }*/
                                if (requestSeed[flashTcuCount - 12].Length > 9)
                                {
                                    if (requestSeed[flashTcuCount - 12].Substring(0, 11) == "1C 26 36 00")
                                    {
                                        flashTcuOrderResponse = true;
                                        Thread.Sleep(10);
                                    }
                                }
                            }
                        }
                        DateTime waitOneSecond = new DateTime();
                        if (flashTcuCount == 16 && startFlashTcu)
                        {
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 6] +
                                                  " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 7] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 8]
                                                  + " " + TCUflashBuffer0[(i - 1)*7 + 9] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 10] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 11] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 12]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                    i--;
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            for (int y = 0; y < 27; y++)
                            {
                                for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                {
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 428)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 111] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 112] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 113]
                                                          + " " + TCUflashBuffer0[y*16*7 + (i)*7 + 114] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 115] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 116] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 117]);
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 111] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 112] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 113] + " AA AA AA AA");
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                        i--;
                                    }
                                }
                            }
                            WaitTcuRtn0176();
                        }
                        if (flashTcuCount >= 17 && flashTcuCount < 19 && startFlashTcu)
                        {
                            SendTcuFlashHeartBeat(DateTime.Now);
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                if (flashTcuCount == 18)
                                {
                                    SendDataToTcu(flsahT76_ReFlash[flashTcuCount - 17] + " " +
                                                  TCUflashBuffer0[3110] +
                                                  " " + TCUflashBuffer0[3111] + " " + TCUflashBuffer0[3112] + " " +
                                                  TCUflashBuffer0[3113]);
                                }
                                else
                                {
                                    SendDataToTcu(flsahT76_ReFlash[flashTcuCount - 17]);
                                    WaitTcuRtn0174();
                                }
                            }
                            tcuFlashLongPacketNum = 1;
                        }
                        if (flashTcuCount == 19 && startFlashTcu)
                        {
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3116] +
                                                  " " + TCUflashBuffer0[(i - 1)*7 + 3117]
                                                  + " " + TCUflashBuffer0[(i - 1)*7 + 3118] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3119] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3120] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3121] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3122]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    i--;
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            for (int y = 0; y < 9; y++)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y * 16*7 + i * 7 + 111));
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + y);
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + cOUNTNUM++);
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y * 16 + i));
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 131)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3221] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3222]
                                                          + " " + TCUflashBuffer0[y*16*7 + (i)*7 + 3223] +
                                                          " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3224] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3225] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3226] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3227]);

                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3221] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3222] + " AA AA AA AA AA");
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                        i--;
                                    }
                                }
                            }
                            WaitTcuRtn0176();
                        }
                        if (flashTcuCount >= 20 && flashTcuCount < 22 && startFlashTcu)
                        {
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                SendDataToTcu(flashBootOrder[flashTcuCount - 20]);
                                if (flashTcuCount == 20)
                                {
                                    WaitTcuRtn0174();
                                }
                                if (flashTcuCount == 21)
                                {
                                    WaitAllowSendLongMsg();
                                }
                            }

                            tcuFlashLongPacketNum = 1;
                        }
                        if (flashTcuCount == 22 && startFlashTcu)
                        {
                            //刷写第一部分的256个字节
                            //刷写21到2F的105个字节
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " + TCUflashBuffer1[(i - 1)*7] +
                                                  " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 1] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 2]
                                                  + " " + TCUflashBuffer1[(i - 1)*7 + 3] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 4] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 5] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 6]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                    i--;
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            //刷写剩下的151个字节
                            for (int y = 0; y < 2; y++)
                            {
                                for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                {
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 21)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 105] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 106] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 107]
                                                          + " " + TCUflashBuffer1[y*16*7 + (i)*7 + 108] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 109] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 110] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 111]);
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 105] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 106] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 107]
                                                          + " " + TCUflashBuffer1[y*16*7 + (i)*7 + 108] + " AA AA AA");
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                        i--;
                                    }
                                }
                            }
                            WaitTcuRtn0176();
                            //剩余部分字节刷写
                            for (int l = 0; l < 17; l++)
                            {
                                //没刷写4080个字节前需发送的指令

                                SendDataToTcu(l == 16 ? flashBootOrder2[0] : flashBootOrder3[0]);
                                WaitAllowSendLongMsg();
                                //刷写4080个字节
                                tcuFlashLongPacketNum = 1;
                                //刷写21到2F的105个字节
                                for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                {
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 256] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 257] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 258]
                                                      + " " + TCUflashBuffer1[l*4080 + (i - 1)*7 + 259] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 260] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 261] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 262]);
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                    }
                                    else
                                    {
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                        i--;
                                    }
                                }
                                //刷写剩下的3975个字节
                                if (l < 16)
                                {
                                    for (int y = 0; y < 36; y++)
                                    {
                                        tcuFlashLongPacketNum = 0;
                                        for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                        {
                                            if (DateTime.Now > waitOneSecond)
                                            {
                                                waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                                if (y*16 + i < 567)
                                                {
                                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 361
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 362
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 363
                                                                      ]
                                                                  + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 364
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 365
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 366
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 367
                                                                      ]);
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                }
                                                //判断每一次写入的最后一行的末尾
                                                else
                                                {
                                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 361] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 362] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 363]
                                                                  + " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 364] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 365] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 366] + " AA");
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                    i = 15;
                                                }
                                            }
                                            else
                                            {
                                                i--;
                                                SendTcuFlashHeartBeat(DateTime.Now);
                                            }
                                        }
                                    }
                                    WaitTcuRtn0176();
                                }
                                else
                                {
                                    for (int y = 0; y < 2; y++)
                                    {
                                        tcuFlashLongPacketNum = 0;
                                        for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                        {
                                            if (DateTime.Now > waitOneSecond)
                                            {
                                                waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                                if (y*16 + i < 21)
                                                {
                                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 361
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 362
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 363
                                                                      ]
                                                                  + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 364
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 365] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 366] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 367
                                                                      ]);
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                }
                                                //判断文件最后写入的最后一行的末尾
                                                else
                                                {
                                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 361] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 362] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 363]
                                                                  + " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 364] + " AA AA AA");
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                    i = 16;
                                                    y = 2;
                                                    l = 16;
                                                }
                                            }
                                            else
                                            {
                                                SendTcuFlashHeartBeat(DateTime.Now);
                                                i--;
                                            }
                                        }
                                    }
                                    WaitTcuRtn0176();
                                }
                            }
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") +
                                            "                                   Proto刷写完毕");
                        }
                        if (flashTcuCount >= 23 && flashTcuCount < 29 && startFlashTcu)
                        {
                            SendTcuFlashHeartBeat(DateTime.Now);
                            SendDataToTcu(flashBootOrder4[flashTcuCount - 23]);
                            Thread.Sleep(10);
                            if (flashTcuCount == 28)
                            {
                                //MessageBox.Show("BOOT刷写完毕！");
                                TcuFlashFinish();
                                send2703 = false;
                                recv6703 = false;
                                sendTcuOrder = true;
                                sendCollectOeder = true;
                                flashTcuCount = 41;
                                //CheckTimer.Start();
                            }
                        }
                    }

                    #endregion

                    #region 刷写main

                    if (flashType == "main")
                    {
                        if (flashTcuCount >= 0 && flashTcuCount < 12 && startFlashTcu)
                        {
                            if (flashTcuCount == 0)
                            {
                                interruptTime = DateTime.Now.AddSeconds(5);
                            }
                            if (DateTime.Now > interruptTime)
                            {
                                SendKeySusses = true;
                                startFlashTcu = false;
                                tcuFlashLongPacketNum = 1;
                                TcuFlashInterrupt();
                            }
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                SendDataToTcu(flashTcuPrepareOrder[flashTcuCount]);
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 01 3E")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 A5 03")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 02 10 02")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                                if (flashTcuPrepareOrder[flashTcuCount] == "FE 01 A2")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        if (flashTcuCount >= 12 && flashTcuCount < 16 && startFlashTcu)
                        {
                            if (flashTcuCount == 12)
                            {
                                interruptTime = DateTime.Now.AddSeconds(5);
                            }
                            if (DateTime.Now > interruptTime)
                            {
                                SendKeySusses = true;
                                startFlashTcu = false;
                                tcuFlashLongPacketNum = 1;
                                TcuFlashInterrupt();
                            }
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                if (flashTcuCount == 12)
                                {
                                    keyValue = "";
                                    SendKeySusses = false;
                                    SendDataToTcu(requestSeed[flashTcuCount - 12]);
                                }
                                if (flashTcuCount == 13)
                                {
                                    if (keyValue != "NOSEED" && keyValue != "")
                                    {
                                        SendDataToTcu(requestSeed[flashTcuCount - 12] + keyValue);
                                        flashTcuOrderResponse = true;
                                        //Thread.Sleep(10);
                                    }
                                    if (keyValue == "NOSEED")
                                    {
                                        flashTcuOrderResponse = true;
                                    }
                                }
                                if (flashTcuCount == 14)
                                {
                                    SendDataToTcu(requestSeed[flashTcuCount - 12]);
                                    WaitTcuRtn0174();
                                }
                                if (flashTcuCount == 15)
                                {
                                    SendDataToTcu(requestSeed[flashTcuCount - 12] + " " +
                                                  TCUflashBuffer0[0] +
                                                  " " + TCUflashBuffer0[1] + " " + TCUflashBuffer0[2] + " " +
                                                  TCUflashBuffer0[3]);
                                }
                                /*if (requestSeed[flashTcuCount - 12] == "05 34 00 00 0C 20")
                                {
                                    flashTcuOrderResponse = true;
                                    Thread.Sleep(10);
                                }*/
                                if (requestSeed[flashTcuCount - 12].Length > 9)
                                {
                                    if (requestSeed[flashTcuCount - 12].Substring(0, 11) == "1C 26 36 00")
                                    {
                                        flashTcuOrderResponse = true;
                                        Thread.Sleep(10);
                                    }
                                }
                            }
                            tcuFlashLongPacketNum = 1;
                        }
                        DateTime waitOneSecond = new DateTime();
                        if (flashTcuCount == 16 && startFlashTcu)
                        {
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 6] +
                                                  " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 7] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 8]
                                                  + " " + TCUflashBuffer0[(i - 1)*7 + 9] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 10] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 11] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 12]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                    i--;
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            for (int y = 0; y < 27; y++)
                            {
                                for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                {
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 428)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 111] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 112] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 113]
                                                          + " " + TCUflashBuffer0[y*16*7 + (i)*7 + 114] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 115] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 116] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 117]);
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 111] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 112] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 113] + " AA AA AA AA");
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                        i--;
                                    }
                                }
                            }
                            WaitTcuRtn0176();
                        }
                        if (flashTcuCount >= 17 && flashTcuCount < 19 && startFlashTcu)
                        {
                            SendTcuFlashHeartBeat(DateTime.Now);
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                //flashTcuOrderResponse = true;
                                flashTcuOrderResponse = false;
                                if (flashTcuCount == 18)
                                {
                                    SendDataToTcu(flsahT76_ReFlash[flashTcuCount - 17] + " " +
                                                  TCUflashBuffer0[3110] +
                                                  " " + TCUflashBuffer0[3111] + " " + TCUflashBuffer0[3112] + " " +
                                                  TCUflashBuffer0[3113]);
                                    WaitAllowSendLongMsg();
                                }
                                else
                                {
                                    SendDataToTcu(flsahT76_ReFlash[flashTcuCount - 17]);
                                    WaitTcuRtn0174();
                                }
                            }
                        }
                        if (flashTcuCount == 19 && startFlashTcu)
                        {
                            SendTcuFlashHeartBeat(DateTime.Now);
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3116] +
                                                  " " + TCUflashBuffer0[(i - 1)*7 + 3117]
                                                  + " " + TCUflashBuffer0[(i - 1)*7 + 3118] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3119] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3120] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3121] + " " +
                                                  TCUflashBuffer0[(i - 1)*7 + 3122]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                    i--;
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            for (int y = 0; y < 9; y++)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y * 16*7 + i * 7 + 111));
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + y);
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + cOUNTNUM++);
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y * 16 + i));
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 131)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3221] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3222]
                                                          + " " + TCUflashBuffer0[y*16*7 + (i)*7 + 3223] +
                                                          " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3224] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3225] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3226] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3227]);
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3221] + " " +
                                                          TCUflashBuffer0[y*16*7 + (i)*7 + 3222] + " AA AA AA AA AA");
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y * 16 * 7 + i * 7 + 113));
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        i--;
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                    }
                                }
                            }
                            WaitTcuRtn0176();
                        }
                        if (flashTcuCount >= 20 && flashTcuCount < 23 && startFlashTcu)
                        {
                            if (!flashTcuOrderResponse)
                            {
                                flashTcuCount--;
                            }
                            else
                            {
                                flashTcuOrderResponse = false;
                                SendDataToTcu(flashmainOrder[flashTcuCount - 20]);
                                if (flashTcuCount == 20)
                                {
                                    Thread.Sleep(7);
                                    flashTcuOrderResponse = true;
                                }
                                if (flashTcuCount == 21)
                                {
                                    WaitTcuRtn0174();
                                }
                                if (flashTcuCount == 22)
                                {
                                    WaitAllowSendLongMsg();
                                }
                            }
                            tcuFlashLongPacketNum = 1;
                        }
                        if (flashTcuCount == 23 && startFlashTcu)
                        {
                            //刷写第一部分的256个字节
                            //刷写21到2F的105个字节
                            for (int i = tcuFlashLongPacketNum; i < 16; i++)
                            {
                                if (DateTime.Now > waitOneSecond)
                                {
                                    waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                    SendDataToTcu(tcuFlashLongPacket[i] + " " + TCUflashBuffer1[(i - 1)*7] +
                                                  " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 1] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 2]
                                                  + " " + TCUflashBuffer1[(i - 1)*7 + 3] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 4] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 5] + " " +
                                                  TCUflashBuffer1[(i - 1)*7 + 6]);
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                                else
                                {
                                    i--;
                                    SendTcuFlashHeartBeat(DateTime.Now);
                                }
                            }
                            tcuFlashLongPacketNum = 0;
                            //刷写剩下的151个字节
                            for (int y = 0; y < 2; y++)
                            {
                                for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                {
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y * 16*7 + i * 7 + 111));
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + y);
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + cOUNTNUM++);
                                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y*16+i));
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        if (y*16 + i < 21)
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 105] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 106] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 107]
                                                          + " " + TCUflashBuffer1[y*16*7 + (i)*7 + 108] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 109] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 110] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 111]);
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                        }
                                        else
                                        {
                                            SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 105] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 106] + " " +
                                                          TCUflashBuffer1[y*16*7 + (i)*7 + 107]
                                                          + " " + TCUflashBuffer1[y*16*7 + (i)*7 + 108] + " AA AA AA");
                                            SendTcuFlashHeartBeat(DateTime.Now);
                                            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " 发送指令：" + (y * 16 * 7 + i * 7 + 113));
                                            i = 15;
                                        }
                                    }
                                    else
                                    {
                                        i--;
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                    }
                                }
                            }
                            WaitTcuRtn0176();

                            //剩余部分字节刷写
                            for (int l = 0; l < 418; l++)
                            {
                                //没刷写4080个字节前需发送的指令
                                SendDataToTcu(l == 417 ? flashmainOrder1[0] : flashmainOrder2[0]);
                                WaitAllowSendLongMsg();
                                //刷写4080个字节
                                tcuFlashLongPacketNum = 1;
                                //刷写21到2F的105个字节
                                for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                {
                                    if (DateTime.Now > waitOneSecond)
                                    {
                                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                        SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 256] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 257] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 258]
                                                      + " " + TCUflashBuffer1[l*4080 + (i - 1)*7 + 259] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 260] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 261] + " " +
                                                      TCUflashBuffer1[l*4080 + (i - 1)*7 + 262]);
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                    }
                                    else
                                    {
                                        i--;
                                        SendTcuFlashHeartBeat(DateTime.Now);
                                    }
                                }
                                //刷写剩下的3975个字节
                                if (l < 417)
                                {
                                    for (int y = 0; y < 36; y++)
                                    {
                                        tcuFlashLongPacketNum = 0;
                                        for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                        {
                                            if (DateTime.Now > waitOneSecond)
                                            {
                                                waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                                if (y*16 + i < 567)
                                                {
                                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 361
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 362
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 363
                                                                      ]
                                                                  + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 364
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 365
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 366
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 367
                                                                      ]);
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                }
                                                //判断每一次写入的最后一行的末尾
                                                else
                                                {
                                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 361] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 362] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 363]
                                                                  + " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 364] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 365] +
                                                                  " " +
                                                                  TCUflashBuffer1[
                                                                      l*4080 + y*16*7 + (i)*7 + 366] + " AA");
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                    i = 15;
                                                }
                                            }
                                            else
                                            {
                                                i--;
                                                SendTcuFlashHeartBeat(DateTime.Now);
                                            }
                                        }
                                    }
                                    WaitTcuRtn0176();
                                }
                                else
                                {
                                    for (int y = 0; y < 36; y++)
                                    {
                                        tcuFlashLongPacketNum = 0;
                                        for (int i = tcuFlashLongPacketNum; i < 16; i++)
                                        {
                                            if (DateTime.Now > waitOneSecond)
                                            {
                                                waitOneSecond = DateTime.Now.AddMilliseconds(1);
                                                if (y*16 + i < 353)
                                                {
                                                    SendDataToTcu(tcuFlashLongPacket[i] + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 361
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 362
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 363
                                                                      ]
                                                                  + " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 364
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 365
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 366
                                                                      ] +
                                                                  " " +
                                                                  TCUflashBuffer1[l*4080 + y*16*7 + i*7 + 367
                                                                      ]);
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                }
                                                //判断文件最后写入的最后一行的末尾
                                                else
                                                {
                                                    //TCU.OnlySendMsg(tcuFlashLongPacket[i] + " " + TCUflashBuffer1[l * 4080 + y * 16 * 7 + (i) * 7 + 361] + " " + TCUflashBuffer1[l * 4080 + y * 16 * 7 + (i) * 7 + 362] + " " + TCUflashBuffer1[l * 4080 + y * 16 * 7 + (i) * 7 + 363]
                                                    //+ " " + TCUflashBuffer1[l * 4080 + y * 16 * 7 + (i) * 7 + 364] + " " + TCUflashBuffer1[l * 4080 + y * 16 * 7 + (i) * 7 + 365] + " " + TCUflashBuffer1[l * 4080 + y * 16 * 7 + (i) * 7 + 366] + " " + TCUflashBuffer1[l * 4080 + y * 16 * 7 + (i) * 7 + 367]);
                                                    i = 16;
                                                    y = 36;
                                                    l = 418;
                                                    SendTcuFlashHeartBeat(DateTime.Now);
                                                }
                                            }
                                            else
                                            {
                                                i--;
                                                SendTcuFlashHeartBeat(DateTime.Now);
                                            }
                                        }
                                    }
                                    WaitTcuRtn0176();
                                }
                            }
                            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") +
                                            "                                   文件1刷写完毕");
                        }
                        if (flashTcuCount == 24 && startFlashTcu)
                        {
                            TcuFlashInterface(TCUflashBuffer2);
                        }
                        if (flashTcuCount == 25 && startFlashTcu)
                        {
                            TcuFlashInterface(TCUflashBuffer3);
                        }
                        if (flashTcuCount == 26 && startFlashTcu)
                        {
                            TcuFlashInterface(TCUflashBuffer4);
                        }
                        if (flashTcuCount >= 26 && flashTcuCount < 37 && startFlashTcu)
                        {
                            if (flashTcuCount - 26 == 7)
                            {
                                var year1 = DateTime.Now.Year.ToString();
                                var month = DateTime.Now.Month.ToString();
                                var day = DateTime.Now.Day.ToString();

                                if (month.Length < 2)
                                {
                                    month = month.Insert(0, "0");
                                }
                                if (day.Length < 2)
                                {
                                    day = day.Insert(0, "0");
                                }
                                SendDataToTcu(flashmainOrder6[flashTcuCount - 26] + year1.Substring(0, 2) + " " +
                                              year1.Substring(2, 2) + " " + month + " " + day);
                                Thread.Sleep(10);
                            }
                            if (flashTcuCount - 26 == 8)
                            {
                                partNumber = Convert.ToInt32(partNumber).ToString("X8");
                                partNumber = partNumber.Substring(0, 2).Insert(2, " ") +
                                             partNumber.Substring(2, 2).Insert(2, " ") +
                                             partNumber.Substring(4, 2).Insert(2, " ") +
                                             partNumber.Substring(6, 2);
                                SendDataToTcu(flashmainOrder6[flashTcuCount - 26] + partNumber);
                                Thread.Sleep(10);
                            }
                            if (flashTcuCount - 26 != 7 && flashTcuCount - 26 != 8)
                            {
                                if (flashTcuCount - 26 == 3|| flashTcuCount - 26 == 4|| flashTcuCount - 26 == 9)
                                {
                                    Thread.Sleep(8);
                                }
                                
                                SendDataToTcu(flashmainOrder6[flashTcuCount - 26]);
                            }


                            if (flashTcuCount - 26 == 10)
                            {
                                flashTcuCount = 40;
                                TcuFlashFinish();
                                send2703 = false;
                                recv6703 = false;
                                sendTcuOrder = true;
                                sendCollectOeder = true;
                                //CheckTimer.Start();
                                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                    "                main程序刷写完成");
                            }
                        }
                    }

                    #endregion

                    flashTcuCount++;
                    if (flashTcuCount == 42)
                    {
                        startFlashTcu = false;
                        flashTcuCount = 0;
                    }
                }

                if (DateTime.Now > TCCEPCInterval && recv6703 && !sendTCCEPCOrder && !adjustSolenoid && !startFlashTcu)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                    "                100ms发送TCCEPC调节指令");
                    sendCollectOeder = false;
                    //AdjustTCCEPCPressure(TCCvalue, EPCvalue);
                    var b4 = Convert.ToString(TCCvalue*16, 16);
                    var b6 = Convert.ToString(EPCvalue*16, 16);
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
                    TCUOrderList.Add(TCCEPCOrder);
                    TCCEPCInterval = DateTime.Now.AddMilliseconds(97);
                    //sendTccEpcCycleOrder = true;
                }
                //TCU心跳(3秒)
                if (DateTime.Now > TCUHeartBeatInterval && recv6703 && !startFlashTcu)
                {
                    sendCollectOeder = false;
                    TCUHeartBeatInterval = DateTime.Now.AddMilliseconds(2500);
                    TCUOrderList.Add("01 3E");
                }
                //刷写VIN
                if (flashVinCode && sendLongMsgCount == 2)
                {
                    sendTcuOrder = true;
                }
                if (flashVinCode && sendTcuOrder)
                {
                    sendCollectOeder = false;
                    if (sendFirstLongMsg)
                    {
                        sendTcuOrder = false;
                        sendFirstLongMsg = false;
                        CAN.SendData("7E2", sendVinCode[sendLongMsgCount]);
                        sendLongMsgCount++;
                    }
                    if (allowSengLongMsg)
                    {
                        sendTcuOrder = false;
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
                if (inquireTcuInfo && sendTcuOrder)
                {
                    sendTcuOrder = false;
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
                if (getFaultCode && sendTcuOrder)
                {
                    //sendCollectOeder = false;
                    TCUOrderList.Add("03 A9 81 5A");
                    getFaultCode = false;
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                查询故障码");
                }
                //清除故障码
                if (clearFaultCode && sendTcuOrder)
                {
                    sendTcuOrder = false;
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
                    sendCollectOeder = false;
                    TCUOrderList.Add(C35RCB26Order);
                    sendC35RCB26Order = false;
                }
                if (sendTCCEPCOrder)
                {
                    sendCollectOeder = false;
                    TCUOrderList.Add(TCCEPCOrder);
                    sendTCCEPCOrder = false;
                }
                if (sendShiftSolenoidOrder)
                {
                    sendCollectOeder = false;
                    TCUOrderList.Add(shiftSolenoidOrder);
                    sendShiftSolenoidOrder = false;
                }
                if (recv6703 && sendCollectOeder && sendTcuOrder && !shiftPerformanceTest && !startFlashTcu)
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
                /*else
                {
                    if (!recv6703)
                    {
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                TCU循环查询标志位recv6703：" + recv6703);
                    }
                    if (!sendCollectOeder)
                    {
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                TCU循环查询标志位sendCollectOeder：" + sendCollectOeder);
                    }
                    if (!sendTcuOrder)
                    {
                        Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                TCU循环查询标志位sendTcuOrder：" + sendTcuOrder);
                    }
                }*/

                /*if (recv6703 && sendCollectOeder)
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
                        sendTcuOrder = true;
                    }
                }*/

                #endregion

                #region 台架指令赋值

                if (deviceDemarcate)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    foreach (var t in deviceDemarcateOrder)
                    {
                        TableOrderList.Add(t);
                    }
                }

                if (adjustInputSpeed)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(inputSpeedOrder);
                    adjustInputSpeed = false;
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                线程中输入转速：" +
                                    inputSpeedOrder);
                }

                if (adjustOutputSpeed)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(outputSpeedOrder);
                    adjustOutputSpeed = false;
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                线程中输出转速：" +
                                    outputSpeedOrder);
                }

                if (collectMcuData)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(collectMcuDataOrder);
                    collectMcuData = false;
                }

                if (disableKnob)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(disableKnobOrder);
                    disableKnob = false;
                }

                if (heatingWireControl)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(heatingWireControlOrder);
                    heatingWireControl = false;
                }

                if (oilPumpControl)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(oilPumpControlOrder);
                    oilPumpControl = false;
                }

                if (switchSolenoidControl)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(switchSolenoidControlOrder);
                    switchSolenoidControl = false;
                }

                if (tcuPowerControl)
                {
                    sendTableOrder = true;
                    sendCollectOeder = false;
                    TableOrderList.Add(tcuPowerControlOrder);
                    tcuPowerControl = false;
                }

                #endregion

                #endregion

                //发送下位机指令List中的指令
                if (sendTableOrder && sendTcuOrder && !flashVinCode && !inquireTcuInfo && !clearFaultCode &&
                    !startFlashTcu)
                {
                    if (deviceDemarcate)
                    {
                        if (DateTime.Now > deviceDemarcateInterval)
                        {
                            if (TableOrderList.Count > 0 && TCUOrderList[0].ToString().Length > 11)
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
                                sendCollectOeder = true;
                                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                "                           向下位机发送指令：" + TableOrderList[0].ToString());
                                if (adjustSpeed)
                                {
                                    liveUpdateData[6] = adjustOrderValue[4];
                                    liveUpdateData[7] = adjustOrderValue[5];
                                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                    "                           发送转速调节指令：" + adjustOrderValue[4] +
                                                    "    " + adjustOrderValue[5]);
                                    adjustSpeed = false;
                                }
                                /*if (adjustInputSpeed||adjustSpeed)
                                {
                                    liveUpdateData[6] = adjustOrderValue[4];
                                    adjustOrderValue[4]="0";
                                    adjustSpeed = false;
                                }
                                if (adjustOutputSpeed || adjustSpeed)
                                {
                                    liveUpdateData[7] = adjustOrderValue[5];
                                    adjustOrderValue[5]="0";
                                    adjustSpeed = false;
                                }*/
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
                if (sendTcuOrder && !flashVinCode && !inquireTcuInfo && !clearFaultCode && !sendTableOrder &&
                    !startFlashTcu)
                {
                    if (TCUOrderList.Count > 0)
                    {
                        sendTcuOrder = false;

                        CAN.SendData("7E2", TCUOrderList[0].ToString());
                        //发送电磁阀调节指令后，允许发送电磁阀采集指令
                        if (TCUOrderList[0].ToString().Length > 8)
                        {
                            if (TCUOrderList[0].ToString().Substring(0, 5) == "07 AE" ||
                                TCUOrderList[0].ToString().Substring(0, 8) == "04 AE 34")
                            {
                                sendCollectOeder = true;
                            }
                            //发送TCCEPC调节指令后，开始计算TCCEPC周期指令时间
                        }


                        if (TCUOrderList[0].ToString().Length > 11)
                        {
                            //将电磁阀调节值更新到数据库中
                            if (TCUOrderList[0].ToString().Substring(3, 2) == "AE" && adjustSolenoid)
                            {
                                sendAdjustTestOrder = true;
                                for (var i = 0; i < 2; i++)
                                {
                                    liveUpdateData[i + 2] = adjustOrderValue[i];
                                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                                    "                发送油压指令：" + adjustOrderValue[i]);
                                    if (i == 1)
                                    {
                                        ShowTestStep(PerformTestName(liveUpdateData[1]) + adjustOrderValue[i]);
                                    }
                                    adjustOrderValue[i] = "0";
                                }
                                adjustSolenoid = false;
                            }
                        }
                        /*if (sendTccEpcCycleOrder)
                        {
                            sendTccEpcCycleOrder = false;
                        }*/
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
                sleepTime = TthrSleepTime - DateTime.Now;
                if (sleepTime.TotalMilliseconds > 0)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                线程运行时间" +
                                    sleepTime.TotalMilliseconds);
                    Thread.Sleep((int) sleepTime.TotalMilliseconds);
                }

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
            if (liveUpdateData[0] == "false")
            {
                CAN.CanConnectStats += new CanConnectStatsEventHandler(GetCanConnectStatus);
                if (CAN.ConncetCan())
                {
                    thr_RecvData = new Thread(DataProcessThread) {IsBackground = true};
                    thr_RecvData.Priority = ThreadPriority.Highest;
                    thr_RecvData.Start();
                    //CheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(CheckHeartBeat);
                    //CheckTimer.AutoReset = true;
                    //CheckTimer.Enabled = true;
                }
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

        private void CheckHeartBeat(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!startFlashTcu)
            {
                //TCU心跳(3秒)
                if (DateTime.Now > TCUHeartBeatInterval && recv6703)
                {
                    sendCollectOeder = false;
                    TCUHeartBeatInterval = DateTime.Now.AddMilliseconds(2500);
                    SendDataToTcu("01 3E");
                }
                if (DateTime.Now > TCCEPCInterval && recv6703 && !sendTCCEPCOrder && !adjustSolenoid)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +
                                    "                100ms发送TCCEPC调节指令");
                    sendCollectOeder = false;
                    var b4 = Convert.ToString(TCCvalue*16, 16);
                    var b6 = Convert.ToString(EPCvalue*16, 16);
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
                    SendDataToTcu(TCCEPCOrder);
                    TCCEPCInterval = DateTime.Now.AddMilliseconds(97);
                }
            }
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
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入测试编号");
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
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                开始控制油泵");
            oilPumpControlOrder = status ? "03 01 ff" : "03 01 00";
            oilPumpControl = true;
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                停止控制油泵");
        }


        /// <summary>
        /// 电热丝加热开关控制
        /// </summary>
        /// <param name="status">电热丝开关</param>
        public void HeatingWireControl(bool status)
        {
            heatingWireControlOrder = status ? "03 02 ff" : "03 02 00";
            heatingWireControl = true;
        }

        /// <summary>
        /// TCU供电开关控制
        /// </summary>
        /// <param name="status">开关状态</param>
        public void TCUPowerControl(bool status)
        {
            tcuPowerControlOrder = status ? "03 03 ff" : "03 03 00";
            tcuPowerControl = true;
        }


        /// <summary>
        /// 开始/停止采集下位机数据
        /// </summary>
        /// <param name="status">true:开始 false:停止</param>
        public void CollectMcuData(bool status)
        {
            collectMcuDataOrder = status ? "03 04 FF" : "03 04 00";
            collectMcuData = true;
        }

        /// <summary>
        /// 禁用旋钮
        /// </summary>
        /// <param name="status">true:禁用 false:启用</param>
        public void DisableKnob(bool status)
        {
            disableKnobOrder = status ? "03 05 ff" : "03 05 00";
            disableKnob = true;
        }

        /// <summary>
        /// 记录并调节输入转速
        /// </summary>
        /// <param name="speed">输入转速值</param>
        public void AdjustInputSpeed(int speed)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入输入转速");
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
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入输出转速");
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

        private void SendDataToTcu(string data)
        {
            if (CAN != null)
            {
                if (data.Substring(0, 2) == "FE")
                {
                    CAN.SendData("101", data);
                }
                else
                {
                    CAN.SendData("7E2", data);
                }
            }
            
            /*if (liveUpdateData[0] == "true")
            {
                if (data.Substring(0, 2) == "FE")
                {
                    CAN.SendData("101", data);
                }
                else
                {
                    CAN.SendData("7E2", data);
                }
            }*/

        }

        /// <summary>
        /// TCU刷写接口
        /// </summary>
        /// <param name="type"></param>
        public void FlashTcu(string type)
        {
            flashTcuCount = 0;
            tcuFlashLongPacketNum = 1;
            flashTcuOrderResponse = true;
            flashType = type;
            //CheckTimer.Stop();
            startFlashTcu = true;
        }

        /// <summary>
        /// TCU刷写接口(用于主程序文件2,3,4)
        /// </summary>
        /// <param name="buffer">刷写文件</param>
        private void TcuFlashInterface(IReadOnlyList<string> buffer)
        {
            if (buffer == null) return;
            var flashCount = 0;
            int minRemaider = 0;
            bool remaiderZero = false;
            string minBuffer = "";
            while (buffer.Count - (4080*flashCount) > 0 && startFlashTcu)
            {
                SendTcuFlashHeartBeat(DateTime.Now);
                //发送刷写大小指令选择
                if (flashCount == 0)
                {
                    SendDataToTcu(flashmainOrder3[0]);
                    WaitTcuRtn0174();
                    SendDataToTcu(flashmainOrder3[1]);
                    WaitAllowSendLongMsg();
                }
                if (flashCount > 0 && flashCount < buffer.Count/4080)
                {
                    SendDataToTcu(flashmainOrder4[0]);
                    WaitAllowSendLongMsg();
                }
                if (flashCount == buffer.Count/4080)
                {
                    var remaider = buffer.Count%4080 + 6;
                    var remaiderHex = remaider.ToString("X3");
                    remaiderHex = remaiderHex.Insert(0, "1");
                    SendDataToTcu(remaiderHex.Substring(0, 2) + " " + remaiderHex.Substring(2, 2) + flashmainOrder5[0]);
                    WaitAllowSendLongMsg();
                }
                tcuFlashLongPacketNum = 1;

                var cycleCount = 0;
                if (buffer.Count - (4080*flashCount) >= 4080)
                {
                    cycleCount = 4080/7 + 1;
                    minRemaider = 6;
                }
                else
                {
                    if ((buffer.Count - (4080*flashCount))%7 == 0)
                    {
                        cycleCount = (buffer.Count - (4080*flashCount))/7;
                        remaiderZero = true;
                    }
                    else
                    {
                        cycleCount = (buffer.Count - (4080*flashCount))/7 + 1;
                        minRemaider = (buffer.Count - (4080*flashCount))%7;
                    }
                }
                DateTime waitOneSecond = new DateTime();
                for (int flash16Count = 0; flash16Count < cycleCount; flash16Count++)
                {
                    if (DateTime.Now > waitOneSecond)
                    {
                        waitOneSecond = DateTime.Now.AddMilliseconds(1);
                        //剩余字节不足7个
                        if (flash16Count == cycleCount - 1 )
                        {
                            if (!remaiderZero)
                            {
                                for (int i = 0; i < minRemaider; i++)
                                {
                                    minBuffer += " " + buffer[4080 * flashCount + flash16Count * 7 + i];
                                }
                                for (int j = (minBuffer.Length + 1) / 3; j < 7; j++)
                                {
                                    minBuffer = minBuffer + " AA";
                                }
                                SendDataToTcu(tcuFlashLongPacket[tcuFlashLongPacketNum] + minBuffer);
                                SendTcuFlashHeartBeat(DateTime.Now);
                                minBuffer = "";
                            }else
                            {
                                SendDataToTcu(tcuFlashLongPacket[tcuFlashLongPacketNum] + " " +
                                          buffer[4080 * flashCount + flash16Count * 7] + " " +
                                          buffer[4080 * flashCount + flash16Count * 7 + 1] + " " +
                                          buffer[4080 * flashCount + flash16Count * 7 + 2] + " " +
                                          buffer[4080 * flashCount + flash16Count * 7 + 3] + " " +
                                          buffer[4080 * flashCount + flash16Count * 7 + 4] + " " +
                                          buffer[4080 * flashCount + flash16Count * 7 + 5] + " " +
                                          buffer[4080 * flashCount + flash16Count * 7 + 6]);
                                SendTcuFlashHeartBeat(DateTime.Now);
                            }
                            
                            WaitTcuRtn0176();
                        }
                        else
                        {
                            SendDataToTcu(tcuFlashLongPacket[tcuFlashLongPacketNum] + " " +
                                          buffer[4080*flashCount + flash16Count*7] + " " +
                                          buffer[4080*flashCount + flash16Count*7 + 1] + " " +
                                          buffer[4080*flashCount + flash16Count*7 + 2] + " " +
                                          buffer[4080*flashCount + flash16Count*7 + 3] + " " +
                                          buffer[4080*flashCount + flash16Count*7 + 4] + " " +
                                          buffer[4080*flashCount + flash16Count*7 + 5] + " " +
                                          buffer[4080*flashCount + flash16Count*7 + 6]);
                            SendTcuFlashHeartBeat(DateTime.Now);
                        }
                        tcuFlashLongPacketNum++;
                        if (tcuFlashLongPacketNum == 16)
                        {
                            tcuFlashLongPacketNum = 0;
                        }
                        if (flash16Count == cycleCount - 1)
                        {
                            flashCount++;
                            remaiderZero = false;
                        }
                    }
                    else
                    {
                        flash16Count--;
                        SendTcuFlashHeartBeat(DateTime.Now);
                    }
                }
            }
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") +
                            "                                   文件刷写完毕");
        }

        /// <summary>
        /// 等待返回0176
        /// </summary>
        private void WaitTcuRtn0176()
        {
            var response0176 = false;
            while (!response0176)
            {
                SendTcuFlashHeartBeat(DateTime.Now);
                var data = CAN.RecviveData();
                if (data.Length != 0)
                {
                    foreach (string t in data)
                    {
                        var rtnData = t.Split(' ');
                        if (rtnData[0] + rtnData[1] == "0176")
                        {
                            response0176 = true;
                            flashTcuOrderResponse = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 等待返回0174
        /// </summary>
        private void WaitTcuRtn0174()
        {
            var response0174 = false;
            while (!response0174)
            {
                SendTcuFlashHeartBeat(DateTime.Now);
                var data = CAN.RecviveData();
                if (data.Length != 0)
                {
                    foreach (string t in data)
                    {
                        var rtnData = t.Split(' ');
                        if (rtnData[0] + rtnData[1] == "0174")
                        {
                            response0174 = true;
                            flashTcuOrderResponse = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 等待允许发送长报文
        /// </summary>
        private void WaitAllowSendLongMsg()
        {
            var allowSend = false;
            while (!allowSend)
            {
                SendTcuFlashHeartBeat(DateTime.Now);
                var data = CAN.RecviveData();
                if (data.Length != 0)
                {
                    foreach (string t in data)
                    {
                        var rtnData = t.Split(' ');
                        if (rtnData[0] + rtnData[1] == "3000")
                        {
                            allowSend = true;
                            flashTcuOrderResponse = true;
                        }
                    }
                }
            }
        }

        private string partNumber = "";

        public void GetPartNumber(string number)
        {
            partNumber = number;
        }

        /// <summary>
        /// 停止刷写TCU
        /// </summary>
        public void StopFlashTcu()
        {
            SendKeySusses = true;
            startFlashTcu = false;
            tcuFlashLongPacketNum = 1;
        }

        public void StopTest()
        {
            TCCvalue = 0;
            EPCvalue = 0;
            shiftPerformanceTest = false;
            solenoidStepTest = false;
            sendCollectOeder = true;
        }

        /// <summary>
        /// SEED计算KEY算法
        /// </summary>
        /// <param name="value"></param>
        private string Arithmetic(string value)
        {
            int seed = 0;
            int l_Value_S1 = 0;
            int l_Value_S2 = 0;
            int h_Value_S1 = 0;
            int h_Value_S2 = 0;
            int l_Value = 0;
            int h_Value = 0;
            string key = "";
            seed = Convert.ToInt32(value, 16);
            l_Value_S1 = seed & Convert.ToInt32("0xFFF0", 16);
            l_Value_S2 = (Convert.ToInt32("0x5B8", 16) - l_Value_S1/8) & Convert.ToInt32("0xFFF", 16);
            h_Value_S1 = seed & Convert.ToInt32("0xF", 16);
            l_Value = (l_Value_S2 - (h_Value_S1 + 4)/8) & Convert.ToInt32("0xFFF", 16);
            if (seed < Convert.ToInt32("0x1C5C", 16) || seed > Convert.ToInt32("0xADC3", 16))
            {
                h_Value_S2 = (Convert.ToInt32("0x9", 16) - 2*h_Value_S1) & Convert.ToInt32("0xF", 16);
            }
            if (seed > Convert.ToInt32("0x1C5B", 16) && seed < Convert.ToInt32("0x2DC4", 16))
            {
                h_Value_S2 = (Convert.ToInt32("0xB", 16) - 2*h_Value_S1) & Convert.ToInt32("0xF", 16);
            }
            if (seed > Convert.ToInt32("0x2DC3", 16) && seed < Convert.ToInt32("0xADC4", 16))
            {
                h_Value_S2 = (Convert.ToInt32("0xA", 16) - 2*h_Value_S1) & Convert.ToInt32("0xF", 16);
            }
            h_Value = (h_Value_S2 << 12) & Convert.ToInt32("0xF000", 16);
            key = (h_Value + l_Value).ToString("X");
            if (key.Length < 4)
            {
                for (int j = key.Length; j < 4; j++)
                {
                    key = key.Insert(0, "0");
                }
            }
            return key;
        }

        /// <summary>
        /// 发送TCU刷写缓冲区0数据
        /// </summary>
        /// <param name="buffer0"></param>
        public void SendTcuFlashData0(string[] buffer0)
        {
            if (buffer0 == null) return;
            TCUflashBuffer0 = new string[buffer0.Length];
            TCUflashBuffer0 = buffer0;
        }

        /// <summary>
        /// 发送TCU刷写缓冲区1数据
        /// </summary>
        /// <param name="buffer1"></param>
        public void SendTcuFlashData1(string[] buffer1)
        {
            if (buffer1 == null) return;
            TCUflashBuffer1 = new string[buffer1.Length];
            TCUflashBuffer1 = buffer1;
        }

        /// <summary>
        /// 发送TCU刷写缓冲区0数据
        /// </summary>
        /// <param name="buffer2"></param>
        public void SendTcuFlashData2(string[] buffer2)
        {
            if (buffer2 == null) return;
            TCUflashBuffer2 = new string[buffer2.Length];
            TCUflashBuffer2 = buffer2;
        }

        /// <summary>
        /// 发送TCU刷写缓冲区0数据
        /// </summary>
        /// <param name="buffer3"></param>
        public void SendTcuFlashData3(string[] buffer3)
        {
            if (buffer3 == null) return;
            TCUflashBuffer3 = new string[buffer3.Length];
            TCUflashBuffer3 = buffer3;
        }

        /// <summary>
        /// 发送TCU刷写缓冲区0数据
        /// </summary>
        /// <param name="buffer4"></param>
        public void SendTcuFlashData4(string[] buffer4)
        {
            if (buffer4 == null) return;
            TCUflashBuffer4 = new string[buffer4.Length];
            TCUflashBuffer4 = buffer4;
        }

        /// <summary>
        /// 发送TCU刷写心跳
        /// </summary>
        /// <param name="time"></param>
        private void SendTcuFlashHeartBeat(DateTime time)
        {
            if (time > flashTcuHeartBeat)
            {
                CAN.SendData("101", "FE 01 3E");
                flashTcuHeartBeat = DateTime.Now.AddMilliseconds(1995);
            }
        }

        /// <summary>
        /// 调节C1234/C456电磁阀油压
        /// </summary>
        /// <param name="data1">C1234油压</param>
        /// <param name="data2">C456油压</param>
        public void AdjustC1234C456Pressure(int data1, int data2)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                开始调节C1234C456   " +
                            data1 + "  " + data2);
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
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                开始调节C35RCB26   " + data1 +
                            "  " + data2);
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
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                开始调节TCCEPC   " + data1 +
                            "  " + data2);
            TCCvalue = data1;
            EPCvalue = data2;
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
            shiftSolenoidOrder = status ? "04 AE 34 00 3C" : "04 AE 34 00 2C";
            sendShiftSolenoidOrder = true;
        }

        /// <summary>
        /// 电磁阀一键压力控制
        /// </summary>
        /// <param name="status">压力状态 true:全压;false:无压</param>
        public void SolenoidPressureControl(bool status)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入一键油压调节");
            if (status)
            {
                TCCvalue = 2000;
                EPCvalue = 2000;
                C1234C456Order = "07 AE 3A C0 7D 00 7D 00";
                sendC1234C456Order = true;
                C35RCB26Order = "07 AE 3B C0 7D 00 7D 00";
                sendC35RCB26Order = true;
                TCCEPCOrder = "07 AE 38 C0 7D 00 7D 00";
                sendTCCEPCOrder = true;
                shiftSolenoidOrder = "04 AE 34 00 3C";
                sendShiftSolenoidOrder = true;
            }
            else
            {
                TCCvalue = 0;
                EPCvalue = 0;
                C1234C456Order = "07 AE 3A C0 00 00 00 00";
                sendC1234C456Order = true;
                C35RCB26Order = "07 AE 3B C0 00 00 00 00";
                sendC35RCB26Order = true;
                TCCEPCOrder = "07 AE 38 C0 00 00 00 00";
                sendTCCEPCOrder = true;
                shiftSolenoidOrder = "04 AE 34 00 2C";
                sendShiftSolenoidOrder = true;
            }
        }

        public void SolenoidHalfPressureControl(int data)
        {
            TCCvalue = data;
            EPCvalue = data;
            var b4 = Convert.ToString(data * 16, 16);
            var b6 = Convert.ToString(data * 16, 16);
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
            C35RCB26Order = "07 AE 3B C0 " + b4 + " " + b5 + " " + b6 + " " + b7;
            sendC35RCB26Order = true;
            TCCEPCOrder = "07 AE 38 C0 " + b4 + " " + b5 + " " + b6 + " " + b7;
            sendTCCEPCOrder = true;
            //shiftSolenoidOrder = "04 AE 34 00 3C";
            //sendShiftSolenoidOrder = true;
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
            if (data < 0)
            {
                data = -data;
            }
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入油压调节:" + data);
            if (solenoid == "TCC")
            {
                TCCvalue = data;
            }
            if (solenoid == "EPC")
            {
                EPCvalue = data;
            }
            adjustOrderValue[0] = solenoid;
            adjustOrderValue[1] = data.ToString();
            adjustOrderValue[2] = "0";
            adjustOrderValue[3] = "0";
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
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入油压调节");
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
            noFaultCode = false;
            getFaultCode = true;
        }

        /// <summary>
        /// 清除故障码
        /// </summary>
        public void ClearFaultCode()
        {
            clearFaultCode = true;
        }


        /// <summary>
        /// 阶跃测试
        /// </summary>
        /// <param name="param_Step">步长</param>
        /// <param name="param_BeginPress">起始压力</param>
        /// <param name="param_EndPress">最终压力</param>
        /// <param name="param_KeepTime">保持时间</param>
        /// <param name="param_TestEndStatus">测试结束状态</param>
        /// <param name="param_SolenoidName">电磁阀名称</param>
        /// <param name="param_TextItemNumber">测试项目编号</param>
        public void SolenoidStepTest(int param_Step, int param_BeginPress, int param_EndPress, int param_KeepTime,
            string param_TestEndStatus,
            string param_SolenoidName, string param_TextItemNumber)
        {
            beginPress = param_BeginPress;
            endPress = param_EndPress;
            keepTime = param_KeepTime;
            solenoidName = param_SolenoidName;
            //判断测试完成后状态
            if (param_TestEndStatus == "全压")
            {
                testEndStatus = 2000;
            }
            if (param_TestEndStatus == "无压")
            {
                testEndStatus = 0;
            }
            //判断步长正负
            if (beginPress > endPress)
            {
                stepSize = -param_Step;
            }
            else
            {
                stepSize = param_Step;
            }
            liveUpdateData[1] = param_TextItemNumber;
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                写入测试编号");
            SolenoidStepInverval = DateTime.Now.AddMilliseconds(param_KeepTime);
            solenoidStepTest = true;
        }

        /// <summary>
        /// 根据电磁阀名称确定要调用的指令
        /// </summary>
        /// <param name="name">电磁阀名称</param>
        /// <param name="data">电磁阀调节值</param>
        /// <returns></returns>
        private string SolenoidAdjustOrder(string name, int data)
        {
            var order = "";
            var b4 = Convert.ToString(data*16, 16);
            for (var i = b4.Length; i < 4; i++)
            {
                b4 = b4.Insert(0, "0");
            }

            var b5 = b4.Substring(2, 2);
            b4 = b4.Substring(0, 2);
            if (name == "TCC")
            {
                order = "07 AE 38 C0 " + b4 + " " + b5 + " 7D 00";
                TCCvalue = data;
                EPCvalue = 2000;
            }
            if (name == "EPC")
            {
                order = "07 AE 38 C0 " + "7D 00 " + b4 + " " + b5;
                TCCvalue = 2000;
                EPCvalue = data;
            }
            if (name == "C1234")
            {
                order = "07 AE 3A C0 " + b4 + " " + b5 + " 7D 00";
            }
            if (name == "C456")
            {
                order = "07 AE 3A C0 " + "7D 00 " + b4 + " " + b5;
            }
            if (name == "C35R")
            {
                order = "07 AE 3B C0 " + b4 + " " + b5 + " 7D 00";
            }
            if (name == "CB26")
            {
                order = "07 AE 3B C0 " + "7D 00 " + b4 + " " + b5;
            }
            if (name == "shift")
            {
                if (b4 + b5 == "3200")
                {
                    order = "04 AE 34 00 3C";
                }
                if (b4 + b5 == "0000")
                {
                    order = "04 AE 34 00 2C";
                }
                if (b4 + b5 == "7d00")
                {
                    order = "04 AE 34 00 3C";
                }
            }
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                电磁阀调节指令" +
                            order.ToUpper());
            return order.ToUpper();
        }

        /// <summary>
        /// 换档性能测试标志位
        /// </summary>
        private bool shiftPerformanceTest = false;

        /// <summary>
        /// 换档时间间隔
        /// </summary>
        private int shiftInverval = 0;

        /// <summary>
        /// 电磁阀1动作指令
        /// </summary>
        private int solenoid1_StepOrder = 0;

        /// <summary>
        /// 电磁阀1动作步长
        /// </summary>
        private int solenoid1_Step = 0;

        /// <summary>
        /// 电磁阀2动作指令
        /// </summary>
        private int solenoid2_StepOrder = 0;

        /// <summary>
        /// 电磁阀2动作步长
        /// </summary>
        private int solenoid2_Step = 0;

        /// <summary>
        /// 电磁阀1名称
        /// </summary>
        private string solenoid1_Name = "";

        // <summary>
        /// 电磁阀2名称
        /// </summary>
        private string solenoid2_Name = "";

        // <summary>
        /// 电磁阀2名称
        /// </summary>
        private string lastsolenoid_Name = "";

        /// <summary>
        /// 发送换档时间常量
        /// </summary>
        private int sendOrderTime = 0;

        /// <summary>
        /// 换档间隔时间标志位
        /// </summary>
        private bool shiftInvervalFlag = false;

        /// <summary>
        /// 发送性能测试第一条指令标志位
        /// </summary>
        private bool sendfirstOrder = false;

        /// <summary>
        /// 换档性能测试电磁阀间隔
        /// </summary>
        private int shiftSolenoidInverval = 0;

        /// <summary>
        /// 电磁阀1测试完成标志位
        /// </summary>
        private bool solenoid1TestFinish = false;

        /// <summary>
        /// 电磁阀2测试完成标志位
        /// </summary>
        private bool solenoid2TestFinish = false;

        /// <summary>
        /// 换档时间间隔
        /// </summary>
        private DateTime shiftInvervalTime = new DateTime();

        /// <summary>
        /// 性能测试指令间隔
        /// </summary>
        private DateTime sendOrderInvervalTime = new DateTime();

        /// <summary>
        /// 换档性能测试
        /// </summary>
        /// <param name="param_ShiftInverval">换档时间间隔</param>
        /// <param name="param_Solenoid1_Step">电磁阀1动作步数</param>
        /// <param name="param_Solenoid2_Step">电磁阀2动作步数</param>
        /// <param name="param_Solenoid1_Name">电磁阀1名称</param>
        /// <param name="param_Solenoid2_Name">电磁阀2名称</param>
        /// <param name="param_SendOrderTime">时间常量</param>
        public void ShiftPerformanceTest(int param_ShiftInverval, int param_Solenoid1_Step, int param_Solenoid2_Step,
            string param_Solenoid1_Name, string param_Solenoid2_Name, int param_SendOrderTime)
        {
            if (param_Solenoid1_Step*param_SendOrderTime < param_ShiftInverval)
            {
                /*SolenoidPressureControl(true);
                Thread.Sleep(1000);*/
                if (param_Solenoid2_Step != 0)
                {
                    switch (param_Solenoid2_Name)
                    {
                        case "C1234":
                            AdjustC1234C456Pressure(0, 2000);
                            break;
                        case "C456":
                            AdjustC1234C456Pressure(2000, 0);
                            break;
                        case "C35R":
                            AdjustC35RCB26Pressure(0, 2000);
                            break;
                        case "CB26":
                            AdjustC35RCB26Pressure(2000, 0);
                            break;
                    }
                }

                Thread.Sleep(1000);
                solenoid2_FinallName = param_Solenoid2_Name;
                shiftInverval = param_ShiftInverval;
                solenoid1_StepOrder = 2000;
                solenoid2_StepOrder = 9999;
                if (param_Solenoid1_Step == 0)
                {
                    solenoid1_Step = 0;
                    solenoid1TestFinish = true;
                    solenoid1_FinallName = solenoid1_Name;
                    solenoid1_StepOrder = 9999;
                    solenoid1_Name = "空";
                }
                else
                {
                    solenoid1_Step = 2000/param_Solenoid1_Step;
                    solenoid1TestFinish = false;
                    solenoid1_Name = param_Solenoid1_Name;
                    solenoid1_FinallName = solenoid1_Name;
                }
                if (param_Solenoid2_Step == 0)
                {
                    solenoid2_Step = 0;
                    solenoid2TestFinish = true;
                    solenoid2_FinallName = solenoid2_Name;
                    solenoid2_StepOrder = 9999;
                    solenoid2_Name = "空";
                }
                else
                {
                    solenoid2_Step = 2000/param_Solenoid2_Step;
                    solenoid2_Name = "空";
                    lastsolenoid_Name = param_Solenoid2_Name;
                    solenoid2TestFinish = false;
                }
                sendOrderTime = param_SendOrderTime;
                shiftSolenoidInverval = param_ShiftInverval;
                //shiftInvervalTime = DateTime.Now.AddMilliseconds(param_ShiftInverval);
                sendOrderInvervalTime = DateTime.Now;


                sendfirstOrder = false;
                shiftInvervalFlag = false;
                shiftPerformanceTest = true;
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                换档性能测试接口被调用");
            }
            else
            {
                solenoid2TestFinish = false;
                sendfirstOrder = false;
                shiftInvervalFlag = false;
                shiftPerformanceTest = false;
                AdjustShiftSolenoid(false);
                SolenoidTest(liveUpdateData[1]);
                //MessageBox.Show("换档性能参数设置不合理，第一个电磁阀调节未结束，就开始调节第二个电磁阀！");
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                换档性能测试接口参数设置错误");
            }
        }


        private string solenoid1_FinallName = "";

        private string solenoid2_FinallName = "";

        private string[] ShiftPerformOrder(string solenoid1, int data1, string solenoid2,
            int data2)
        {
            solenoid1 = solenoid1.ToUpper();
            solenoid2 = solenoid2.ToUpper();
            string[] order = new string[] {};
            if (solenoid1 != "空" && solenoid2 != "空")
            {
                if (solenoid1 == "C1234" || solenoid1 == "C456")
                {
                    if (solenoid2 == "C35R" || solenoid2 == "CB26")
                    {
                        order = new string[2] {"", ""};
                    }
                    else
                    {
                        order = new string[1] {""};
                    }
                }
                if (solenoid1 == "C35R" || solenoid1 == "CB26")
                {
                    if (solenoid2 == "C1234" || solenoid2 == "C456")
                    {
                        order = new string[2] {"", ""};
                    }
                    else
                    {
                        order = new string[1] {""};
                    }
                }
            }
            else
            {
                order = new string[1] {""};
            }
            string b4 = "";
            string b5 = "";
            string b6 = "";
            string b7 = "";
            if (data1 != 9999)
            {
                b4 = Convert.ToString(data1*16, 16);
                for (var i = b4.Length; i < 4; i++)
                {
                    b4 = b4.Insert(0, "0");
                }
                b5 = b4.Substring(2, 2);
                b4 = b4.Substring(0, 2);
            }

            if (data2 != 9999)
            {
                b6 = Convert.ToString(data2*16, 16);
                for (var i = b6.Length; i < 4; i++)
                {
                    b6 = b6.Insert(0, "0");
                }
                b7 = b6.Substring(2, 2);
                b6 = b6.Substring(0, 2);
            }
            //只有第一个电磁阀动作
            if (solenoid1 != "空" && solenoid2 == "空")
            {
                if (solenoid1 == "C1234")
                {
                    if (solenoid2_FinallName == "C456")
                    {
                        order[0] = "07 AE 3A C0 " + b4 + " " + b5 + " 00 00";
                    }
                    else
                    {
                        order[0] = "07 AE 3A C0 " + b4 + " " + b5 + " 7D 00";
                    }
                }
                if (solenoid1 == "C456")
                {
                    if (solenoid2_FinallName == "C1234")
                    {
                        order[0] = "07 AE 3A C0 " + "00 00 " + b4 + " " + b5;
                    }
                    else
                    {
                        order[0] = "07 AE 3A C0 " + "7D 00 " + b4 + " " + b5;
                    }
                }
                if (solenoid1 == "C35R")
                {
                    if (solenoid2_FinallName == "CB26")
                    {
                        order[0] = "07 AE 3B C0 " + b4 + " " + b5 + " 00 00";
                    }
                    else
                    {
                        order[0] = "07 AE 3B C0 " + b4 + " " + b5 + " 7D 00";
                    }
                }
                if (solenoid1 == "CB26")
                {
                    if (solenoid2_FinallName == "C35R")
                    {
                        order[0] = "07 AE 3B C0 " + "00 00 " + b4 + " " + b5;
                    }
                    else
                    {
                        order[0] = "07 AE 3B C0 " + "7D 00 " + b4 + " " + b5;
                    }
                }
            }
            //两个电磁阀同时动作
            if (solenoid1 != "空" && solenoid2 != "空")
            {
                if (solenoid1 == "C1234" || solenoid1 == "C456")
                {
                    if (solenoid2 == "C1234")
                    {
                        order[0] = "07 AE 3A C0 " + b6 + " " + b7 + " " + b4 + " " + b5;
                    }
                    if (solenoid2 == "C456")
                    {
                        order[0] = "07 AE 3A C0 " + b4 + " " + b5 + " " + b6 + " " + b7;
                    }
                    if (solenoid2 == "C35R")
                    {
                        if (solenoid1 == "C1234")
                        {
                            order[0] = "07 AE 3A C0 " + b4 + " " + b5 + " 7D 00";
                        }
                        if (solenoid1 == "C456")
                        {
                            order[0] = "07 AE 3A C0 7D 00 " + b4 + " " + b5;
                        }
                        order[1] = "07 AE 3B C0 " + b6 + " " + b7 + " 7D 00";
                    }
                    if (solenoid2 == "CB26")
                    {
                        if (solenoid1 == "C1234")
                        {
                            order[0] = "07 AE 3A C0 " + b4 + " " + b5 + " 7D 00";
                        }
                        if (solenoid1 == "C456")
                        {
                            order[0] = "07 AE 3A C0 7D 00 " + b4 + " " + b5;
                        }
                        order[1] = "07 AE 3B C0 7D 00 " + b6 + " " + b7;
                    }
                }
                if (solenoid1 == "C35R" || solenoid1 == "CB26")
                {
                    if (solenoid2 == "C35R")
                    {
                        order[0] = "07 AE 3B C0 " + b6 + " " + b7 + " " + b4 + " " + b5;
                    }
                    if (solenoid2 == "CB26")
                    {
                        order[0] = "07 AE 3B C0 " + b4 + " " + b5 + " " + b6 + " " + b7;
                    }
                    if (solenoid2 == "C1234")
                    {
                        if (solenoid1 == "C35R")
                        {
                            order[0] = "07 AE 3B C0 " + b4 + " " + b5 + " 00 00";
                        }
                        if (solenoid1 == "CB26")
                        {
                            order[0] = "07 AE 3B C0 7D 00 " + b4 + " " + b5;
                        }
                        order[1] = "07 AE 3A C0 " + b6 + " " + b7 + " 7D 00";
                    }
                    if (solenoid2 == "C456")
                    {
                        if (solenoid1 == "C35R")
                        {
                            order[0] = "07 AE 3B C0 " + b4 + " " + b5 + " 7D 00";
                        }
                        if (solenoid1 == "CB26")
                        {
                            order[0] = "07 AE 3B C0 7D 00 " + b4 + " " + b5;
                        }
                        order[1] = "07 AE 3A C0 7D 00 " + b6 + " " + b7;
                    }
                }
            }
            //只剩下第二个电磁阀动作
            if (solenoid1 == "空" && solenoid2 != "空")
            {
                if (solenoid2 == "C1234")
                {
                    if (solenoid1_FinallName == "C456")
                    {
                        order[0] = "07 AE 3A C0 " + b6 + " " + b7 + " 00 00";
                    }
                    else
                    {
                        order[0] = "07 AE 3A C0 " + b6 + " " + b7 + " 7D 00";
                    }
                }
                if (solenoid2 == "C456")
                {
                    if (solenoid1_FinallName == "C1234")
                    {
                        order[0] = "07 AE 3A C0 " + "00 00 " + b6 + " " + b7;
                    }
                    else
                    {
                        order[0] = "07 AE 3A C0 " + "7D 00 " + b6 + " " + b7;
                    }
                }
                if (solenoid2 == "C35R")
                {
                    if (solenoid1_FinallName == "CB26")
                    {
                        order[0] = "07 AE 3B C0 " + b6 + " " + b7 + " 00 00";
                    }
                    else
                    {
                        order[0] = "07 AE 3B C0 " + b6 + " " + b7 + " 7D 00";
                    }
                }
                if (solenoid2 == "CB26")
                {
                    if (solenoid1_FinallName == "C35R")
                    {
                        order[0] = "07 AE 3B C0 " + "00 00 " + b6 + " " + b7;
                    }
                    else
                    {
                        order[0] = "07 AE 3B C0 " + "7D 00 " + b6 + " " + b7;
                    }
                }
            }
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] == "")
                {
                    //MessageBox.Show("有值为空");
                }
            }
            return order;
        }

        /// <summary>
        /// 返回测试项目名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string PerformTestName(string data)
        {
            var testName = "";
            switch (data)
            {
                case "C00":
                    testName = "7个电磁阀同时全压/无压";
                    break;
                case "C01":
                    testName = "EPC单体测试(L_H)";
                    break;
                case "C02":
                    testName = "EPC单体测试(H_L)";
                    break;
                case "C03":
                    testName = "TCC单体测试(L_H)";
                    break;
                case "C04":
                    testName = "TCC单体测试(H_L)";
                    break;
                case "C05":
                    testName = "C1234单体测试(L_H)";
                    break;
                case "C06":
                    testName = "C1234单体测试(H_L)";
                    break;
                case "C07":
                    testName = "CB26单体测试(L_H)";
                    break;
                case "C08":
                    testName = "CB26单体测试(H_L)";
                    break;
                case "C09":
                    testName = "C35R单体测试(L_H)";
                    break;
                case "C10":
                    testName = "C35R单体测试(H_L)";
                    break;
                case "C11":
                    testName = "C456单体测试(L_H)";
                    break;
                case "C12":
                    testName = "C456单体测试(H_L)";
                    break;
                case "C13":
                    testName = "换档单体测试(L_H)";
                    break;
                case "C14":
                    testName = "换档单体测试(H_L)";
                    break;
                case "C15":
                    testName = "EPC阶跃测试(L_H)";
                    break;
                case "C16":
                    testName = "EPC阶跃测试(H_L)";
                    break;
                case "C17":
                    testName = "TCC阶跃测试(L_H)";
                    break;
                case "C18":
                    testName = "TCC阶跃测试(H_L)";
                    break;
                case "C19":
                    testName = "C1234阶跃测试(L_H)";
                    break;
                case "C20":
                    testName = "C1234阶跃测试(H_L)";
                    break;
                case "C21":
                    testName = "CB26阶跃测试(L_H)";
                    break;
                case "C22":
                    testName = "CB26阶跃测试(H_L)";
                    break;
                case "C23":
                    testName = "C35R阶跃测试(L_H)";
                    break;
                case "C24":
                    testName = "C35R阶跃测试(H_L)";
                    break;
                case "C25":
                    testName = "C456阶跃测试(L_H)";
                    break;
                case "C26":
                    testName = "C456阶跃测试(H_L)";
                    break;
                case "D00":
                    testName = "D1-D2换档性能测试";
                    break;
                case "D01":
                    testName = "D2-D3换档性能测试";
                    break;
                case "D02":
                    testName = "D3-D4换档性能测试";
                    break;
                case "D03":
                    testName = "D4-D5换档性能测试";
                    break;
                case "D04":
                    testName = "D5-D6换档性能测试";
                    break;
                case "D05":
                    testName = "D6-D5换档性能测试";
                    break;
                case "D06":
                    testName = "D5-D4换档性能测试";
                    break;
                case "D07":
                    testName = "D4-D3换档性能测试";
                    break;
                case "D08":
                    testName = "D3-D2换档性能测试";
                    break;
                case "D09":
                    testName = "D2-D1换档性能测试";
                    break;
            }
            return testName + ":";
        }

        #endregion

        #endregion

        #region 实现接口

        /// <summary>
        /// 注册对象修改标志位
        /// </summary>
        private bool operatObserver = false;

        public void RegisterObserver(Observer o)
        {
            operatObserver = true;
            observers.Add(o);
            operatObserver = false;
        }

        public void RemoveObserver(Observer o)
        {
            //throw new NotImplementedException();
            int i = observers.IndexOf(o);
            if (i >= 0)
            {
                operatObserver = true;
                observers.RemoveAt(i);
                operatObserver = false;
            }
        }

        public void NotifyObservers()
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "                           向界面更新数据");
            if (observers == null) return;
            /*foreach (var t in observers)
            {
                var observer = (Observer) t;
                observer.Update(updateData);
            }*/
            //注册对象被修改时等待
            while (operatObserver)
            {
            }
            for (int i = 0; i < observers.Count; i++)
            {
                var observer = (Observer) observers[i];
                observer.Update(updateData);
            }
        }

        #endregion
    }
}