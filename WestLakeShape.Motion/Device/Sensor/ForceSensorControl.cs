using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestLakeShape.Motion.Device
{
    public class ForceSensorControl
    {
        private readonly byte Slave_ID = 1;
        private readonly int Sensor_Register_Count = 4;
        private readonly int Sensor_Count = 3;
        private SerialPort _port;
        private bool _isConnected;
        private Stream _stream;
        private List<double> _forceValues;

        public double ForceValue0
        {
            get => _forceValues[0];
            private set => _forceValues[0] = value;
        }
        public double ForceValue1
        {
            get => _forceValues[1];
            private set => _forceValues[1] = value;
        }
        public double ForceValue2
        {
            get => _forceValues[2];
            private set => _forceValues[2] = value;
        }
        public double ForceValue3
        {
            get => _forceValues[3];
            private set => _forceValues[3] = value;
        }



        public ForceSensorControl(string name)
        {
            _port = new SerialPort()
            {
                PortName = name,
                BaudRate = 19200,
                DataBits = 8,
                StopBits = StopBits.Two,
                Parity = Parity.None,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
            _forceValues = new List<double> 
            {
                0.1,
                0.2,
                0.3,
                0.4
            };
        }
        public void Connected()
        {
            if (!_isConnected)
                _port.Open();
            _isConnected = true;
            _stream = _port.BaseStream;
        }


        public void Disconnected()
        {
            if (_isConnected)
                _port.Close();

            _isConnected = false;
        }


        public double[] RefreshValues()
        {
            var buff = new byte[8];
            CreatRequest(buff);

            try
            {
                //发送数据
                _stream.Write(buff, 0, buff.Length);
                //接受数据
                var receData = new byte[ 5 + buff[5] * 2];
                _stream.Read(receData, 0, receData.Length);

                //数据分析
                var data = new byte[Sensor_Count * Sensor_Register_Count];
                Array.Copy(receData,3, data, 0, data.Length);

                //转化成double保存在_forceValues
                ConvertToDouble(data);
                return _forceValues.ToArray();

            }
            catch (Exception ex) 
            {
                throw ex;
            }
         
        }

        /// <summary>
        /// bytes转化成data（double）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public void ConvertToDouble(byte[] bytes)
        {
            var datas = new List<double>();
            ///4个byte为一通道数据
            ///不到两个为其他数据
            if (bytes.Length % 4 == 0)
            {
                for (var i = 0; i < bytes.Length / 4; i += 4)
                {
                    double k = (bytes[0] & 0x80) == 0x80 ? -1 : 1;
                    var temp = (double)(bytes[0] * 256 + bytes[1] + (bytes[2] * 256 + bytes[3]) * 0.0001);
                    _forceValues.Add(k * temp);
                }
            }
            else
            {
                for (var i = 0; i < bytes.Length; i++)
                    _forceValues.Add((double)bytes[i]);
            }
           
        }


        private void CreatRequest(byte[] request)
        {
            request[0] = Slave_ID;
            request[1] = 0x03;
            request[2] = 0x01;
            request[3] = 0xA4;
            request[4] = 0x00;
            request[5] = (byte)(Sensor_Count * Sensor_Register_Count);
            UpdateCrc16(request);
        }

        private void UpdateCrc16(byte[] request)
        {
            var tmp = ComputeCrc16(request, 0, request.Length - 2);
            request[request.Length - 1] = (byte)(tmp >> 8);
            request[request.Length - 2] = (byte)(tmp & 0xff);
        }
        private static ushort ComputeCrc16(byte[] data, int offset, int count)
        {
            ushort tmp = 0xffff;

            for (var n = 0; n < count; n++)
            {
                tmp = (ushort)((data[offset + n]) ^ tmp);
                for (var i = 0; i < 8; i++)
                {
                    if ((tmp & 0x01) > 0)
                    {
                        tmp = (ushort)(tmp >> 1);
                        tmp = (ushort)(tmp ^ 0xa001);
                    }
                    else
                    {
                        tmp = (ushort)(tmp >> 1);
                    }
                }
            }

            return tmp;
        }
    }
}
