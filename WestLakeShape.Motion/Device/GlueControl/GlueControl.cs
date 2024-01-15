using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WestLakeShape.Motion.Device
{
    public class GlueControl
    {
        private GlueControlPort _port;
        private const byte Slave_ID = 1;
        private GlueControlConfig _config;

        /// <summary>
        /// 已点胶次数
        /// </summary>
        public int PointsCount { get; set; }

        /// <summary>
        /// 公式计算
        /// </summary>
        public int GlueCycle { get; set; }


        public GlueControlConfig Config
        {
            get => _config;
            set => _config = value;
        }


        public GlueControl(GlueControlConfig config)
        {
            _port = new GlueControlPort(config.PortName);
        }

        public bool Connect()
        {
            _port.Connected();
            return true;
        }

        public bool Disconnected()
        {
            _port.Disconnected();
            return true;
        }

        /// <summary>
        /// 开始点胶
        /// </summary>
        /// <returns></returns>
        public bool StartDispense()
        {
            WriteCommand(RegisterNo.StartDispense, CommandValue.Start_Dispense);
            return true;
        }

        /// <summary>
        /// 停止点胶
        /// </summary>
        /// <returns></returns>
        public bool StopDispense()
        {
             WriteCommand(RegisterNo.StartDispense, CommandValue.Stop_Dispense);
            return true;
        }

        /// <summary>
        /// 保存参数指令
        /// </summary>
        /// <returns></returns>
        public bool SaveParam()
        {
            WriteCommand(RegisterNo.SaveParamter, CommandValue.Save_Param);
            return true;
        }

        /// <summary>
        /// 点胶延时
        /// </summary>
        /// <returns></returns>
        public bool WriteDispensingDeleyTime()
        {
            WriteParamValue(RegisterNo.DispensingDelayTime, _config.DispensingDelayTime);
            return true;
        }



        private  bool WriteCommand(RegisterNo registerNo, ushort command)
        {
            _port.WriteSingleRegister(Slave_ID, (ushort)registerNo, command);
            return true;
        }
        private bool WriteParamValue(RegisterNo registerNo, int val)
        {
            _port.WriteSingleRegister(Slave_ID, (ushort)registerNo, (ushort)val);
            return true;
        }


        /// <summary>
        /// 写入单个参数
        /// </summary>
        /// <param name="registerName"></param>
        /// <param name="registerValue"></param>
        /// <returns></returns>
        public bool DownloadParameter(string registerName, int registerValue)
        {
            if (Enum.IsDefined(typeof(RegisterNo), registerName))
            {
                var registerNo = (RegisterNo)Enum.Parse(typeof(RegisterNo), registerName, true);
                WriteParamValue(registerNo, registerValue);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 写入所有参数
        /// </summary>
        /// <returns></returns>
        public bool DownloadAllParameter()
        {
            Type configType = _config.GetType();
            PropertyInfo[] properties = configType.GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                if (Enum.IsDefined(typeof(RegisterNo), propertyName))
                {
                    var registerNo = (RegisterNo)Enum.Parse(typeof(RegisterNo), propertyName, true);
                    var registerValue = (int)property.GetValue(_config);
                    WriteParamValue(registerNo, registerValue);
                }
            }
            return true;
        }


        //public async void Refresh()
        //{
        //    WriteCommand(_config.SlaveID, 0x2102, 2);
        //    WriteCommand(_config.SlaveID, 0x2101, 1);
        //    WriteCommand(_config.SlaveID, 0x220A, 1);
        //}


        enum RegisterNo : ushort
        {
            SaveParamter = 0x2008,
            ModbusAddress,
            ModbusBaudrate,
            StartDispense,
            //OpenValveIntensity=0x2100,重复功能寄存器，找供应商
            Cycle = 0x2101,
            HighGlueCount,
            LowerGlueCount,

            DownTime = 0x2200,
            OpenValveTime,
            UpTime,
            ClosedValveTime,
            OpenValveIntensity,
            ControlModel,
            GluePoints,
            DispensingDelayTime,
            TargetTemperatore
        }

        static class CommandValue
        {
            public static ushort Save_Param = 1;

            public static ushort Start_Dispense = 1;
            public static ushort Stop_Dispense = 0;

            public static ushort Line_Model = 0;
            public static ushort Point_Model = 1;
            public static ushort Heating_Enable = 1;
            public static ushort Heating_Disable = 0;
        }
    }

    public class GlueControlConfig
    {
        [Category("GlueControl"), Description("Comm端口号"), DefaultValue(1)]
        [DisplayName("PortName")]
        public string PortName { get; set; }

        [Category("GlueControl"), Description("从站地址"), DefaultValue(1)]
        [DisplayName("SlaveID")]
        public byte SlaveID { get; set; }

        [Category("GlueControl"), Description("下降时间，单位10us"), DefaultValue(10)]
        [DisplayName("SlaveConfig")]
        public ModbusHubConfig SlaveConfig { get; set; }


        [Category("GlueControl"), Description("下降时间，单位10us"), DefaultValue(10)]
        [DisplayName("DownTime")]
        public int DownTime { get; set; }

        [Category("GlueControl"), Description("开阀时间，单位10us"), DefaultValue(30)]
        [DisplayName("OpenValveTime")]
        public int OpenValveTime { get; set; }


        [Category("GlueControl"), Description("上升时间，单位10us"), DefaultValue(20)]
        [DisplayName("UpTime")]
        public int UpTime { get; set; }

        [Category("GlueControl"), Description("关阀时间，单位10us"), DefaultValue(30)]
        [DisplayName("ClosedValveTime")]
        public int ClosedValveTime { get; set; }

        [Category("GlueControl"), Description("开阀力度，单位%")]
        [DisplayName("OpenValveIntensity")]
        public int OpenValveIntensity { get; set; }

        [Category("GlueControl"), Description("点胶模式")]
        [DisplayName("ControlModel")]
        public int ControlModel { get; set; }

        [Category("GlueControl"), Description("点数计算，点模式生效"), DefaultValue(1)]
        [DisplayName("GluePoints")]
        public int GluePoints { get; set; }

        [Category("GlueControl"), Description("点胶延时，单位ms")]
        [DisplayName("DispensingDelayTime")]
        public int DispensingDelayTime { get; set; }

        [Category("GlueControl"), Description("温度设定"), DefaultValue(25)]
        [DisplayName("TargetTemperatore")]
        public int TargetTemperatore { get; set; }
    }
}
