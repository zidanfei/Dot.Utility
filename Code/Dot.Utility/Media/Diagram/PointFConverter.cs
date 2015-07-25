using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.ComponentModel.Design.Serialization;

namespace Dot.Utility.Media.Diagram
{
    internal sealed class PointFConverter : TypeConverter
    {
        public PointFConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if ((sourceType != typeof(string)) && (sourceType != typeof(InstanceDescriptor)))
            {
                return base.CanConvertFrom(context, sourceType);
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                char[] chArray1 = new char[1] { ',' };
                string[] textArray1 = ((string)value).Split(chArray1);
                return new PointF(float.Parse(textArray1[0], NumberFormatInfo.InvariantInfo), float.Parse(textArray1[1], NumberFormatInfo.InvariantInfo));
            }
            if (value is InstanceDescriptor)
            {
                InstanceDescriptor descriptor1 = (InstanceDescriptor)value;
                if (descriptor1.Arguments.Count == 2)
                {
                    object[] objArray1 = new object[2];
                    descriptor1.Arguments.CopyTo(objArray1, 0);
                    return new PointF((float)objArray1[0], (float)objArray1[1]);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is PointF)
            {
                PointF tf1 = (PointF)value;
                if (destinationType == typeof(string))
                {
                    return (tf1.X.ToString(NumberFormatInfo.InvariantInfo) + ", " + tf1.Y.ToString(NumberFormatInfo.InvariantInfo));
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    Type[] typeArray1 = new Type[2] { typeof(float), typeof(float) };
                    ConstructorInfo info1 = typeof(PointF).GetConstructor(typeArray1);
                    if (info1 != null)
                    {
                        object[] objArray1 = new object[2] { tf1.X, tf1.Y };
                        return new InstanceDescriptor(info1, objArray1, true);
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return new PointF((float)propertyValues["X"], (float)propertyValues["Y"]);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

    }
}
