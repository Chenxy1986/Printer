using NanoImprinter.Events;
using NanoImprinter.Model;
using NanoImprinter.Procedures;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanoImprinter.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private readonly SemaphoreSlim _workDoneEvent = new SemaphoreSlim(1, 1);
        private ProcedureManager _manager;
        //private IEventAggregator _eventAggregator;
        private WorkStatus _status;

        private ProcedureStatus _loadStatus;
        private ProcedureStatus _glueStatus;
        private ProcedureStatus _preprintStatus;
        private ProcedureStatus _imprintStatus;
        private ProcedureStatus _uvStatus;
        private ProcedureStatus _demoldStatus;
        private ProcedureStatus _positionStatus;

        private bool _isAuto = false;
        private string _startContent = "启动";

        private int _currentRow;
        private int _currentCol;


        #region property
        public int MaskUsageCount
        {
            get => _machine.Config.MaskInfo.MaskUsageCount;
            set
            {
                if (_machine.Config.MaskInfo.MaskUsageCount != value)
                {
                    _machine.Config.MaskInfo.MaskUsageCount = value;
                    RaisePropertyChanged(nameof(MaskUsageCount));
                }
            }
        }

        public int MaskLifetimeCount
        {
            get => _machine.Config.MaskInfo.MaskLifetimeCount;
            set 
            {
                if (_machine.Config.MaskInfo.MaskLifetimeCount != value)
                {
                    _machine.Config.MaskInfo.MaskLifetimeCount = value;
                    RaisePropertyChanged(nameof(MaskLifetimeCount));
                }
            }
        }

        public int ImprintCol
        {
            get => _machine.Config.MaskInfo.ImprintCol;
            set
            {
                if (_machine.Config.MaskInfo.ImprintCol != value)
                {
                    _machine.Config.MaskInfo.ImprintCol = value;
                    RaisePropertyChanged(nameof(ImprintCol));
                }
            }
        }
        
        public int ImprintRow
        {
            get => _machine.Config.MaskInfo.ImprintRow;
            set
            {
                if (_machine.Config.MaskInfo.ImprintRow != value)
                {
                    _machine.Config.MaskInfo.ImprintRow = value;
                    RaisePropertyChanged(nameof(ImprintRow));
                }
            }
        }

        public int ImprintCount
        {
            get => _machine.Config.MaskInfo.ImprintCount;
            set
            {
                if (_machine.Config.MaskInfo.ImprintCount != value)
                {
                    _machine.Config.MaskInfo.ImprintCount = value;
                    RaisePropertyChanged(nameof(ImprintCount));
                }
            }
        }
       
        public int CurrentIndex
        {
            get => _machine.Config.MaskInfo.CurrentIndex;
            set
            {
                if (_machine.Config.MaskInfo.CurrentIndex != value)
                {
                    _machine.Config.MaskInfo.CurrentIndex = value;
                    RaisePropertyChanged(nameof(CurrentIndex));
                }
            }
        }

        public int CurrentRow
        {
            get => _currentRow;
            set => SetProperty(ref _currentRow, value);
        }
        public int CurrentCol
        {
            get => _currentCol;
            set => SetProperty(ref _currentCol, value);
        }

        public WorkStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        
        public bool IsAuto
        {
            get => _isAuto;
            set => SetProperty(ref _isAuto, value);
        }
        
        public string StartContent
        {
            get => _startContent;
            set => SetProperty(ref _startContent, value);
        }
        #endregion


        #region procedure status
        public ProcedureStatus LoadStatus
        {
            get => _loadStatus;
            set
            {
                _loadStatus = value;
                SetProperty(ref _loadStatus, value);
            }
        }
        public ProcedureStatus GlueStatus
        {
            get => _glueStatus;
            set
            {
                _glueStatus = value;
                SetProperty(ref _glueStatus, value);
            }
        }
        public ProcedureStatus PreprintStatus
        {
            get => _preprintStatus;
            set
            {
                _preprintStatus = value;
                SetProperty(ref _preprintStatus, value);
            }
        }
        public ProcedureStatus ImprintStatus
        {
            get => _imprintStatus;
            set
            {
                _imprintStatus = value;
                SetProperty(ref _imprintStatus, value);
            }
        }
        public ProcedureStatus UVStatus
        {
            get => _uvStatus;
            set
            {
                _uvStatus = value;
                SetProperty(ref _uvStatus, value);
            }
        }
        public ProcedureStatus DemoldStatus
        {
            get => _demoldStatus;
            set
            {
                _uvStatus = value;
                SetProperty(ref _demoldStatus, value);
            }
        }
        public ProcedureStatus PositionStatus
        {
            get => _positionStatus;
            set
            {
                _positionStatus = value;
                SetProperty(ref _positionStatus, value);
            }
        }
        #endregion


        #region command
        public DelegateCommand StartCommand => new DelegateCommand(Start);
        public DelegateCommand EmergencyCommand => new DelegateCommand(Emergency);
        public DelegateCommand ResetCommand => new DelegateCommand(Reset);
        public DelegateCommand GoHomeCommand => new DelegateCommand(GoHome);
        public DelegateCommand VacuumCommand => new DelegateCommand(Vacuum);
        public DelegateCommand EvacuateCommand => new DelegateCommand(Evacuate);
        #endregion


        public MainViewModel(IMachineModel machine, ProcedureManager manager,IEventAggregator eventAggregator)
        {
            _machine = machine;
            _manager = manager;
            eventAggregator.GetEvent<ProcedureInfoEvent>().Subscribe(ProcedureInfoChanged);
        }

       

        private void Emergency()
        {
            _machine.Axes.All().ForEach(o => o.EmergencyStop());
        }

        private void Start()
        {
            switch (_status)
            {
                case WorkStatus.Emergency:
                    throw new InvalidOperationException("急停状态下，必须先复位才能再次启动");
                case WorkStatus.Terminated:
                    _workDoneEvent.Wait();
                    Status = WorkStatus.Running;
                    StartContent = "暂停";
                    ThreadPool.QueueUserWorkItem(o => StartLoop());
                    break;
                case WorkStatus.Running:
                    Status = WorkStatus.Paused;
                    StartContent = "启动";
                    return;
                case WorkStatus.Paused:
                    Status = WorkStatus.Running;
                    break;
            }
        }

        private void Reset()
        {
            Status = WorkStatus.Terminated;

            _machine.Axes.All().ForEach(o => o.ResetAlarm());
        }

        private void GoHome()
        {
            Status = WorkStatus.Terminated;

            //先脱模
            var microPlatform = _machine.GetPlatform(typeof(MicroPlatform).Name) as MicroPlatform;
            microPlatform.Demold();

            foreach (var plate in _machine.Platforms)
                plate.Value.GoHome();
        }


        private void Vacuum()
        {
            
        }


        private void Evacuate()
        {
            
        }


        /// <summary>
        /// 开始执行自动流程
        /// </summary>
        private void StartLoop()
        {
            try
            {
                //放wafe
                _manager.ExcuteProcedureByName(typeof(LoadProcedure).Name);
                //定位wafe圆心
                _manager.ExcuteProcedureByName(typeof(FindRotateCenterProcedure).Name);
                //定位初次压印位置
                _manager.ExcuteProcedureByName(typeof(PositionProcedure).Name);

                while (true)
                {
                    switch (_status)
                    {
                        case WorkStatus.Emergency:
                            return;
                        case WorkStatus.Terminated:
                            return;
                        case WorkStatus.Running:
                            SingleLoop();
                            break;
                        case WorkStatus.Paused:
                            Thread.Sleep(10);
                            break;
                        default:
                            throw new Exception("未知");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _workDoneEvent.Release();
            }
        }
        

        /// <summary>
        /// 单次循环
        /// </summary>
        private void SingleLoop()
        {
            //达到压印次数，则终止任务
            if (CurrentIndex >= ImprintCount)
            {
                Status = WorkStatus.Terminated;
                StartContent = "启动";
                return;
            }

            while (_machine.ProcedureIndex < _manager.AutoProcedures.Count)
            {
                var result = _manager.ExecuteAutoProcedureByIndex(_machine.ProcedureIndex);
                if (result)
                {
                    //记录当前流程的步骤，报错关机后下次继续执行
                    _machine.ProcedureIndex++;

                    //结束流程
                    if (_machine.ProcedureIndex == _manager.AutoProcedures.Count)
                    {
                        _machine.ProcedureIndex = 0;
                        break;
                    } 
                }
                else
                {
                    var name = _manager.GetProcedureName(_machine.ProcedureIndex);
                    throw new Exception($"流程{name}未正确执行完");
                }
            }

            //压印图案计数
            CurrentIndex++;
            CurrentRow = CurrentIndex / ImprintCol;
            CurrentCol = CurrentIndex % ImprintCol;
        }
        

        /// <summary>
        /// 刷新当前各个流程状态
        /// </summary>
        /// <param name="info"></param>
        private void ProcedureInfoChanged(ProcedureInfo info)
        {
            switch (info.Name)
            {
                case "取料流程":
                    LoadStatus = info.ProcedureStatus;
                    break;
                case "点胶流程":
                    GlueStatus = info.ProcedureStatus;
                    break;
                case "预压印流程":
                    PreprintStatus = info.ProcedureStatus;
                    break;
                case "压印流程":
                    ImprintStatus = info.ProcedureStatus;
                    break;
                case "UV固化流程":
                    PreprintStatus = info.ProcedureStatus;
                    break;
                case "脱模流程":
                    DemoldStatus = info.ProcedureStatus;
                    break;
                case "定位流程":
                    PositionStatus = info.ProcedureStatus;
                    break;

                case "找圆心流程":
                    var status = info.ProcedureStatus;
                    break;
            }
        }

    }


    public enum WorkStatus
    {
        /// <summary>
        /// 终止
        /// </summary>
        Terminated,
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        /// <summary>
        /// 暂停中
        /// </summary>
        Paused,
        /// <summary>
        /// 急停中
        /// </summary>
        Emergency,

    }

}
