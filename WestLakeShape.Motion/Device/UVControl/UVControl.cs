﻿
using System.ComponentModel;
using System.Threading.Tasks;
using static WestLakeShape.Motion.IOStateSource;

namespace WestLakeShape.Motion.Device
{
    public class UVControl
    {
        private UVControlConfig _config;
        //private IOState _io;
        private UVControlPort _port;
        private readonly byte Slave_ID = 1;
        private bool _isAuto;

        public UVControl(UVControlConfig config)
        {
            _config = config;
            _port = new UVControlPort(config.PortName);
            
        }

        public void OnConnecting()
        {
            _port.Connected();
            _port.WriteSingleRegister(00, 0x9C42, 0x0001);
        }
        public void OnDisconnecting()
        {
            _port.Disconnected();
        }

        public void StartPreheat()
        {
            //await Task.Delay(_config.PreheatTime).ConfigureAwait(false);
        }

        /// <summary>
        /// 切换工作模式
        /// </summary>
        public void SwitchWorkModel()
        {
            var flag = _isAuto ? 0x0000 : 0x0001;
            _port.WriteSingleRegister(Slave_ID, 0x9c43, flag);
            _isAuto = !_isAuto;
        }

        public void TrunOn()
        {
            _port.WriteSingleRegister(Slave_ID, 0x9C41, (int)ChannelNo.First);
        }
        public void TurnOff()
        {
            _port.WriteSingleRegister(Slave_ID, 0x9C41, (int)ChannelNo.First*2);
        }
        public void TrunOnAll()
        {
            _port.WriteSingleRegister(Slave_ID, 0x9C41, (int)ChannelNo.All);
        }
        public void TurnOffAll()
        {
            _port.WriteSingleRegister(Slave_ID, 0x9C41, (int)ChannelNo.All * 2);
        }

        public void TurnOnBuzzer()
        {
            _port.WriteSingleRegister(Slave_ID, 0x9c44, 0x0001);
        }
        public void TurnOffBuzzer()
        {
            _port.WriteSingleRegister(Slave_ID, 0x9c44, 0x0000);
        }

        //public async Task<bool> UVExpose()
        //{
        //    _io.Set(true);
        //    await Task.Delay(_config.ExposureTime).ConfigureAwait(false);
        //    _io.Set(false);
        //    return true;
        //}

        enum ChannelNo
        {
            First = 0x0100,
            Second =0x1000,
            Third = 0x0001,
            Four = 0x0010,
            All = 0x1111,
        }
    }


    public class UVControlConfig
    {
        public string PortName { get; set; }

        [Category("PrintPlatform"), Description("UV灯保压时间，单位为毫秒")]
        [DisplayName("保压时间")]
        public int PreheatTime { get; set; }

        [Category("PrintPlatform"), Description("掩膜预压印高度.,单位为毫秒")]
        [DisplayName("UV曝光时间")]
        public int ExposureTime { get; set; }
    }
}