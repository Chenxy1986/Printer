using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.ViewModels
{
    public class ImprintViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private ImprintPlatform _platform; 

        private double _maskZWorkVel;
        private double _cameraZWorkVel;
        private double _preprintPosition;
        private double _takePicturePosition;

        private double _irradiationPosition;
        private double _waitPosition;
        private double _cameraWorkVel;
       

        #region property
        public double MaskZWorkVel
        {
            get => _maskZWorkVel;
            set => SetProperty(ref _maskZWorkVel, value);
        }

        public double CameraZWorkVel
        {
            get => _cameraZWorkVel;
            set => SetProperty(ref _cameraZWorkVel, value);
        }

        public double PreprintPosition
        {
            get => _preprintPosition;
            set => SetProperty(ref _preprintPosition, value);
        }
        public double TakePicturePosition 
        {
            get => _takePicturePosition;
            set => SetProperty(ref _takePicturePosition, value);
        }
        public double IrradiationPosition
        {
            get => _irradiationPosition;
            set => SetProperty(ref _irradiationPosition, value);
        }
        public double WaitPosition
        {
            get => _waitPosition;
            set => SetProperty(ref _waitPosition, value);
        }
        public double CameraWorkVel
        {
            get => _cameraWorkVel;
            set => SetProperty(ref _cameraWorkVel, value);
        }

        #endregion


        #region Command
        private DelegateCommand _saveParamCommand;
        private DelegateCommand _moveToPreprintPositionCommand;
        private DelegateCommand _moveToTakePicturePositionCommand;
        private DelegateCommand _maskZGoHomeCommand;
        private DelegateCommand _resetAlarmCommand;
        private DelegateCommand _moveToWaitPositionCommand;
        private DelegateCommand _movetoIrradiationPositionCommand;
        private DelegateCommand _uvGoHomeCommand;

        public DelegateCommand SaveParamCommand => _saveParamCommand ?? new DelegateCommand(MaskZGoHome);
        public DelegateCommand MoveToPreprintPositionCommand => _moveToPreprintPositionCommand ?? new DelegateCommand(MoveToPreprintPosition);
        public DelegateCommand MoveToTakePicturePositionCommand => _moveToTakePicturePositionCommand ?? new DelegateCommand(MoveToTakePicturePosition);
        public DelegateCommand MaskZGoHomeCommand => _maskZGoHomeCommand ?? new DelegateCommand(MaskZGoHome);
        public DelegateCommand ResetAlarmCommand => _resetAlarmCommand ?? new DelegateCommand(ResetAlarm);
        public DelegateCommand MoveToWaitPositionCommand => _moveToWaitPositionCommand ?? new DelegateCommand(MoveToWaitPosition);
        public DelegateCommand MoveToIrradiationPositionCommand => _movetoIrradiationPositionCommand ?? new DelegateCommand(MoveToIrradiationPosition);
        public DelegateCommand UVGoHomeCommand => _uvGoHomeCommand ?? new DelegateCommand(UVGoHome);

        #endregion


        public ImprintViewModel(IMachineModel machine)
        {
            _machine = machine;
            _platform = machine.GetPlatform(typeof(ImprintPlatform).Name) as ImprintPlatform;
        }

        private void MaskZGoHome()
        {
            
        }
        private void MoveToPreprintPosition()
        {

        }
        private void MoveToTakePicturePosition()
        {
            
        }
        private void ResetAlarm()
        {
            
        }

        private void SaveParam()
        {

        }

        private void MoveToWaitPosition()
        {

        }
        private void MoveToIrradiationPosition()
        {

        }
        private void UVGoHome()
        {

        }

    }
}
