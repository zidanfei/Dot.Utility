//-----------------------------------------------------------------------
// <copyright file="ICmdletProcessor.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Generated on 1-13-2010
// </copyright>
// <summary>Implements the ICmdletProcessor classes.</summary>
//-----------------------------------------------------------------------


namespace Dot.Utility.PSWrapper
{
    #region ICmdletProcessor

    interface ICmdletProcessor
    {
        System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> RunCommand(CmdletParameterSetBase paramSet, out System.Collections.ObjectModel.Collection<object> errors);
        System.Management.Automation.Runspaces.Runspace Runspace { get; }
    }

    #endregion
}

