using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WestLakeShape.Motion.Device
{
    public class GlueControlPort
    {
        private readonly ushort Head_Length = 1;
        private readonly ushort Tail_Length = 2;
        private readonly byte Slave_ID = 1;
        private ManualResetEventSlim _signal = new ManualResetEventSlim(false);
        private SerialPort _port;
        private byte[] _receiveDataBuff;


        public bool IsConnected => _port.IsOpen;
        
        public string Name
        {
            get => _port.PortName;
            set
            {
                Disconnected();
                _port.PortName = value;
                Connected();
            }
        }

        public GlueControlPort(string name)
        {
            _port = new SerialPort()
            {
                PortName = name,
                BaudRate = 115200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };
            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }



        public void Connected()
        {
            if (!_port.IsOpen)
                _port.Open();
        }

        public void Disconnected()
        {
            if (_port.IsOpen)
                _port.Close();
        }


        public byte[] WriteSingleRegister(byte slaveId, ushort address, ushort value)
        {
            var request = new byte[Head_Length + 5 + Tail_Length]; ;
            request[Head_Length + 0] = FunctionCodes.WriteSingleRegister;
            request[Head_Length + 1] = (byte)(address >> 8);
            request[Head_Length + 2] = (byte)(address & 0xff);
            request[Head_Length + 3] = (byte)(value >> 8);
            request[Head_Length + 4] = (byte)(value & 0xff);

            return SendData(request);
        }


        private byte[] SendData(byte[] data)
        {
            _signal.Reset();
            _receiveDataBuff = null;

            UpdateCrc16(data);

            _port.Write(data, 0, data.Length);
            
            if (!_signal.Wait(5000))
                throw new Exception($"{_port.PortName}通信超时");
            
            return _receiveDataBuff;
        }


        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var buff = new byte[256];
            var start = 0;
            bool needReceive = true;
            while (needReceive)
            {
                var size = _port.BytesToRead;
                _port.Read(buff, start, size);
                if (size == 0)
                    break;
                start += size;

                //获取单个消息
                needReceive = TryTakeMessage(buff);
                if (!needReceive)
                    _signal.Set();
            }
        }



        /// <summary>
        /// 试图从缓冲区取出一条完整的回复消息
        /// </summary>
        private bool TryTakeMessage(byte[] buffer)
        {
            var offset = Head_Length;
            var tail = Tail_Length;

            var minLength = offset + tail + 1;
            if (buffer.Length < minLength)
                return false;

            var functionCode = buffer[offset];
            int messageLength;
            if (functionCode < 0x80)
            {
                // No exception
                switch (functionCode)
                {
                    case FunctionCodes.ReadInputRegisters:
                        {
                            // 0: Function Code, 1: Byte Count, 2+ Data
                            var byteCount = buffer[offset + 1];
                            messageLength = offset + 2 + byteCount + tail;
                        }
                        break;
                    case FunctionCodes.WriteSingleCoil:
                            // 0: Function Code, 1-2: Register Address, 3-4: Data Value
                            messageLength = offset + 5 + tail;
                        break;
                    case FunctionCodes.WriteRegisters:
                            // 0: Function Code, 1-2: Start Address, 3-4: Data Count
                            messageLength = offset + 5 + tail;
                        break;
                    default:
                        throw new UnexpectedValueException(functionCode);
                }
            }
            else
            {
                throw new UnexpectedValueException(functionCode);
            }

            if (0 < messageLength && messageLength == buffer.Length)
            {
                _receiveDataBuff = new byte[messageLength];
                Array.Copy(buffer, 0, _receiveDataBuff, 0, messageLength);

                if (!ValidateCrc16(_receiveDataBuff))
                    throw new CrcValidateException();
                
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算奇偶校验，并更新到消息内容最后两个字节
        /// </summary>
        /// <param name="request">Modbus消息内容</param>
        private void UpdateCrc16(byte[] request)
        {
            var tmp = ComputeCrc16(request, 0, request.Length - 2);
            request[request.Length - 1] = (byte)(tmp >> 8);
            request[request.Length - 2] = (byte)(tmp & 0xff);
        }


        /// <summary>
        /// 进行 CRC 奇偶校验
        /// </summary>
        private bool ValidateCrc16(byte[] message)
        {
            var tmp = ComputeCrc16(message, 0, message.Length - 2);

            return message[message.Length - 1] == (byte)(tmp >> 8) &&
                message[message.Length - 2] == (byte)(tmp & 0x00ff);
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
