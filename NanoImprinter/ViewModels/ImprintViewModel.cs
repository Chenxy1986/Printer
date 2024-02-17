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
        private int _uvPreheatTime;
        private int _uvExposureTime;
        private ImprintPlatformConfig _platformConfig;
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

        public double CameraTakePictureHeight
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

        public int UVPreheatTime
        {
            get => _uvPreheatTime;
            set => SetProperty(ref _uvPreheatTime, value);
        }
        public int UVExposureTime
        {
            get => _uvExposureTime;
            set => SetProperty(ref _uvExposureTime, value);
        }


        public double UVZDirSafePosition
        {
            get => _uvZDirSafePosition;
            set => SetProperty(ref _uvZDirSafePosition, value);
        }

        #endregion


        #region Command
        public DelegateCommand SaveParamCommand => new DelegateCommand(SaveParam);
        public DelegateCommand ReloadParamCommand => new DelegateCommand(ReloadParam);
        public DelegateCommand MoveToMaskPreprintPositionCommand =>new DelegateCommand(MoveToMaskPreprintHeight);
        public DelegateCommand MoveToCameraTakePicturePositionCommand => new DelegateCommand(MoveToCameraTakePictureHeight);
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
            _platformConfig = _machine.Config.ImprintPlatform;
            Axes = new ObservableCollection<IAxis>();
            Axes.Add(_plate.MaskZAxis);
            Axes.Add(_plate.CameraZAxis);
            Axes.Add(_plate.UVXAxis);
            Axes.Add(_plate.UVZAxis);
            ReloadParam();
        }

        private void MaskZGoHome()
        {
            _plate.MaskZAxis.GoHome();
        }
        private void MoveToMaskPreprintHeight()
        {
            _plate.MoveToMaskPreprintHeight();
        }
        private void MoveToCameraTakePictureHeight()
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
            _platformConfig.MaskWaitHeight = MaskWaitHeight;
            _platformConfig.MaskPreprintHeight = MaskPreprintHeight;
            _platformConfig.MaskZWorkVel = MaskZWorkVel;
            _platformConfig.CameraWaitHeight = CameraWaitHeight;
            _platformConfig.CameraTakePictureHeight = CameraTakePictureHeight;
            _platformConfig.CameraZWorkVel = CameraZWorkVel;
            _platformConfig.CameraXDirSafePosition = XDirSafePosition;
            _platformConfig.UVWaitPosition = UVWaitPosition;
            _platformConfig.UVIrradiationPosition = UVIrradiationPosition;
            _platformConfig.UVXWorkVel = UVXWorkVel;
            _platformConfig.UVYWorkVel = UVYWorkVel;
            _platformConfig.UVConfig.PreheatTime = UVPreheatTime;
            _platformConfig.UVConfig.ExposureTime = UVExposureTime;
            _platformConfig.UVZDirSafePosition = UVZDirSafePosition;
            _machine.SaveParam();
        }

        private void ReloadParam()
        {
           MaskWaitHeight = _platformConfig.MaskWaitHeight;
           MaskPreprintHeight = _platformConfig.MaskPreprintHeight;
           MaskZWorkVel = _platformConfig.MaskZWorkVel;
           CameraWaitHeight = _platformConfig.CameraWaitHeight;
           CameraTakePictureHeight = _platformConfig.CameraTakePictureHeight;
           CameraZWorkVel = _platformConfig.CameraZWorkVel ;
           XDirSafePosition = _platformConfig.CameraXDirSafePosition ;
           UVWaitPosition = _platformConfig.UVWaitPosition ;
           UVIrradiationPosition = _platformConfig.UVIrradiationPosition ;
           UVXWorkVel = _platformConfig.UVXWorkVel;
           UVYWorkVel = _platformConfig.UVYWorkVel ;
           UVZDirSafePosition = _platformConfig.UVZDirSafePosition ;
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
