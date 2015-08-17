//-----------------------------------------------------------------------
// <copyright file="ICmdletParameter.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Built on 6-24-2010
// </copyright>
// <summary>Implements the ICmdletParameter classes.</summary>
//-----------------------------------------------------------------------


namespace Microsoft.Hosting.DPM.ServiceImplementation
{
    #region ICmdletParameter

    public interface ICmdletParameter<T>
    {
        T Value { get; set; }
        string ValueString { get; set; }
    }

    #endregion
}

