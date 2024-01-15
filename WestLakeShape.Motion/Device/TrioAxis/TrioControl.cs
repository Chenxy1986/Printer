using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrioMotion.TrioPC_NET;

namespace WestLakeShape.Motion.Device
{
    public class TrioControl : IDisposable
    {
        private static readonly Lazy<TrioControl> _instance = new Lazy<TrioControl>(() => new TrioControl());
        private static TrioPC _trioControl;
        private bool _isConnected;
        private string _ip = "127.0.0.1";

        public static TrioControl Instance => _instance.Value;

        public TrioPC TrioPC => _trioControl;

        public bool IsConnected => _isConnected;
        public bool IsError => _trioControl.IsError();

        private TrioControl()
        {
            _trioControl = new TrioPC();
        }

        public void Dispose()
        {
            if (_trioControl.IsOpen(PortId.Default))
                _trioControl.Close();
        }

        public bool Connected()
        {
            _trioControl.HostAddress = _ip;
            _isConnected = _trioControl.Open(PortType.Ethernet, PortId.Default);
            return _isConnected;
        }
        //public bool SetIP(string ip)
        //{
        //    _ip = ip;
        //    _isConnected = false;
        //    Connected();
        //    return _isConnected;
        //}

        public int GetLastError()
        {
            return _trioControl.GetLastError();
        }

        public void EmegercyStop()
        {
            _trioControl.RapidStop();
        }
    }

   
}
