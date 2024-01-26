using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Common;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.ViewModels
{
    public class MicroViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private MicroPlatform _microPlatform;
        private double _currentPressure;
        private double _moveDistance;

        private ChannelNo _selectedChannel;//选择通道

        #region property

        public string ComName
        {
            get => _machine.Config.MicroPlatform.ComName;
            set
            {
                if (_machine.Config.MicroPlatform.ComName != value)
                {
                    _machine.Config.MicroPlatform.ComName = value;
                    RaisePropertyChanged(nameof(ComName));
                }
            }
        }

        /// <summary>
        /// 压印过程快速移动到接触位，然后开始读取压力值并开始蠕动
        /// </summary>
        public double ContactPosition
        {
            get => _machine.Config.MicroPlatform.ContactPosition;
            set
            {
                if (_machine.Config.MicroPlatform.ContactPosition != value)
                {
                    _machine.Config.MicroPlatform.ContactPosition = value;
                    RaisePropertyChanged(nameof(ContactPosition));
                }
            }
        }
        /// <summary>
        /// Z向压印过程中的蠕动距离
        /// </summary>
        public double ZCreepDistance
        {
            get => _machine.Config.MicroPlatform.ZCreepDistance;
            set
            {
                if (_machine.Config.MicroPlatform.ZCreepDistance != value)
                {
                    _machine.Config.MicroPlatform.ZCreepDistance = value;
                    RaisePropertyChanged(nameof(ZCreepDistance));
                }
            }
        }

        public PointZRXY DemoldPosition
        {
            get => _machine.Config.MicroPlatform.DemoldPosition;
            set
            {
                if (_machine.Config.MicroPlatform.DemoldPosition != value)
                {
                    _machine.Config.MicroPlatform.DemoldPosition = value;
                    RaisePropertyChanged(nameof(DemoldPosition));
                }
            }
        }

        public PointZRXY LevelPosition
        {
            get => _machine.Config.MicroPlatform.LevelPosition;
            set
            {
                if (_machine.Config.MicroPlatform.LevelPosition != value)
                {
                    _machine.Config.MicroPlatform.LevelPosition = value;
                    RaisePropertyChanged(nameof(LevelPosition));
                }
            }
        }
 
        public double MaxPressure
        {
            get => _machine.Config.MicroPlatform.MaxPressure;
            set
            {
                if (_machine.Config.MicroPlatform.MaxPressure != value)
                {
                    _machine.Config.MicroPlatform.MaxPressure = value;
                    RaisePropertyChanged(nameof(MaxPressure));
                }
            }
        }     
        public double MinPressure
        {
            get => _machine.Config.MicroPlatform.MinPressure;
            set
            {
                if (_machine.Config.MicroPlatform.MinPressure != value)
                {
                    _machine.Config.MicroPlatform.MinPressure = value;
                    RaisePropertyChanged(nameof(MinPressure));
                }
            }
        }
        
        public double CurrentPressure
        {
            get => _currentPressure;
            set => SetProperty(ref _currentPressure, value);
        }
        
        public double MoveDistance
        {
            get => _moveDistance;
            set => SetProperty(ref _moveDistance, value);
        }
        
        public ChannelNo SelectedChannel
        {
            get => _selectedChannel;
            set => SetProperty(ref _selectedChannel, value);
        }
        
        public IList<ChannelNo> ChannelIndex { get; }

        #endregion


        #region Command

        public DelegateCommand GoHomeCommand => new DelegateCommand(GoHome);
        public DelegateCommand MoveToLevelPositionCommand => new DelegateCommand(MoveToLevelPosition);
        public DelegateCommand MoveToDemoldPositionCommand => new DelegateCommand(MoveToDemoldPosition);
        public DelegateCommand CreepCommand => new DelegateCommand(Creep);
        public DelegateCommand JogForwardCommand => new DelegateCommand(JogForward);
        public DelegateCommand JogBackwardCommand => new DelegateCommand(JogBackward);
        public DelegateCommand SaveParamCommand => new DelegateCommand(SaveParam);
        public DelegateCommand ReloadParamCommand => new DelegateCommand(ReloadParam);

        #endregion



        public MicroViewModel(IMachineModel machine)
        {
            _machine = machine;
            _microPlatform = _machine.GetPlatform(typeof(MicroPlatform).Name) as MicroPlatform;
            ChannelIndex = Enum.GetValues(typeof(ChannelNo)).Cast<ChannelNo>().ToList();
        }


        private void GoHome()
        {
            _microPlatform.GoHome();
        }

        private void MoveToLevelPosition()
        {
            _microPlatform.MoveTo(LevelPosition);
        }

        private void MoveToDemoldPosition()
        {
            _microPlatform.MoveTo(DemoldPosition);
        }

        private void Creep()
        {
            _microPlatform.Creep(SelectedChannel, MoveDistance);
        }

        private void JogForward()
        {
            
        }
        private void JogBackward()
        {
            
        }

        private void RefreshPressure()
        {
           
        }
        private void SaveParam()
        {
           _machine.Config.MicroPlatform.ComName = ComName;
            _machine.Config.MicroPlatform.ContactPosition = ContactPosition;
            _machine.Config.MicroPlatform.ZCreepDistance = ZCreepDistance;
            _machine.Config.MicroPlatform.DemoldPosition = DemoldPosition;
            _machine.Config.MicroPlatform.LevelPosition = LevelPosition;
            _machine.Config.MicroPlatform.MaxPressure = MaxPressure;
            _machine.Config.MicroPlatform.MinPressure = MinPressure;
        }
    private void ReloadParam()
        {
            ComName = _machine.Config.MicroPlatform.ComName;
            ContactPosition = _machine.Config.MicroPlatform.ContactPosition;
            ZCreepDistance = _machine.Config.MicroPlatform.ZCreepDistance;
            DemoldPosition = _machine.Config.MicroPlatform.DemoldPosition;
            LevelPosition = _machine.Config.MicroPlatform.LevelPosition;
            MaxPressure = _machine.Config.MicroPlatform.MaxPressure;
            MinPressure = _machine.Config.MicroPlatform.MinPressure;
        }
    }
}
