﻿using NanoImprinter.Model;
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
        private double _uvWaitPosition;
        private double _uvIrradiationPosition;
        private double _uvXWorkVel;
        private double _uvYWorkVel;
        private int _uvIrradiationTime;
        private int _uvPowerPercentage;       
        private double _uvYDirSafePosition;

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
        public double UVWaitPosition
        {
            get => _uvWaitPosition;
            set => SetProperty(ref _uvWaitPosition, value);
        }
        public double UVIrradiationPosition
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
            get => _uvYWorkVel;
            set => SetProperty(ref _uvYWorkVel, value);
        }
        public int UVIrradiationTime
        {
            get => _uvIrradiationTime;
            set => SetProperty(ref _uvIrradiationTime, value);
        }
        public int UVPowerPercentage
        {
            get => _uvPowerPercentage;
            set => SetProperty(ref _uvPowerPercentage, value);
        }
        public double UVYDirSafePosition
        {
            get => _uvYDirSafePosition;
            set => SetProperty(ref _uvYDirSafePosition, value);
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
        public DelegateCommand OpenUVLightCommand => new DelegateCommand(OpenUVLight);
        public DelegateCommand CloseUVLightCommand => new DelegateCommand(CloseUVLight);
        public DelegateCommand WriteUVParameterCommand => new DelegateCommand(WriteUVParameter);


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
            
            RefreshPortNames();
            ReloadParam();
        }

        private void MaskZGoHome()
        {
            var task = Task.Run(() =>
            {
                _plate.GoHome();
            });
        }
        private void MoveToMaskPreprintHeight()
        {
            var task = Task.Run(() =>
            {
                _plate.MoveToMaskPreprintHeight();
            });
            
        }
        private void MoveToCameraTakePictureHeight()
        {
            var task = Task.Run(() =>
            {
                _plate.MoveToTakePictureHeight();
            });
        }
        private void ResetAlarm()
        {
            _plate.MaskZAxis.ResetAlarm();
            _plate.CameraZAxis.ResetAlarm();
            _plate.UVXAxis.ResetAlarm();
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
            _platformConfig.UVConfig.IrradiationTime = UVIrradiationTime;
            _platformConfig.UVConfig.PowerPercentage = UVPowerPercentage;
            _platformConfig.UVXDirSafePosition = UVYDirSafePosition;
            _platformConfig.UVConfig.PortName = _uvPortName;
            _platformConfig.ForceSensorControlConfig.PortName = _forceSensorPortName;
            
            _machine.SaveParam();
            _plate.ReloadConfig();
            
            //保存参数到UV控制器中
            _plate.WriteUVParam();
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
            UVIrradiationTime = _platformConfig.UVConfig.IrradiationTime;
            UVPowerPercentage = _platformConfig.UVConfig.PowerPercentage;
            UVYDirSafePosition = _platformConfig.UVXDirSafePosition;
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
            var task = Task.Run(() =>
            {
                _plate.MoveToUVWaitPositon();
            });
        }
        private void MoveToUVIrradiationPosition()
        {
            var task = Task.Run(() =>
            {
                _plate.MoveToUVIrradiationPosition();
            });
        }
        private void UVGoHome()
        {
            var task = Task.Run(() =>
            {
                _plate.UVXAxis.GoHome();
            });
        }
        private void RefreshPortNames()
        {
            PortNames.Clear();
            foreach (var port in SerialPort.GetPortNames())
            {
                PortNames.Add(port);
            }
        }

        private void OpenUVLight()
        {
            _plate.OpenUVLight();
        }
        private void CloseUVLight()
        {
            _plate.CloseUVLight();
        }
        private void WriteUVParameter()
        {
            _plate.WriteUVParam();
        }
    }
}
