//-----------------------------------------------------------------------
// <copyright file="CmdletStringProcessor.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Built on 6-24-2010
// </copyright>
// <summary>Implements the CmdletStringProcessor classes.</summary>
//-----------------------------------------------------------------------


namespace Dot.Utility.PSWrapper
{
    #region CmdletStringProcessor
    //[System.CLSCompliant(false)]
    public class CmdletStringProcessor : ICmdletProcessor
    {
        public CmdletStringProcessor(System.Management.Automation.Runspaces.Runspace runspace)
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
            System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> results = null;

            System.Text.StringBuilder cmdString = new System.Text.StringBuilder(paramSet.Cmdlet);
            System.Text.StringBuilder parameterValueClearScriptBuilder = new System.Text.StringBuilder();
            foreach (CmdletParameterSwitchValuePair parameter in paramSet.Parameters)
            {
                if (parameter.Value != null && parameter.Value.IsSet)
                {
                    if (parameter.Value is CmdletSwitchParameter)
                    {
                        cmdString.AppendFormat(" -{0}", parameter.ParameterSwitch);
                    }
                    else
                    {
                        string parameterSting = System.String.Empty, parameterGuidString;
                        switch (parameter.Value.ParameterValueType)
                        {
                            case ParameterValueTypeEnum.String:
                                parameterSting = (string)parameter.Value.ValueObject;
                                break;
                            case ParameterValueTypeEnum.Object:
                                parameterGuidString = System.Guid.NewGuid().ToString("N");
                                parameterSting = System.String.Format("${0}", parameterGuidString);
                                runspace.SessionStateProxy.SetVariable(parameterGuidString, parameter.Value.ValueObject);
                                parameterValueClearScriptBuilder.AppendLine(System.String.Format("{0} = $NULL", parameterSting));
                                break;
                            default:
                                System.Diagnostics.Debug.Assert(false, "Parameter object is not set properly. This error should never be seen.");
                                break;
                        }
                        cmdString.AppendFormat(" -{0} {1}", parameter.ParameterSwitch, parameterSting);
                    }
                }
                try
                {
                    using (System.Management.Automation.Runspaces.Pipeline pipeline = runspace.CreatePipeline(cmdString.ToString()))
                    {
                        results = pipeline.Invoke();
                        errors = pipeline.Error.ReadToEnd();
                    }
                }
                finally
                {
                    using (System.Management.Automation.Runspaces.Pipeline pipeline = runspace.CreatePipeline(parameterValueClearScriptBuilder.ToString()))
                    {
                        pipeline.Invoke();
                    }
                }
            }
            return results;
        }
    }
    #endregion
}

