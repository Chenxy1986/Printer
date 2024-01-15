using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.Procedures
{
    public interface IWorkProcedure
    {
        string Name { get; set; }
        DateTime StartTime { get; }
        DateTime EndTime { get; }
        TimeSpan Deration { get; }
        ProcesureStatus Status { get; set; }
        bool Excute();
    }


    public enum ProcesureStatus
    {
        None,
        Stopped,
        Running,
        Succeeded,
        Failed,
        Aborted,
        Cancelled
    }


    public abstract class WorkProcedure : IWorkProcedure
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private ProcesureStatus _status;

        public string Name { get; set; }

        public DateTime StartTime => _startTime;

        public DateTime EndTime => _endTime;

        public TimeSpan Deration => _endTime - _startTime;

        public ProcesureStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public bool Excute()
        {
            if (_status >= ProcesureStatus.Failed)
                return false;
            _startTime = DateTime.Now;
            _status = ProcesureStatus.Running;
            try
            {
                Prepare();
                OnExcute();
                _endTime = DateTime.Now;
                _status = ProcesureStatus.Succeeded;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        protected abstract bool Prepare();

        protected abstract bool OnExcute();

    }
}
