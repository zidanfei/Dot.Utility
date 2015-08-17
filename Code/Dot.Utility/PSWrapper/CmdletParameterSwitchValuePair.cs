//-----------------------------------------------------------------------
// <copyright file="CmdletParameterSwitchValuePair.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Generated on 1-13-2010
// </copyright>
// <summary>Implements the CmdletParameterSwitchValuePair classes.</summary>
//-----------------------------------------------------------------------


namespace Dot.Utility.PSWrapper
{
    #region CmdletParameterSwitchValuePair

    public sealed class CmdletParameterSwitchValuePair
    {
        public readonly string ParameterSwitch;
        public ParameterBase Value;

        public CmdletParameterSwitchValuePair(string parameterSwitch, ParameterBase value)
        {
            this.ParameterSwitch = parameterSwitch;
            this.Value = value;
        }
    }

    #endregion
}

