//-----------------------------------------------------------------------
// <copyright file="PALCmdletInvocationException.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Generated on 1-13-2010
// </copyright>
// <summary>Implements the PALCmdletInvocationException classes.</summary>
//-----------------------------------------------------------------------


namespace Dot.Utility.PSWrapper
{
    
    public partial class PALCmdletInvocationException : System.Management.Automation.CmdletInvocationException
    {
        public PALCmdletInvocationException()
        {
        }

        public PALCmdletInvocationException(string message)
            : base(message)
        {
        }

        public PALCmdletInvocationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public PALCmdletInvocationException(string message, System.Collections.ObjectModel.Collection<System.Object> errors)
            : base(message)
        {
            this.Errors = errors;
        }

        private System.Collections.ObjectModel.Collection<System.Object> errors = null;
        public System.Collections.ObjectModel.Collection<System.Object> Errors
        {
            get
            {
                return this.errors;
            }
            set
            {
                this.errors = value;
            }
        }
    }
}

