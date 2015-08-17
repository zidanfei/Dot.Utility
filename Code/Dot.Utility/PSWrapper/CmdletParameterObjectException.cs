//-----------------------------------------------------------------------
// <copyright file="CmdletParameterObjectException.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Built on 6-24-2010
// </copyright>
// <summary>Implements the CmdletParameterObjectException classes.</summary>
//-----------------------------------------------------------------------


namespace Microsoft.Hosting.DPM.ServiceImplementation
{
    #region CmdletParameterObjectException

    public class CmdletParameterObjectException : System.Exception
    {

        public CmdletParameterObjectException()
            : base()
        {
        }

        public CmdletParameterObjectException(string message)
            : base(message)
        {
        }

        protected CmdletParameterObjectException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public CmdletParameterObjectException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }

    #endregion
}

