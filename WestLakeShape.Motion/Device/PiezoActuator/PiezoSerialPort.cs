using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestLakeShape.Motion.Device
{
    /// <summary>
    /// 对供应商的类做了新的改进
    /// 测试后改成异步
    /// </summary>
    public class PiezoSerialPort
    {
        private SerialPort _port;
        private bool _isConnected;
        private Stream _stream;

        private readonly byte Start_Bytes = 0xAA;
        private readonly byte Mcu_Address = 1;
        private readonly byte B4_Alt_Command = 0;
        private readonly int Pdu_Offset = 5;
       
        public bool IsConnected => _isConnected;

        public string PortName
        {
            get => _port.PortName;
            set => _port.PortName = value;
        }
        public PiezoSerialPort(string name)
        {
            _port = new SerialPort()
            {
                PortName = name,
                BaudRate = 9600,
                DataBits = 8,
                StopBits =StopBits.One,
                Parity = Parity.None,
                ReadTimeout=5000,
                WriteTimeout =5000
            };
        }


        public void Connected()
        {
            try
            {
                if (!_isConnected)
                    _port.Open();
                _isConnected = true;
                _stream = _port.BaseStream;

            }
            catch (Exception e)
            {
                throw new Exception($"微动平台连接失败，{e.Message}");
            }
        }
        

        public void Disconnected()
        {
            if (_isConnected)
                _port.Close();

            _isConnected = false;
        }


        public bool WriteData(B3Commands command, int channelNo, double[] values)
        {
            var buff = CreatePdu(command, channelNo, values);
            
            _stream.Write(buff, 0, buff.Length);
            
            return true;
        }

        /// <summary>
        /// 发送数据；将通道数放入到data中
        /// </summary>
        /// <param name="command"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool WriteCommand(B3Commands command,int[] values= null)
        {
            int size =0 ;
            if (values != null)
                size = values.Length;

            var buff = new byte[5 + size + 1];
            buff[0] = Start_Bytes;          //起始字节
            buff[1] = Mcu_Address;          //地址
            buff[2] = (byte)buff.Length;    //包长
            buff[3] = (byte)command;        //B3命令
            buff[4] = (byte)B4_Alt_Command; //B4命令

            if (values != null)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    buff[5 + i] = (byte)values[i];
                }
            }

            buff[buff.Length - 1] = ParityCheck(buff);

            _stream.Write(buff, 0, buff.Length);
            
            return true;
        }


        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="command"></param>
        /// <param name="channelNo"></param>
        /// <param name="channelNum"></param>
        /// <returns></returns>
        public double[] ReceiveValues(B3Commands command, int channelNo,int channelNum)
        {
            try
            {
                //后期是否考虑对比command，校验位，保证信息的完整性
                //可通过消息的第三个字节获取到消息长度。
                _port.DiscardInBuffer();
                var size = channelNum * 4;

                var buff = new byte[5 + size + 1];

                _stream.Read(buff, 0, buff.Length);

                ///拷贝数据到数组
                var bytes = new byte[size];
                Array.Copy(buff, Pdu_Offset, bytes, 0, size);

                return ConvertToDouble(bytes).ToArray();
            }
            catch (Exception e)
            {
                throw e;  
            }
        }

        public byte ReceiveFlag(B3Commands command, int channelNo, int channelNum)
        {
            try
            {
                //后期是否考虑对比command，校验位，保证信息的完整性
                //可通过消息的第三个字节获取到消息长度。
                _port.DiscardInBuffer();
                var size = 2;

                var buff = new byte[5 + size + 1];

                _stream.Read(buff, 0, buff.Length);


                return buff[Pdu_Offset];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

       /// <summary>
       /// 创建PDU
       /// </summary>
       /// <param name="command"></param>
       /// <param name="channelNo"></param>
       /// <param name="values"></param>
       /// <returns></returns>
        private byte[] CreatePdu(B3Commands command, int channelNo, double[] values)
        {
            //一个数据4byte
            var size = 4 * values.Length;
            //5byte为header，1byte为校验位
            var buff = new byte[5 + size + 1];
            buff[0] = Start_Bytes;          //起始字节
            buff[1] = Mcu_Address;          //地址
            buff[2] = (byte)buff.Length;    //包长
            buff[3] = (byte)command;        //B3命令
            buff[4] = (byte)B4_Alt_Command; //B4命令
            buff[5] = (byte)channelNo;      //通道号
            //转化要发送的数据
            var bytes = ConvertToByte(values);

            for (var i = 0; i < bytes.Length; i++)
                buff[Pdu_Offset + 1 + i] = bytes[i];
            
            //校验位
            buff[buff.Length - 1] = ParityCheck(buff);

            return buff;
        }


        /// <summary>
        /// bytes转化成data（double）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public List<double> ConvertToDouble(byte[] bytes)
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
                    datas.Add(k * temp);
                }
            }
            else
            {
                for (var i = 0; i < bytes.Length; i++)
                    datas.Add((double)bytes[i]);
            }
            return datas;
        }


        /// <summary>
        /// date转化bytes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] ConvertToByte(double[] data)
        {
            var bytes = new byte[data.Length * 4];

            foreach (var val in data)
            {
                //计算绝对值
                var absValue = Math.Abs(val);
                //计算小数点
                var decimalPart = absValue - (int)absValue;

                if (val < 0)
                {
                    bytes[0] = (byte)(absValue / 256 + 0x80);//将F中的内容转换为负数
                    bytes[1] = (byte)(absValue % 256);
                    decimalPart = (int)(decimalPart * 10000);
                    bytes[2] = (byte)(decimalPart / 256);
                    bytes[3] = (byte)(decimalPart % 256);
                }
                else
                {
                    
                    bytes[0] = (byte)(absValue / 256);
                    bytes[1] = (byte)(absValue % 256);
                    decimalPart = (int)((decimalPart + 0.000001) * 10000);
                    bytes[2] = (byte)(decimalPart / 256);
                    bytes[3] = (byte)(decimalPart % 256);
                }
            }
            return bytes;
        }


        /// <summary>
        /// 计算校验位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte ParityCheck(byte[] data)
        {
            var parityValue = data[0];
            for (var i = 1; i < data.Length; i++)
            {
                parityValue ^= data[i];
            }
            return parityValue;
        }
    }
}
