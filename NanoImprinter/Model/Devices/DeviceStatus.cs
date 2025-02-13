﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter
{
    /// <summary>
    /// 当前尚没有想好如何
    /// </summary>
    public enum DeviceStatus
    {
        [Description("Orange")]
        Value1,

        [Description("Green")]
        Value2,

        [Description("Blue")]
        Value3
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
            return descriptionAttribute.Description;
        }
    }
}
