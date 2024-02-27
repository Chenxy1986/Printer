﻿using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static WestLakeShape.Motion.IOStateSource;

namespace NanoImprinter.ViewModels
{
    public class OtherViewModel
    {
        private readonly IMachineModel _machine;
        private readonly IRefreshDataService _refreshDataService;
        public DelegateCommand<string> SetOutputValueCommand => new DelegateCommand<string>(SetOutputValue);

        public ObservableCollection<StateValue> InputIOs { get; private set; }
        public ObservableCollection<StateValue> OutputIOs { get;  set; }
        public OtherViewModel(IMachineModel machine, IRefreshDataService refreshDataService)
        {
            _machine = machine;
            InputIOs = new ObservableCollection<StateValue>();
            OutputIOs = new ObservableCollection<StateValue>();
            Initial();
            _refreshDataService = refreshDataService;
            refreshDataService.Register(RefreshState);
        }

        ~OtherViewModel()
        {
            _refreshDataService.Unregister(RefreshState);
        }

        private void Initial()
        {
            var states = _machine.IOStates;
            foreach (ImprinterIOName name in Enum.GetValues(typeof(ImprinterIOName)))
            {
                var state =states.GetInputIO(name);
                if (state.ReadOnly)
                    InputIOs.Add(new StateValue(state));
                else
                    OutputIOs.Add(new StateValue(state));
            }
        }


        private void RefreshState()
        {
            var states = _machine.IOStates;
            
            foreach (var stateValue in InputIOs)
            {
                var stateName = (ImprinterIOName)Enum.Parse(typeof(ImprinterIOName), stateValue.Name);
                stateValue.IsOn = states.GetInputIO(stateName).State;
            }

            foreach (var stateValue in OutputIOs)
            {
                var stateName = (ImprinterIOName)Enum.Parse(typeof(ImprinterIOName), stateValue.Name);
                stateValue.IsOn = states.GetInputIO(stateName).State;
            }
        }

        private void SetOutputValue(string stateName)
        {
            var stateValue = OutputIOs.Where(o => o.Name == stateName).FirstOrDefault().IsOn;
            var ioName = (ImprinterIOName)Enum.Parse(typeof(ImprinterIOName), stateName);
           _machine.IOStates.GetInputIO(ioName).Set(stateValue);
        }
    }


    public class StateValue: BindableBase
    {
        private string _name;
        private bool _isOn;
        private string _test;
        public StateValue(IOState ioState)
        {
            _name = ioState.Name;
            _isOn = ioState.State;
            _test = _name;
        }
        public string Name 
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }
        public string Test
        {
            get => _test;
            set => SetProperty(ref _test, value);
        }
        public bool IsOn
        {
            get => _isOn;
            set => SetProperty(ref _isOn, value);
        }
    }
}
