//-----------------------------------------------------------------------
// <copyright file="CmdletParameterSetProcessor.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Generated on 1-13-2010
// </copyright>
// <summary>Implements the CmdletParameterSetProcessor classes.</summary>
//-----------------------------------------------------------------------
using System;

namespace Dot.Utility.PSWrapper
{
    #region CmdletParameterSetProcessor
    
    public class CmdletParameterSetProcessor : ICmdletProcessor
    {
        public CmdletParameterSetProcessor(System.Management.Automation.Runspaces.Runspace runspace)
        {
            this.runspace = runspace;
        }

        private System.Management.Automation.Runspaces.Runspace runspace;
        public System.Management.Automation.Runspaces.Runspace Runspace
        {
            get
            {
                return runspace;
            }
            protected set
            {
                runspace = value;
            }
        }

        public System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> RunCommand(CmdletParameterSetBase paramSet, out System.Collections.ObjectModel.Collection<object> errors)
        {
            errors = new System.Collections.ObjectModel.Collection<object>();
            System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> results;
            using (System.Management.Automation.Runspaces.Pipeline pipeline = runspace.CreatePipeline())
            {
                System.Management.Automation.Runspaces.Command cmd = new System.Management.Automation.Runspaces.Command(paramSet.Cmdlet);
                if (paramSet.Parameters != null)
                {
                    foreach (CmdletParameterSwitchValuePair parameter in paramSet.Parameters)
                    {
                        if (parameter.Value != null && parameter.Value.IsSet)
                        {
                            cmd.Parameters.Add(parameter.ParameterSwitch, parameter.Value.ValueObject);
                        }
                    }
                    pipeline.Commands.Add(cmd);
                }
                results = pipeline.Invoke();
                errors = pipeline.Error.ReadToEnd();
                //try
                //{
                //    results = pipeline.Invoke();
                //    return results;
                //}
                //catch (Exception Ex)
                //{
                //    errors = pipeline.Error.ReadToEnd();
                    
                //}
                //return null;
            
                
            }
            return results;
            
        }
    }

    #endregion
}

