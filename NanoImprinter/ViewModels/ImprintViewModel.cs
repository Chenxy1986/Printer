using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Common;
using WestLakeShape.Motion;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.ViewModels
{
    public class ImprintViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private ImprintPlatform _plate;

        private double _maskWaitHeight;
        private double _maskPreprintHeight;
        private double _maskZWorkVel;
        private double _cameraWaitHeight;
        private double _takePictureHeight;
        private double _cameraZWorkVel;
        private double _xDirSafePosition;
        private PointXZ _uvWaitPosition;
        private PointXZ _uvIrradiationPosition;
        private double _uvXWorkVel;
        private double _uvYWorkVel;
        private UVControlConfig _uvConfig;
        private double _uvZDirSafePosition;

        #region property
        public double MaskWaitHeight
        {
            get => _maskWaitHeight;
            set => SetProperty(ref _maskWaitHeight, value);
        }
        public double MaskPreprintHeight
        {
            get => _maskPreprintHeight;
            set => SetProperty(ref _maskPreprintHeight, value);
        }
        public double MaskZWorkVel
        {
            get => _maskZWorkVel;
            set => SetProperty(ref _maskZWorkVel, value);
        }

        public double CameraWaitHeight
        {
            get => _cameraWaitHeight;
            set => SetProperty(ref _cameraWaitHeight, value);
        }

        public double TakePictureHeight
        {
            get => _takePictureHeight;
            set => SetProperty(ref _takePictureHeight, value);
        }

        public double CameraZWorkVel
        {
            get => _cameraZWorkVel;
            set => SetProperty(ref _cameraZWorkVel, value);
        }

        public double XDirSafePosition
        {
            get => _xDirSafePosition;
            set => SetProperty(ref _xDirSafePosition, value);
        }

        public PointXZ UVWaitPosition
        {
            get => _uvWaitPosition;
            set => SetProperty(ref _uvWaitPosition, value);
        }

        public PointXZ UVIrradiationPosition
        {
            get => _uvIrradiationPosition;
            set => SetProperty(ref _uvIrradiationPosition, value);
        }

        public double UVXWorkVel
        {
            get => _uvXWorkVel;
            set => SetProperty(ref _uvXWorkVel, value);
        }

        public double UVYWorkVel
        {
            get => _uvYWorkVel;
            set => SetProperty(ref _uvYWorkVel, value);
        }

        public UVControlConfig UVConfig
        {
            get => _uvConfig;
            set => SetProperty(ref _uvConfig, value);
        }
        public double UVZDirSafePosition
        {
            get => _uvZDirSafePosition;
            set => SetProperty(ref _uvZDirSafePosition, value);
        }

        #endregion


        #region Command
        public DelegateCommand SaveParamCommand => new DelegateCommand(MaskZGoHome);
        public DelegateCommand ReloadParamCommand => new DelegateCommand(ReloadParam);
        public DelegateCommand MoveToPreprintPositionCommand =>new DelegateCommand(MoveToMaskPreprintHeight);
        public DelegateCommand MoveToTakePicturePositionCommand => new DelegateCommand(MoveToTakePictureHeight);
        public DelegateCommand MaskZGoHomeCommand =>  new DelegateCommand(MaskZGoHome);
        public DelegateCommand ResetAlarmCommand =>  new DelegateCommand(ResetAlarm);
        public DelegateCommand MoveToUVWaitPositionCommand =>  new DelegateCommand(MoveToUVWaitPosition);
        public DelegateCommand MoveToUVIrradiationPositionCommand => new DelegateCommand(MoveToUVIrradiationPosition);
        public DelegateCommand UVGoHomeCommand => new DelegateCommand(UVGoHome);

        #endregion


        public ObservableCollection<IAxis> Axes { get; set; }

        public ImprintViewModel(IMachineModel machine)
        {
            _machine = machine;
            _plate = machine.GetPlatform(typeof(ImprintPlatform).Name) as ImprintPlatform;
            Axes = new ObservableCollection<IAxis>();
            Axes.Add(_plate.MaskZAxis);
            Axes.Add(_plate.CameraZAxis);
            Axes.Add(_plate.UVXAxis);
            Axes.Add(_plate.UVZAxis);
        }

        private void MaskZGoHome()
        {
            _plate.MaskZAxis.GoHome();
        }
        private void MoveToMaskPreprintHeight()
        {
            _plate.MoveToMaskPreprintHeight();
        }
        private void MoveToTakePictureHeight()
        {
            _plate.MoveToTakePictureHeight();
        }
        private void ResetAlarm()
        {
            _plate.MaskZAxis.ResetAlarm();
            _plate.CameraZAxis.ResetAlarm();
            _plate.UVXAxis.ResetAlarm();
            _plate.UVZAxis.ResetAlarm();
        }

        private void SaveParam()
        {
            _machine.Config.ImprintPlatform.MaskWaitHeight = MaskWaitHeight;
            _machine.Config.ImprintPlatform.MaskPreprintHeight = MaskPreprintHeight;
            _machine.Config.ImprintPlatform.MaskZWorkVel = MaskZWorkVel;
            _machine.Config.ImprintPlatform.CameraWaitHeight = CameraWaitHeight;
            _machine.Config.ImprintPlatform.TakePictureHeight = TakePictureHeight;
            _machine.Config.ImprintPlatform.CameraZWorkVel = CameraZWorkVel;
            _machine.Config.ImprintPlatform.XDirSafePosition = XDirSafePosition;
            _machine.Config.ImprintPlatform.UVWaitPosition = UVWaitPosition;
            _machine.Config.ImprintPlatform.UVIrradiationPosition = UVIrradiationPosition;
            _machine.Config.ImprintPlatform.UVXWorkVel = UVXWorkVel;
            _machine.Config.ImprintPlatform.UVYWorkVel = UVYWorkVel;
            _machine.Config.ImprintPlatform.UVConfig = UVConfig;
            _machine.Config.ImprintPlatform.UVZDirSafePosition = UVZDirSafePosition;
            _machine.SaveParam();
        }

        private void ReloadParam()
        {
           MaskWaitHeight =_machine.Config.ImprintPlatform.MaskWaitHeight;
           MaskPreprintHeight =_machine.Config.ImprintPlatform.MaskPreprintHeight;
           MaskZWorkVel = _machine.Config.ImprintPlatform.MaskZWorkVel;
           CameraWaitHeight =_machine.Config.ImprintPlatform.CameraWaitHeight;
           TakePictureHeight = _machine.Config.ImprintPlatform.TakePictureHeight;
           CameraZWorkVel = _machine.Config.ImprintPlatform.CameraZWorkVel ;
           XDirSafePosition = _machine.Config.ImprintPlatform.XDirSafePosition ;
           UVWaitPosition = _machine.Config.ImprintPlatform.UVWaitPosition ;
           UVIrradiationPosition = _machine.Config.ImprintPlatform.UVIrradiationPosition ;
           UVXWorkVel = _machine.Config.ImprintPlatform.UVXWorkVel;
           UVYWorkVel = _machine.Config.ImprintPlatform.UVYWorkVel ;
           UVConfig = _machine.Config.ImprintPlatform.UVConfig;
           UVZDirSafePosition = _machine.Config.ImprintPlatform.UVZDirSafePosition ;
        }

        private void MoveToUVWaitPosition()
        {
            _plate.MoveToUVWaitPositon(); 
        }
        private void MoveToUVIrradiationPosition()
        {
            _plate.MoveToUVIrradiationPosition();
        }
        private void UVGoHome()
        {
            _plate.UVXAxis.GoHome();
            _plate.UVZAxis.GoHome();
        }

    }
}
