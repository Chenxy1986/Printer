﻿using NanoImprinter.Model;
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
        private MacroPlatform _macroPlatform;
        private double _xWorkVel;
        private double _yWorkVel;
        private double _rWorkVel;
        private Point2D _loadPosition = new Point2D();
        private Point2D _gluePosition = new Point2D();
        private Point2D _imprintPosition = new Point2D();
        private Point2D _leftCenterPosition = new Point2D();
        private Point2D _rightCenterPosition = new Point2D();
        private Point2D _upCenterPosition = new Point2D();
        private Point2D _downCenterPosition = new Point2D();
        #region property
        public ObservableCollection<IAxis> Axes { get; set; }

        public double XWorkVel
        {
            get => _xWorkVel;
            set => SetProperty(ref _xWorkVel, value);
        }
      
        public double YWorkVel
        {
            get => _yWorkVel;
            set => SetProperty(ref _yWorkVel, value);
        }

        public double RWorkVel
        {
            get => _rWorkVel;
            set => SetProperty(ref _rWorkVel, value);
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
            get => _imprintPosition;
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
        public DelegateCommand GoHomeCommand => new DelegateCommand(GoHome);
        public DelegateCommand ResetAlarmCommand => new DelegateCommand(ResetAlarm);
        public DelegateCommand SaveParamCommand => new DelegateCommand(SaveParam);
        public DelegateCommand ReloadParamCommand => new DelegateCommand(ReloadParam);
        public DelegateCommand MoveToLoadPositionCommand => new DelegateCommand(MoveToLoadPosition);
        public DelegateCommand MoveToGluePositionCommand => new DelegateCommand(MoveToGluePosition);

        public DelegateCommand MoveToImprintPositionCommand => new DelegateCommand(MoveToImprintPosition);
        public DelegateCommand MoveToLeftCenterPositionCommand => new DelegateCommand(MoveToLeftCenterPosition);
        public DelegateCommand MoveToRightCenterPositionCommand => new DelegateCommand(MoveToRightCenterPosition);
        public DelegateCommand MoveToUpCenterPositionCommand => new DelegateCommand(MoveToUpCenterPosition);
        public DelegateCommand MoveToDownCenterPositionCommand => new DelegateCommand(MoveToDownCenterPosition);
        #endregion


        public MacroViewModel(IMachineModel machine)
        {
            _machine = machine;
            _macroPlatform = _machine.GetPlatform(typeof(MacroPlatform).Name) as MacroPlatform;
            Axes = new ObservableCollection<IAxis>();
            Axes.Add(_macroPlatform.XAxis);
            Axes.Add(_macroPlatform.YAxis);
            Axes.Add(_macroPlatform.RAxis);
            ReloadParam();
        }


        private void GoHome()
        {
            _macroPlatform.GoHome();
        }

        private void MoveToLoadPosition()
        {
            _macroPlatform.MoveToLoadPosition();
        }
        private void MoveToGluePosition()
        {
            _macroPlatform.MoveToGluePosition();
        }
        private void ResetAlarm()
        {
            _macroPlatform.XAxis.ResetAlarm();
            _macroPlatform.YAxis.ResetAlarm();
            _macroPlatform.RAxis.ResetAlarm();
        }

        private void SaveParam()
        {
            _machine.Config.MacroPlatform.XWorkVel = XWorkVel;
            _machine.Config.MacroPlatform.YWorkVel = YWorkVel;
            _machine.Config.MacroPlatform.RWorkVel = RWorkVel;
            _machine.Config.MacroPlatform.LoadPosition = LoadPosition;
            _machine.Config.MacroPlatform.GluePosition = GluePosition;
            _machine.Config.MacroPlatform.ImprintPosition = ImprintPosition;
            _machine.Config.MacroPlatform.LeftCenterPosition = LeftCenterPosition;
            _machine.Config.MacroPlatform.RightCenterPosition = RightCenterPosition;
            _machine.Config.MacroPlatform.UpCenterPosition = UpCenterPosition;
            _machine.Config.MacroPlatform.DownCenterPosition = DownCenterPosition;
            _machine.SaveParam();
        }

        private void ReloadParam()
        {
            XWorkVel = _machine.Config.MacroPlatform.XWorkVel;
            YWorkVel = _machine.Config.MacroPlatform.YWorkVel;
            RWorkVel = _machine.Config.MacroPlatform.RWorkVel;
            LoadPosition = _machine.Config.MacroPlatform.LoadPosition;
            GluePosition = _machine.Config.MacroPlatform.GluePosition;
            ImprintPosition = _machine.Config.MacroPlatform.ImprintPosition;
            LeftCenterPosition = _machine.Config.MacroPlatform.LeftCenterPosition;
            RightCenterPosition = _machine.Config.MacroPlatform.RightCenterPosition;
            UpCenterPosition = _machine.Config.MacroPlatform.UpCenterPosition;
            DownCenterPosition = _machine.Config.MacroPlatform.DownCenterPosition;
        }

        private void MoveToImprintPosition()
        {
            var task = Task.Run(() =>
            {
                _macroPlatform.MoveToImprintPosition();
            });
        }
        private void MoveToLeftCenterPosition()
        {
            var task = Task.Run(() =>
            {
                _macroPlatform.MoveTo(LeftCenterPosition);
            });
        }
        private void MoveToRightCenterPosition()
        {
            var task = Task.Run(() =>
            {
                _macroPlatform.MoveTo(RightCenterPosition);
            });
        }
        private void MoveToUpCenterPosition()
        {
            var task = Task.Run(() =>
            {
                _macroPlatform.MoveTo(UpCenterPosition);
            });
        }
        private void MoveToDownCenterPosition()
        {
            var task = Task.Run(() =>
            {
                _macroPlatform.MoveTo(DownCenterPosition);
            });
            
        }
    }
}
