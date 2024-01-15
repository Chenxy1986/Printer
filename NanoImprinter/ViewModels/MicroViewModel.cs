using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Common;

namespace NanoImprinter.ViewModels
{
    public class MicroViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private MicroPlatform _platform;

        private PointZRXY _levelPosition;
        private PointZRXY _demoldPosition;
        private double _creepDistance;
        private double _maxPressure;
        private double _minPressure;
        private double _currentPressure;

        #region
        public PointZRXY LevelPosition
        {
            get => _levelPosition;
            set => SetProperty(ref _levelPosition, value);
        }
        public PointZRXY DemoldPosition
        {
            get => _demoldPosition;
            set => SetProperty(ref _demoldPosition, value);
        }
        public double CreepDistance
        {
            get => _creepDistance;
            set => SetProperty(ref _creepDistance, value);
        }
        
        public double MaxPressure
        {
            get => _maxPressure;
            set => SetProperty(ref _maxPressure, value);
        }
        
        public double MinPressure
        {
            get => _minPressure;
            set => SetProperty(ref _minPressure, value);
        }
        public double CurrenPressure
        {
            get => _currentPressure;
            set => SetProperty(ref _currentPressure, value);
        }
        #endregion

        public MicroViewModel(IMachineModel machine)
        {
            _machine = machine;
            _platform = _machine.GetPlatform(typeof(MicroPlatform).Name) as MicroPlatform;
        }

        #region
        
        private DelegateCommand _goHomeCommand;
        private DelegateCommand _moveToLevelPositionCommand;
        private DelegateCommand _moveToDemoldPositionCommand;

        public DelegateCommand GoHomeCommand => _goHomeCommand ?? new DelegateCommand(GoHome);
        public DelegateCommand MoveToLevelPositionCommand => _moveToLevelPositionCommand ?? new DelegateCommand(MoveToLevelPosition);
        public DelegateCommand MoveToDemoldPositionCommand => _moveToDemoldPositionCommand ?? new DelegateCommand(MoveToDemoldPosition);
        
        #endregion


        private void GoHome()
        {
            _platform.GoHome();
        }

        private void MoveToLevelPosition()
        {
            _platform.MoveTo(_levelPosition);
        }

        private void MoveToDemoldPosition()
        {
            _platform.MoveTo(_demoldPosition);
        }

        private void RefreshPressure()
        {
           
        }

    }
}
