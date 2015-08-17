//-----------------------------------------------------------------------
// <copyright file="CmdletStringParameter.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Built on 6-24-2010
// </copyright>
// <summary>Implements the CmdletStringParameter classes.</summary>
//-----------------------------------------------------------------------


using Dot.Utility.PSWrapper;
namespace Microsoft.Hosting.DPM.ServiceImplementation
{
    #region CmdletStringParameter

    public sealed class CmdletStringParameter : ParameterBase, ICmdletParameter<string>
    {
        public CmdletStringParameter()
            : base()
        {
            this.valueString = null;
        }

        public CmdletStringParameter(string strVal)
            : base()
        {
            this.valueString = strVal;
            this.IsSet = true;
            this.ParameterValueType = ParameterValueTypeEnum.Object;
        }

        public static implicit operator CmdletStringParameter(string rhs)
        {
            return new CmdletStringParameter(rhs);
        }

        public static implicit operator string(CmdletStringParameter rhs)
        {
            return rhs.ValueString;
        }

        public string Value
        {
            get
            {
                return this.ValueString;
            }
            set
            {
               this.ValueString = value;
            }
        }

        private string valueString;
        public string ValueString
        {
            get
            {
                if (this.ParameterValueType != ParameterValueTypeEnum.Object)
                {
                    throw new CmdletParameterObjectException("String not set");
                }
                return this.valueString;
            }
            set
            {
                this.valueString = value;
                this.ParameterValueType = ParameterValueTypeEnum.Object;
                this.IsSet = true;
            }
        }

        public override object ValueObject
        {
            get
            {
                object retVal;
                switch (this.ParameterValueType)
                {
                    case ParameterValueTypeEnum.Object:
                        retVal = this.valueString;
                        break;
                    default:
                        retVal = null;
                        break;
                }
                return retVal;
            }
        }
    }

    #endregion
}

