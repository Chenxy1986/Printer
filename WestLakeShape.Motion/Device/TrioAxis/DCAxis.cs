﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrioMotion.TrioPC_NET;

namespace WestLakeShape.Motion.Device
{
    /// <summary>
    /// 临时添加的直流电机
    /// </summary>
    public class DCAxis : TrioAxis
    {
        private DCAxisConfig _config;
        public DCAxis(DCAxisConfig config) : base(config)
        {
            _config = config;
        }

        public override void ServoOn()
        {
            var rt = _trioPC.CoWrite(0,(short)(_config.Index + 1), 0x60FE, 2, 7, 0X10000);
            CheckException(rt);
            rt = _trioPC.CoWrite(0, (short)(_config.Index + 1), 0x60FE, 1, 7, 0X0);//输出抱闸
            CheckException(rt);
            rt = _trioPC.SetAxisVariable(TrioParamName.ServoOn, (short)_config.Index, 1);//上使能
            CheckException(rt);
            rt = _trioPC.CoWrite(0, (short)(_config.Index + 1), 0x60FE, 1, 7, 0X10000);//关闭抱闸
            CheckException(rt);

            rt = _trioPC.SetAxisParameter(AxisParameter.SERVO, _config.Index, 1); //所有轴打开编码器
            CheckException(rt);
            GetState();
        }
        public override void InitialParameter()
        {
            base.InitialParameter();
            
            //完成直流电机特有的参数初始化
            if (_config.IsFwdEnable)
            {
                _trioPC.SetAxisParameter(AxisParameter.FWD_IN, _config.Index, _config.FwdIndex);
            }
            if (_config.IsRevEnable)
            {
                _trioPC.SetAxisParameter(AxisParameter.REV_IN, _config.Index, _config.RevIndex);
            }        
        }


        public override void ServoOff()
        {
            var rt = _trioPC.CoWrite(0, 1, 0x60FE, 2, 7, 0X10000);
            CheckException(rt);
            rt = _trioPC.CoWrite(0, 1, 0x60FE, 1, 7, 0X0);//输出抱闸
            CheckException(rt);
            rt = _trioPC.SetAxisVariable("AXIS_ENABLE", 0, 0);//断使能
            CheckException(rt);
            rt = _trioPC.CoWrite(0, 1, 0x60FE, 1, 7, 0);//关闭抱闸
            CheckException(rt);
        }

        public override bool GoHome()
        {
            double value;
            double fwdIOIndex;
            double datumIOIndex;
            _trioPC.GetAxisParameter(AxisParameter.AXISSTATUS, _config.Index, out value);//获取轴的回零模式
            
            switch (_config.HomeModel)
            {
                //正向回原到正限位
                case TrioHomeModel.PositiveAndLimit:
                    _trioPC.GetAxisParameter(AxisParameter.FWD_IN, _config.Index, out fwdIOIndex);// 获取正限位输入IO端口号

                    _trioPC.GetAxisParameter(AxisParameter.DATUM_IN, _config.Index, out datumIOIndex);// 获取原点的输入IO端口号

                    _trioPC.SetAxisParameter(AxisParameter.FWD_IN, _config.Index, -1);          //取消正限位关联IO
                    
                    _trioPC.SetAxisParameter(AxisParameter.DATUM_IN, _config.Index, fwdIOIndex);//原正限位关联IO作为原点IO
                    
                    _trioPC.Datum(3, _config.Index);//开始回零
                    
                    _trioPC.GetAxisParameter(AxisParameter.MTYPE, _config.Index, out value);//获取轴回零状态

                    while (value == 0) //判断回零完成
                    {
                        _trioPC.GetAxisParameter(AxisParameter.MTYPE, _config.Index, out value);
                    }

                    //System.Threading.Thread.Sleep(2);
                    _trioPC.GetAxisParameter(AxisParameter.AXISSTATUS, _config.Index, out value);//获取轴位置到达信号

                    while (((int)value >> 6) == 1)
                    {
                        _trioPC.GetAxisParameter(AxisParameter.AXISSTATUS, _config.Index, out value);

                    }

                    _trioPC.SetAxisParameter(AxisParameter.DATUM_IN, _config.Index, datumIOIndex);  //恢复原点IO设置
                    _trioPC.SetAxisParameter(AxisParameter.FWD_IN, _config.Index, fwdIOIndex);      //恢复原点IO设置
                    //MoveTo(0);
                    break;
            }
            _trioPC.SetAxisParameter(AxisParameter.FS_LIMIT, _config.Index, _config.SoftPositiveDistance);
            _trioPC.SetAxisParameter(AxisParameter.RS_LIMIT, _config.Index, _config.SoftNegativeDistance);
            return true;
        }

    }


    public class DCAxisConfig : TrioAxisConfig
    {
        [Category("DCAxis"), Description("正限位IO索引"), DefaultValue(10)]
        [DisplayName("正限位IO索引")]
        public double FwdIndex { get; set; }


        [Category("DCAxis"), Description("负限位IO索引"), DefaultValue(10)]
        [DisplayName("负限位IO索引")]
        public double RevIndex { get; set; }
        

        [Category("DCAxis"), Description("存在负限位"), DefaultValue(10)]
        [DisplayName("存在负限位")]
        public bool IsFwdEnable { get; set; }


        [Category("DCAxis"), Description("存在正限位"), DefaultValue(10)]
        [DisplayName("存在正限位")]
        public bool IsRevEnable { get; set; }

        [Category("DCAxis"), Description("软正限位值，行程值"), DefaultValue(10)]
        [DisplayName("软正限位值")]
        public double SoftPositiveDistance { get; set; }

        [Category("DCAxis"), Description("软正限位值，行程值"), DefaultValue(10)]
        [DisplayName("软负限位")]
        public double SoftNegativeDistance { get; set; }
    }
}
