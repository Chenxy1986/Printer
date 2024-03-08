using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WestLakeShape.Common.WpfCommon;

namespace WestLakeShape.Motion.Device
{
    /// <summary>
    /// 执行动作的压电陶瓷机构
    /// 闭环可以读取到位移，开环读取的电压
    /// </summary>
    public class PiezoActuator
    {
        private bool[] _isClosedLoop;          //开环读取的是电压，闭环读取的是位移
        private PiezoActuatorConfig _config;
        private PiezoSerialPort _piezoPort;

        private readonly int Closed_Loop_Flag = 67;
        private readonly int Opened_Loop_Flag = 79;
        public bool IsConnected => _piezoPort.IsConnected;

        public bool this[int i]
        {
            get => _isClosedLoop[i];
            set => _isClosedLoop[i] = value;
        }

        public PiezoActuator(PiezoActuatorConfig config)
        {
            _config = config;
            _piezoPort = new PiezoSerialPort(config.PortName);
            _isClosedLoop = new bool[3];
        }

        public void Connect()
        {
            _piezoPort.Connected();
            ReadClosedLoopFlag();
        }

        public void Disconnected()
        {
            _piezoPort.Disconnected();
        }

        public void ReloadConfig()
        {
            _piezoPort.PortName = _config.PortName;
        }

        public void EnableClosedLoop(int channelNo)
        {
            var openChannels = ClassifyChannels(false);
            openChannels.ForEach(index => 
            {
                _piezoPort.WriteCommand(B3Commands.EnableClosedLoop, new int[] { index, Closed_Loop_Flag });
                _isClosedLoop[index] = true;
            });
        }
        
        public void DisableClosedLoop()
        {
            var closedChannels = ClassifyChannels(true);
            closedChannels.ForEach(index =>
            {
                _piezoPort.WriteCommand(B3Commands.EnableClosedLoop, new int[] { index, Opened_Loop_Flag });
                _isClosedLoop[index] = false;
            });
        }

        /// <summary>
        /// 写单路位移
        /// </summary>
        /// <param name="no"></param>
        /// <param name="disp"></param>
        public void WriteDisplace(ChannelNo no,double disp)
        {
            _piezoPort.WriteData(B3Commands.WriteSingleChannelDisp, (int)no, new double[] { disp });    
        }

        /// <summary>
        /// 写多路位移
        /// </summary>
        /// <param name="disp"></param>
        public void WriteMultiDisplace(double[] disp)
        {
            _piezoPort.WriteData(B3Commands.WriteMultiChannelsDisp, (int)ChannelNo.One, disp);
        }

        /// <summary>
        /// 读单路位移
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public double ReadDisplace(ChannelNo no)
        {
            _piezoPort.WriteCommand(B3Commands.ReadSingleChannelDisp, new int[] { (int)no });

            var temp = _piezoPort.ReceiveValues(B3Commands.ReadSingleChannelDisp, (int)no, 1);
           
            return temp[0];
        }

        /// <summary>
        /// 读多路位移
        /// </summary>
        /// <param name="disp"></param>
        /// <returns></returns>
        public double[] ReadMultiDisplace()
        {
            _piezoPort.WriteCommand(B3Commands.ReadMultiChannelDispOrV);
            var temp = _piezoPort.ReceiveValues(B3Commands.ReadMultiChannelDispOrV, 0, 3);
            return temp;
        }

        /// <summary>
        /// 标定载荷
        /// </summary>
        /// <param name="loadValue"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public bool LoadCalib(ChannelNo no,double loadValue)
        {
            _piezoPort.WriteData(B3Commands.LoadCalib, (int)no, new double[] { loadValue });
            return true;
        }

        /// <summary>
        /// 设定位移的限位
        /// </summary>
        /// <param name="no"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool SetDisplaceSoftLimit(ChannelNo no,double position)
        {
            _piezoPort.WriteData(B3Commands.DisplaceSoftLimit,(int)no,new double[] {position});
            return true;
        }

        /// <summary>
        /// 读取位移的限位
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public double ReadDisplaceSoftLimt(ChannelNo no)
        {
            _piezoPort.WriteCommand(B3Commands.DisplaceSoftLimit, new int[] { (int)no });

            //var temp = _piezoPort.ReceiveMessage(B3Commands.DisplaceSoftLimit, (int)no, 1,false);
            //return temp[0];
            return 0;
        }

        /// <summary>
        /// 清零多路
        /// </summary>
        /// <returns></returns>
        public bool ClearMultiChannel()
        {
            _piezoPort.WriteCommand(B3Commands.ClearMultiChannels);
            return true;
        }

        /// <summary>
        /// 读取通道的开闭环状态
        /// </summary>
        /// <returns></returns>
        private bool ReadClosedLoopFlag()
        {
            var channels = ((ChannelNo[])Enum.GetValues(typeof(ChannelNo))).ToList();
            channels.ForEach(index =>
            {
                //发送数据
                _piezoPort.WriteCommand(B3Commands.ReadClosedLoopFlag, new int[] { (int)index });
                //读取数据
                var temp = _piezoPort.ReceiveFlag(B3Commands.ReadClosedLoopFlag, (int)index, 1);
                //数据转化
                _isClosedLoop[(int)index] = temp[0] == Closed_Loop_Flag ? true : false;

                Thread.Sleep(100);
            });

            return true;
        }

        /// <summary>
        /// 根据开闭环进行通道的分类
        /// </summary>
        /// <param name="isClosedLoop"></param>
        /// <returns></returns>
        private List<int> ClassifyChannels(bool isClosedLoop)
        {
            return _isClosedLoop.Select((value, index) => new { Value = value, Index = index })
                                .Where(item => item.Value == isClosedLoop)
                                .Select(item => item.Index)
                                .ToList();
        }
    }


    public class PiezoActuatorConfig : NotifyPropertyChanged
    {
        private string _name;
        public string PortName
        {
            get => _name??"COM1";
            set => SetProperty(ref _name, value);
        }
    }


    public enum ChannelNo : byte
    {
        One = 0,
        Two = 1,
        Third = 2,
    }


    public enum B3Commands
    {
        //写
        WriteSingleChannelV = 0,
        WriteSingleChannelDisp = 1,
        WriteMultiChannelsV = 2,
        WriteMultiChannelsDisp = 3,
        ClearMultiChannels = 4,

        //读
        ReadSingleChannelV = 5,
        ReadSingleChannelDisp = 6,

        ReadSingleChannelRealtimeV = 7,
        ReadSingleChannelRealtimeDisp = 8,
        ReadMultiChannelsRealtimeV = 9,
        ReadMultiChannelsRealtimeDisp = 10,
        StopReadRealtime = 11,

        EnableClosedLoop = 18,
        ReadClosedLoopFlag = 19,

        LoadCalib =24,

        DisplaceSoftLimit =26,
        //自检类
        StartSelfCheck = 39,
        ReadSelfCheckCode = 40,

        ReadMultiChannelsRealtimeDataH = 48,
        ReadMultiChannelRealtimeDataD = 49,

        ReadMultiChannelDispOrV=51,

    }
}
