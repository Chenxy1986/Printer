using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestLakeShape.Motion
{
    public class UVControlPort
    {
        private SerialPort _port;
        private bool _isConnected;
        private Stream _stream;

        public string PortName
        {
            get => _port.PortName;
            set => _port.PortName = value;
        }

        public UVControlPort(string name)
        {
            _port = new SerialPort()
            {
                PortName = name,
                BaudRate = 9800,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                ReadTimeout = 5000,
                WriteTimeout = 5000
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


        public bool WriteSingleRegister(byte slaveId,int address,int value)
        {
            var values = new byte[] {
                (byte)((value >> 8) & 0xff),
                (byte)(value & 0xff)
            };   
           
            var buff = CreatePdu(slaveId, FunctionCodes.WriteSingleRegister, address, values);
                   //寄存器数值低位
            _stream.Write(buff, 0, buff.Length);
            return true;
        }

        public bool WriteMultiRegister(byte slaveId, int address, byte[] values)
        {
            var buff = CreatePdu(slaveId, FunctionCodes.WriteSingleRegister, address, values);
            //寄存器数值低位
            _stream.Write(buff, 0, buff.Length);
            return true;
        }

        private byte[] CreatePdu(byte slaveId, byte command, int address,byte[] values)
        {
            var buff = new byte[6];

            buff[0] = slaveId;                         //从站地址
            buff[1] = command;                         //命令
            buff[2] = (byte)((address >> 8) & 0xff);   //寄存器地址高位
            buff[3] = (byte)(address & 0xff);          //寄存器地址低位
            Array.Copy(values, buff, values.Length);

            return buff;
        }


    }
}
