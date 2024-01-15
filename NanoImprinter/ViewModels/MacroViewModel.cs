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

namespace NanoImprinter.ViewModels
{
    public class MacroViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private MacroPlatform _plate;

        private Point2D _workVel;
        private Point2D _loadPosition;
        private Point2D _gluePosition;
        private Point2D _imprintPosition;
        private Point2D _leftCenterPosition;
        private Point2D _rightCenterPosition;
        private Point2D _upCenterPosition;
        private Point2D _downCenterPosition;



        #region property
        public ObservableCollection<IAxis> Axes { get; set; }

        public Point2D WorkVel
        {
            get => _workVel;
            set=> SetProperty(ref _workVel,value);
        }
        public Point2D LoadPosition
        {
            get => _loadPosition;
            set => SetProperty(ref _loadPosition, value);
        }
        public Point2D GluePosition
        {
            get => _gluePosition;
            set => SetProperty(ref _gluePosition, value);
        }
        public Point2D ImprintPosition
        {
            get => _gluePosition;
            set => SetProperty(ref _imprintPosition, value);
        }
        public Point2D LeftCenterPosition
        {
            get => _leftCenterPosition;
            set => SetProperty(ref _leftCenterPosition, value);
        }
        public Point2D RightCenterPosition
        {
            get => _rightCenterPosition;
            set => SetProperty(ref _rightCenterPosition, value);
        }

        public Point2D UpCenterPosition
        {
            get => _upCenterPosition;
            set => SetProperty(ref _upCenterPosition, value);
        }
        public Point2D DownCenterPosition
        {
            get => _downCenterPosition;
            set => SetProperty(ref _downCenterPosition, value);
        }
        #endregion


        #region Command
        private DelegateCommand _goHomeCommand;
        private DelegateCommand _resetAlarmCommand;
        private DelegateCommand _saveParamCommand;
        private DelegateCommand _moveToLoadPositionCommand;
        private DelegateCommand _moveToGluePositionCommand;
        private DelegateCommand _moveToImprintPositonCommand;
        private DelegateCommand _moveToLeftCenterPositionCommand;
        private DelegateCommand _moveToRightCenterPositionCommand;
        private DelegateCommand _moveToUpCenterPositionCommand;
        private DelegateCommand _moveToDownCenterPositionCommand;

        public DelegateCommand GoHomeCommand => _goHomeCommand ?? new DelegateCommand(MaskZGoHome);
        public DelegateCommand ResetAlarmCommand => _resetAlarmCommand ?? new DelegateCommand(ResetAlarm);
        public DelegateCommand SaveParamCommand => _saveParamCommand ?? new DelegateCommand(MaskZGoHome);
        public DelegateCommand MoveToLoadPositionCommand => _moveToLoadPositionCommand ?? new DelegateCommand(MoveToPreprintPosition);
        public DelegateCommand MoveToGluePositionCommand => _moveToGluePositionCommand ?? new DelegateCommand(MoveToTakePicturePosition);

        public DelegateCommand MoveToImprintPositionCommand => _moveToImprintPositonCommand ?? new DelegateCommand(MoveToWaitPosition);
        public DelegateCommand MoveToLeftCenterPositionCommand => _moveToLeftCenterPositionCommand ?? new DelegateCommand(MoveToIrradiationPosition);
        public DelegateCommand MoveToRightCenterPositionCommand => _moveToRightCenterPositionCommand ?? new DelegateCommand(MoveToIrradiationPosition);
        public DelegateCommand MoveToUpCenterPositionCommand => _moveToUpCenterPositionCommand ?? new DelegateCommand(MoveToIrradiationPosition);
        public DelegateCommand MoveToDownCenterPositionCommand => _moveToDownCenterPositionCommand ?? new DelegateCommand(MoveToIrradiationPosition);
        #endregion


        public MacroViewModel(IMachineModel machine)
        {
            _machine = machine;
            _plate = _machine.GetPlatform(typeof(MacroPlatform).Name) as MacroPlatform;
            Axes = new ObservableCollection<IAxis>();
            Axes.Add(_plate.XAxis);
            Axes.Add(_plate.YAxis);
            Axes.Add(_plate.RAxis);
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
