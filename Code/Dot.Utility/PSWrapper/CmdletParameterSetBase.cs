//-----------------------------------------------------------------------
// <copyright file="CmdletParameterSetBase.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Generated on 1-13-2010
// </copyright>
// <summary>Implements the CmdletParameterSetBase classes.</summary>
//-----------------------------------------------------------------------



using System.Collections.Generic;
namespace Dot.Utility.PSWrapper
{
    #region CmdletParameterSetBase

    public class CmdletParameterSetBase
    {
        public CmdletParameterSetBase(string cmdletName)
        {
            this.cmdletName = cmdletName;
        }

        private List<CmdletParameterSwitchValuePair> parameters;
        public CmdletParameterSwitchValuePair[] Parameters
        {
            get
            {
                return this.parameters.ToArray();
            }           
        }

        public CmdletParameterSetBase AddParameter(CmdletParameterSwitchValuePair pair)
        {
            if (parameters == null)
                parameters = new List<CmdletParameterSwitchValuePair>();
            if (!parameters.Contains(pair))
                parameters.Add(pair);
            return this;
        }

        private string cmdletName;
        public string Cmdlet
        {
            get
            {
                return cmdletName;
            }
            protected set
            {
                cmdletName = value;
            }
        }

        public DefaultReturnType[] RunCmdlet<DefaultReturnType>()
        {
            return CmdletProcessor.DefaultCmdletProcessor.RunCmdlet<DefaultReturnType>(this);
        }


        public DefaultReturnType[] RunCmdlet<DefaultReturnType>(CmdletProcessor cmdletProcessor)
        {
            return cmdletProcessor.RunCmdlet<DefaultReturnType>(this);
        }
    }

    #endregion
}

