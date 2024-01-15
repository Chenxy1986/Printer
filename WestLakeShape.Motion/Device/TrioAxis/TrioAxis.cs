using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrioMotion.TrioPC_NET;
using WestLakeShape.Common.Common;

namespace WestLakeShape.Motion.Device
{
    public class TrioAxis : Axis<TrioAxisConfig>
    {
        private readonly TimeSpan _startWait = TimeSpan.FromMilliseconds(5);
        
        private static TrioPC _trioPC;
        private TrioAxisConfig _config;
        private Movement _currentMovement;
        private AxisState _state;

        public int Index => _config.Index;
        public override double Position => _state.Position;
        public override double Speed => _state.Speed;
        public bool IsBusy => _state.IsBusy;
        public string Name => _config.Name;


        public TrioAxis(TrioAxisConfig config) : base(config)
        {
            _trioPC = TrioControl.Instance.TrioPC;
            _config = config;
            _state = new AxisState();
            Initial();
        }


        public void ReloadConfig(TrioAxisConfig newConfig)
        {
            _config = newConfig.DeepClone();
            Initial();
        }


        /// <summary>
        /// 回零
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> GoHome()
        {
            if (_config.HomeModel == TrioHomeModel.MoveToZero)
            {
                return await MoveTo(0).ConfigureAwait(false);
            }
            else
            {
                //先停止轴运动
                var ret = _trioPC.Execute("RAPIDSTOP(2)");
                //执行回零动作
                _trioPC.Datum((int)(_config.HomeModel), _config.Index);
                //等待回零完成
                return await WaitGoHome().ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 绝对移动
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override async Task<bool> MoveTo(double position)
        {
            await Stop().ConfigureAwait(false);
  
            var movement = new Movement(position);
            _currentMovement = movement;

            var rt = _trioPC.MoveAbs(new double[] { position }, _config.Index);
            CheckException(rt);

            return await movement.TaskCompletionSource.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<bool> MoveBy(double position) 
        {
            await Stop().ConfigureAwait(false);

            var movement = new Movement(position);
            _currentMovement = movement;

            var rt = _trioPC.MoveRel(new double[] { position }, _config.Index);
            CheckException(rt);

            return await movement.TaskCompletionSource.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// 以jog速度持续移动
        /// </summary>
        /// <param name="jogSpeed"></param>
        /// <param name="isForward"></param>
        public void ContinueMove(double jogSpeed, bool isForward)
        {
            SetAxisParameter(AxisParameter.JOGSPEED, jogSpeed);
            if (isForward)
                _trioPC.Forward(_config.Index);
            else
                _trioPC.Reverse(_config.Index);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public override async Task Stop()
        {
            if (!IsBusy)
                return;

            var rt = _trioPC.Cancel(0, _config.Index);
            CheckException(rt);

            var movement = _currentMovement;
            _currentMovement = null;
            if (movement != null)
            {
                if (movement.TaskCompletionSource.TrySetResult(false))
                    throw new Exception("运动终止失败");
            }

            while (IsBusy)
                await Task.Delay(5).ConfigureAwait(false);
        }

        /// <summary>
        /// 急停
        /// </summary>
        public void EmegercyStop()
        {
            _trioPC.Cancel(2, _config.Index);
        }

        public Task RefreshState()
        {
            return Task.Run(() =>
            {
                double positionValue,speedValue;
                double moveValue, stopValue;

                //获取目标位置
                //GetAxisParameter(AxisParameter.DPOS, out targetPostion);

                //当前的速度和位置
                GetAxisParameter(AxisParameter.MPOS, out positionValue);
                GetAxisParameter(AxisParameter.MSPEED, out speedValue);
                //运动否已停止
                GetAxisParameter(AxisParameter.IDLE, out stopValue);
                //当前的运动指令
                GetAxisParameter(AxisParameter.MTYPE, out moveValue);

                _state.Speed = speedValue;
                _state.Position = positionValue;
                _state.Command = (TrioMtypeValue)moveValue;
                //0(false)代表正在移动
                _state.IsBusy = stopValue == 0 ? true : false;


                var movement = _currentMovement;
                if (movement != null)
                {
                    if (DateTime.UtcNow - movement.StartTimeUtc > _startWait)
                    {
                        if (!_state.IsBusy &&
                           Math.Abs(positionValue - movement.TargetPostion) <= 10)
                        {
                            if (!movement.TaskCompletionSource.TrySetResult(true))
                                throw new Exception("轴运动完成赋值出错");
                            lock (_state)
                            {
                                if (movement == _currentMovement)
                                    _currentMovement = null;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 清除报警
        /// </summary>
        public void ResetAlarm()
        {
            _trioPC.Datum(0);
            var command = $"datum({_config.Index})";
            _trioPC.Execute(command);
        }

        public string GetErrorCode()
        {
            double errorCodeValue;
            //获取状态
            GetAxisParameter(AxisParameter.AXISSTATUS, out errorCodeValue);

            string errorMsg = $"错误码:{errorCodeValue}; ";
            var errorCode = (TrioErroCode)errorCodeValue;

            foreach (TrioErroCode flag in Enum.GetValues(typeof(TrioErroCode)))
            {
                if (errorCode.HasFlag(flag))
                    errorMsg += GetErrorCode(flag);
            }

            return errorMsg;
        }

        public override void ServoOff()
        {
            var ret = _trioPC.SetVariable(TrioParamName.Enable, 0);
            CheckException(ret);
        }
        public override void ServoOn()
        {
            var ret = _trioPC.SetVariable(TrioParamName.Enable, 1);
            CheckException(ret);
        }

        private async Task<bool> WaitGoHome()
        {
            var commValue = TrioMtypeValue.Datum;
            double typeValue = 1;

            while (commValue == TrioMtypeValue.Datum)
            {
                await Task.Delay(50);
                //获取当前运动指令
                GetAxisParameter(AxisParameter.MTYPE, out typeValue);

                typeValue = Math.Round(typeValue, 0);
                Enum.TryParse(typeValue.ToString(), out commValue);

                //回零后将vr101置2
                if (commValue == TrioMtypeValue.Idle)
                    _trioPC.SetVr(101, 2);
            }
            return true;
        }

        /// <summary>
        /// 写入轴默认参数
        /// </summary>
        private void Initial()
        {
            SetAxisParameter(AxisParameter.UNITS, _config.PlusEquivalent);
            SetAxisParameter(AxisParameter.ACCEL,  _config.Acc);
            SetAxisParameter(AxisParameter.DECEL, _config.Dec);
            SetAxisParameter(AxisParameter.SPEED, _config.Speed);
            SetAxisParameter(AxisParameter.JOGSPEED, 100);
            SetAxisParameter(AxisParameter.CREEP, 10);
        }

        /// <summary>
        /// 设置轴参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void SetAxisParameter(AxisParameter name, double value)
        {
            var ret = _trioPC.SetAxisParameter(name, _config.Index, value);
            CheckException(ret);
        }

        /// <summary>
        /// 获取轴参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void GetAxisParameter(AxisParameter name, out double value)
        {
            var ret= _trioPC.GetAxisParameter(name, _config.Index, out value);
            CheckException(ret);
        }

        //private void SetAxisVariable(string name,double value)
        //{
        //    _trioControl.SetAxisVariable(name, _config.Index, value);
        //}

        //private void GetAxisVariable(string name, out double value) 
        //{
        //    _trioControl.GetAxisVariable(name, _config.Index, out value);
        //}

        /// <summary>
        /// 获取报警说明
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        private static string GetErrorCode(TrioErroCode errorCode)
        {
            var file = errorCode.GetType().GetField(errorCode.ToString());
            var attributs = (DescriptionAttribute[])file.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributs != null && attributs.Length > 0)
                return attributs[0].ToString();
            else
                return errorCode.ToString();
        }

        private static void CheckException(bool ret)
        {
            if (!ret)
                throw new Exception("dll的Api函数使用报错");
        }

        class Movement
        {
            public Movement(double position)
            {
                TargetPostion = position;
                StartTimeUtc = DateTime.UtcNow;
                TaskCompletionSource = new TaskCompletionSource<bool>();
            }

            public double TargetPostion { get; private set; }

            public DateTime StartTimeUtc { get; private set; }

            public TaskCompletionSource<bool> TaskCompletionSource { get; private set; }
        }

        public class AxisState
        {

            public double Position { get; internal set; }

            public double Speed { get; internal set; }

            public TrioMtypeValue Command { get; internal set; }
            public bool IsBusy { get; set; }
        }
    }


    public class TrioAxisConfig: AxisConfig
    {
        [Category("TrioAxis"), Description("脉冲当量"), DefaultValue(10)]
        [DisplayName("脉冲当量")]
        public double PlusEquivalent { get; set; }

        [Category("TrioAxis"), Description("加速度"), DefaultValue(100)]
        [DisplayName("加速度")]
        public double Acc { get; set; }
        
        [Category("TrioAxis"), Description("减速度"), DefaultValue(100)]
        [DisplayName("减速度")]
        public double Dec { get; set; }

        [Category("TrioAxis"), Description("启动速度"), DefaultValue(1000)]
        [DisplayName("启动速度")]
        public double StartSpeed { get; set; }

        [Category("TrioAxis"), Description("工作速度"), DefaultValue(1000)]
        [DisplayName("工作速度")]
        public double WorkSpeed { get; set; }

        [Category("TrioAxis"), Description("回零方式"), DefaultValue(TrioHomeModel.MoveToZero)]
        [DisplayName("回零方式")]
        public TrioHomeModel HomeModel { get; set; }

        [Category("TrioAxis"), Description("回零蠕动速度"), DefaultValue(10)]
        [DisplayName("回零蠕动速度")]
        public double Creep { get; set; }
    }
}
