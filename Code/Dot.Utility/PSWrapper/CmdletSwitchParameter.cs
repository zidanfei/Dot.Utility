//-----------------------------------------------------------------------
// <copyright file="CmdletSwitchParameter.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Generated on 1-13-2010
// </copyright>
// <summary>Implements the CmdletSwitchParameter classes.</summary>
//-----------------------------------------------------------------------


namespace Dot.Utility.PSWrapper
{
    #region SwitchParameter

    public sealed class CmdletSwitchParameter : ParameterBase
    {
        public CmdletSwitchParameter()
            : this(false)
        {
        }

        public CmdletSwitchParameter(bool val)
            : base()
        {
            this.IsSet = val;
            this.IsSwitch = true;
        }

        public static implicit operator CmdletSwitchParameter(bool rhs)
        {
            return new CmdletSwitchParameter(rhs);
        }

        public static implicit operator bool(CmdletSwitchParameter rhs)
        {
            return rhs.IsSet;
        }

        public override object ValueObject
        {
            get {
                object retVal;
                if (this.IsSet)
                {
                    retVal = true;
                }
                else
                {
                    retVal = null;
                }
                return retVal;
            }
        }
    }

    #endregion
}

