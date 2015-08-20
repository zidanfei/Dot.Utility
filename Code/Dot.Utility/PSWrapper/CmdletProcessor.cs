//-----------------------------------------------------------------------
// <copyright file="CmdletProcessor.cs" company="Microsoft">
//     Copyright 2010 (c) Microsoft Corporation.  All rights reserved.
//     Built on 6-24-2010
// </copyright>
// <summary>Implements the CmdletProcessor classes.</summary>
//-----------------------------------------------------------------------



namespace Dot.Utility.PSWrapper
{
    #region CmdletProcessor    
    /// <summary>
    /// Power Shell ¥¶¿Ì¿‡
    /// </summary>
    //[System.CLSCompliant(false)]
    public class CmdletProcessor : System.IDisposable
    {
        private static CmdletProcessor defaultCmdletProcessor;
        public static CmdletProcessor DefaultCmdletProcessor
        {
            get
            {
                return defaultCmdletProcessor;
            }
            set
            {
                defaultCmdletProcessor = value;
            }
        }

        public CmdletProcessor(string psSnapinName, out System.Management.Automation.Runspaces.PSSnapInException warning)
        {
            System.Management.Automation.Runspaces.RunspaceConfiguration runspaceConfig = System.Management.Automation.Runspaces.RunspaceConfiguration.Create();
            runspaceConfig.AddPSSnapIn(psSnapinName, out warning);
            this.runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(runspaceConfig);
            this.SetupRunspace();
        }

        public CmdletProcessor(string consoleFilePath, out System.Management.Automation.Runspaces.PSConsoleLoadException warnings)
        {
            this.runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(System.Management.Automation.Runspaces.RunspaceConfiguration.Create(consoleFilePath, out warnings));
            this.SetupRunspace();
        }

        public CmdletProcessor(string assemblyName)
        {
            this.runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(System.Management.Automation.Runspaces.RunspaceConfiguration.Create(assemblyName));
            this.SetupRunspace();
        }

        public CmdletProcessor(System.Management.Automation.Runspaces.Runspace runspace)
        {
            this.runspace = runspace;
            this.SetupRunspace();
        }

        public CmdletProcessor()
        {
            this.runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(System.Management.Automation.Runspaces.RunspaceConfiguration.Create());
            this.SetupRunspace();
        }

        private void SetupRunspace()
        {
            runspace.Open();
            cmdletParameterSetProcessor = new CmdletParameterSetProcessor(runspace);
            cmdletStringProcessor = new CmdletStringProcessor(runspace);
        }

        private CmdletParameterSetProcessor cmdletParameterSetProcessor;
        private CmdletStringProcessor cmdletStringProcessor;

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

        public T[] RunCmdlet<T>(CmdletParameterSetBase paramSet)
        {
            System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> results = this.ProcessCmdletCall(paramSet);
            // powershell sometimes returns null PSObject objects, 
            // so we can't rely on the .Count property of the results 
            // collection for an accurate count of the results and we 
            // need to count them ourselves
            System.Collections.Generic.List<T> retVal = new System.Collections.Generic.List<T>(results.Count); // can never be larger than the results colleciton
            foreach (System.Management.Automation.PSObject result in results)
            {
                if (result != null && result.BaseObject is T)
                {
                    retVal.Add((T)result.BaseObject);
                }
            }
            return retVal.ToArray();
        }

        private System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> ProcessCmdletCall(CmdletParameterSetBase paramSet)
        {
            System.Collections.ObjectModel.Collection<object> errors;
            ICmdletProcessor iCmdletProcessor = GetCmdletProcessor(paramSet);
            System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> results = iCmdletProcessor.RunCommand(paramSet, out errors);
            if (errors.Count > 0)
            {
                HandleErrors(errors, paramSet);
            }
            return results;
        }

        private ICmdletProcessor GetCmdletProcessor(CmdletParameterSetBase paramSet)
        {
            ICmdletProcessor retVal = this.cmdletParameterSetProcessor;
            if (paramSet.Parameters != null)
            {
                foreach (CmdletParameterSwitchValuePair parameter in paramSet.Parameters)
                {
                    if (parameter.Value.IsSet &&
                        parameter.Value.ParameterValueType == ParameterValueTypeEnum.String)
                    {
                        retVal = this.cmdletStringProcessor;
                        break;
                    }
                }
            }
            return retVal;
        }

        private void HandleErrors(System.Collections.ObjectModel.Collection<object> errors, CmdletParameterSetBase paramSet)
        {
            if (errors.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine(System.String.Format("Errors were reported by powershell while running the {0} cmdlet. Check the exception Data property for more details. A list of the errors follows:\n", paramSet.Cmdlet));
                for (int i = 0; i < errors.Count; i++)
                {
                    sb.AppendLine("\t" + errors[i].ToString());
                }
                Log.LogFactory.ExceptionLog.Error(string.Format("Cmdlet:{0}{1}error:{2}", paramSet.Cmdlet, System.Environment.NewLine, sb.ToString()));
                throw new PALCmdletInvocationException(sb.ToString(), errors);
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (this.runspace != null)
            {
                runspace.Close();
                runspace.Dispose();
                runspace = null;
            }
        }

        #endregion
    }
    #endregion
}
