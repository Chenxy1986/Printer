using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
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
        private ImprintPlatformConfig _platformConfig;
        private string _uvPortName;
        private string _forceSensorPortName;

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
        private double _uvZWorkVel;
        private int _uvPreheatTime;
        private int _uvExposureTime;       
        private double _uvZDirSafePosition;

        #region property
        public string UVPortName 
        {
            get => _uvPortName;
            set => SetProperty(ref _uvPortName, value);
        }
        public string ForceSensorPortName
        {
            get => _forceSensorPortName;
            set => SetProperty(ref _forceSensorPortName, value);
        }

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

        public double UVZWorkVel
        {
            get => _uvZWorkVel;
            set => SetProperty(ref _uvZWorkVel, value);
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
        public DelegateCommand ConnectedUVControlCommand => new DelegateCommand(ConnectedUVControl);
        public DelegateCommand ConnectedForceControlCommand => new DelegateCommand(ConnectedForceControl);

        public DelegateCommand SaveParamCommand => new DelegateCommand(SaveParam);
        public DelegateCommand ReloadParamCommand => new DelegateCommand(ReloadParam);
        public DelegateCommand MoveToMaskPreprintPositionCommand =>new DelegateCommand(MoveToMaskPreprintHeight);
        public DelegateCommand MoveToCameraTakePicturePositionCommand => new DelegateCommand(MoveToCameraTakePictureHeight);
        public DelegateCommand MaskZGoHomeCommand =>  new DelegateCommand(MaskZGoHome);
        public DelegateCommand ResetAlarmCommand =>  new DelegateCommand(ResetAlarm);
        public DelegateCommand MoveToUVWaitPositionCommand =>  new DelegateCommand(MoveToUVWaitPosition);
        public DelegateCommand MoveToUVIrradiationPositionCommand => new DelegateCommand(MoveToUVIrradiationPosition);
        public DelegateCommand UVGoHomeCommand => new DelegateCommand(UVGoHome);
        public DelegateCommand RefreshPortNamesCommand => new DelegateCommand(RefreshPortNames);

        #endregion

        public ObservableCollection<string> PortNames { get; set; }
        public ObservableCollection<IAxis> Axes { get; set; }

        public ImprintViewModel(IMachineModel machine)
        {
            _machine = machine;
            _plate = machine.GetPlatform(typeof(ImprintPlatform).Name) as ImprintPlatform;
            _platformConfig = _machine.Config.ImprintPlatform;
            Axes = new ObservableCollection<IAxis>();
            PortNames = new ObservableCollection<string>();
            Axes.Add(_plate.MaskZAxis);
            Axes.Add(_plate.CameraZAxis);
            Axes.Add(_plate.UVXAxis);
            Axes.Add(_plate.UVZAxis);
            _uvWaitPosition = new PointXZ(0,0);
            _uvIrradiationPosition = new PointXZ(0,0);
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
            _platformConfig.UVYWorkVel = UVZWorkVel;
            _platformConfig.UVConfig.PreheatTime = UVPreheatTime;
            _platformConfig.UVConfig.ExposureTime = UVExposureTime;
            _platformConfig.UVZDirSafePosition = UVZDirSafePosition;
            _platformConfig.UVConfig.PortName = _uvPortName;
            _platformConfig.ForceSensorControlConfig.PortName = _forceSensorPortName;
            _machine.SaveParam();
            _plate.ReloadConfig();
        }

        private void ReloadParam()
        {
            MaskWaitHeight = _platformConfig.MaskWaitHeight;
            MaskPreprintHeight = _platformConfig.MaskPreprintHeight;
            MaskZWorkVel = _platformConfig.MaskZWorkVel;
            CameraWaitHeight = _platformConfig.CameraWaitHeight;
            CameraTakePictureHeight = _platformConfig.CameraTakePictureHeight;
            CameraZWorkVel = _platformConfig.CameraZWorkVel;
            XDirSafePosition = _platformConfig.CameraXDirSafePosition;
            UVWaitPosition = _platformConfig.UVWaitPosition;
            UVIrradiationPosition = _platformConfig.UVIrradiationPosition;
            UVXWorkVel = _platformConfig.UVXWorkVel;
            UVZWorkVel = _platformConfig.UVYWorkVel;
            UVPreheatTime = _platformConfig.UVConfig.PreheatTime;
            UVExposureTime = _platformConfig.UVConfig.ExposureTime;
            UVZDirSafePosition = _platformConfig.UVZDirSafePosition;
            UVPortName = _platformConfig.UVConfig.PortName;
            ForceSensorPortName =_platformConfig.ForceSensorControlConfig.PortName;
        }

        private void ConnectedUVControl()
        {
            _plate.ConnectedUVControl();
        }
        private void ConnectedForceControl()
        {
            _plate.ConnectedForceControl();
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
        private void RefreshPortNames()
        {
            PortNames.Clear();
            foreach (var port in SerialPort.GetPortNames())
            {
                PortNames.Add(port);
            }
        }
    }
}
