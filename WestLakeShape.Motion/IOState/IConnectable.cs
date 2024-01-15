using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WestLakeShape.Motion
{
    public interface IConnectable
    {
        bool Connected { get; }

        void Connect();

        void Disconnect();
    }

    public abstract class Connectable:IConnectable
    {
        public abstract string Name { get; }

        public bool Connected { get; private set; }


        public void Connect()
        {
            if (!Connected)
            {
                OnConnecting();
                Connected = true;
                ThreadPool.QueueUserWorkItem(_ => TickProc());
            }
        }

        public void Disconnect()
        {
            if (Connected)
            {
                OnDisconnecting();
                Connected = false;
            }
        }


        private void TickProc()
        {
            while (Connected)
            {
                RefreshStates();
                Thread.Sleep(1);
            }
        }


        protected virtual void OnConnecting()
        {

        }
        protected virtual void OnDisconnecting()
        { 

        }

        protected abstract void RefreshStates();
    }

}
