using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Services.Description;
using System.Xml.Serialization;


namespace Dot.Utility.Service
{
    public class WebServiceInvoker
    {
        public WebServiceInvoker(string Url, string NameSpace, string ClassName)
        {
            this._Url = Url;
            this._NameSpace = NameSpace;
            this._ClassName = ClassName;
        }

        public WebServiceInvoker(string Url, string NameSpace, string ClassName, string UserName, string Password, string Domain)
        {
            this._Url = Url;
            this._NameSpace = NameSpace;
            this._ClassName = ClassName;
            this._UserName = UserName;
            this._Password = Password;
            this._Domain = Domain;
        }

        private string _Url = null;
        public string Url
        {
            get
            {
                return this._Url;
            }
        }

        private string _NameSpace = null;
        public string NameSpace
        {
            get
            {
                return this._NameSpace;
            }
        }

        private string _ClassName = null;
        public string ClassName
        {
            get
            {
                return this._ClassName;
            }
        }

        private string _UserName = null;
        public string UserName
        {
            get
            {
                return this._UserName;
            }
        }

        private string _Password = null;
        public string Password
        {
            get
            {
                return this._Password;
            }
        }

        private string _Domain = null;
        public string Domain
        {
            get
            {
                return this._Domain;
            }
        }

        private System.Type _ProxyType = null;
        public System.Type ProxyType
        {
            get
            {
                if (this._ProxyType == null)
                {
                    try
                    {
                        System.Net.WebClient client = new System.Net.WebClient();
                        client.Credentials = this.Credentials;
                        System.IO.Stream stream = client.OpenRead(this.Url + "?WSDL");

                        System.Web.Services.Description.ServiceDescription description = System.Web.Services.Description.ServiceDescription.Read(stream);

                        System.Web.Services.Description.ServiceDescriptionImporter importer = new System.Web.Services.Description.ServiceDescriptionImporter();
                        importer.ProtocolName = "Soap";
                        importer.Style = ServiceDescriptionImportStyle.Client;
                        importer.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync;
                        importer.AddServiceDescription(description, null, null);

                        System.CodeDom.CodeNamespace space = new System.CodeDom.CodeNamespace(NameSpace);
                        System.CodeDom.CodeCompileUnit unit = new System.CodeDom.CodeCompileUnit();
                        unit.Namespaces.Add(space);
                        importer.Import(space, unit);

                        Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
                        System.CodeDom.Compiler.ICodeCompiler compiler = provider.CreateCompiler();

                        System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();
                        parameters.GenerateExecutable = false;
                        parameters.GenerateInMemory = true;
                        parameters.ReferencedAssemblies.Add("System.dll");
                        parameters.ReferencedAssemblies.Add("System.XML.dll");
                        parameters.ReferencedAssemblies.Add("System.Web.Services.dll");
                        parameters.ReferencedAssemblies.Add("System.Data.dll");

                        System.CodeDom.Compiler.CompilerResults results = compiler.CompileAssemblyFromDom(parameters, unit);
                        if (true == results.Errors.HasErrors)
                        {
                            System.Text.StringBuilder builder = new System.Text.StringBuilder();
                            foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                            {
                                builder.Append(error.ToString());
                                builder.Append(System.Environment.NewLine);
                            }
                            throw new Exception(builder.ToString());
                        }
                        System.Reflection.Assembly assembly = results.CompiledAssembly;
                        this._ProxyType = assembly.GetType(NameSpace + "." + ClassName, true, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Can't get the definition of the web service, Url={0}, User={1}, Password={2}, Domain={3}", this.Url, this.UserName, this.Password, this.Domain), ex);
                    }
                }
                return this._ProxyType;
            }
        }

        private System.Web.Services.Protocols.SoapHttpClientProtocol _ProxyObject;
        public System.Web.Services.Protocols.SoapHttpClientProtocol ProxyObject
        {
            get
            {
                if (this._ProxyObject == null)
                {
                    this._ProxyObject = (System.Web.Services.Protocols.SoapHttpClientProtocol)Activator.CreateInstance(this.ProxyType);
                    this._ProxyObject.Credentials = this.Credentials;
                }
                return this._ProxyObject;
            }
        }

        private ICredentials Credentials
        {
            get
            {
                if (UserName == null || UserName == "")
                {
                    return System.Net.CredentialCache.DefaultCredentials;
                }
                else if (Domain == null || Domain == "")
                {
                    return new System.Net.NetworkCredential(this.UserName, this.Password);
                }
                else
                {
                    return new System.Net.NetworkCredential(this.UserName, this.Password, this.Domain);
                }
            }
        }


        public object Invoke(string MethodName, object[] Args)
        {
            System.Reflection.MethodInfo method = this.ProxyType.GetMethod(MethodName);
            return method.Invoke(this.ProxyObject, Args);
        }

        public object Invoke(string MethodName, Dictionary<string, object> Args)
        {
            System.Reflection.MethodInfo method = this.ProxyType.GetMethod(MethodName);
            System.Reflection.ParameterInfo[] parameters = method.GetParameters();
            object[] convertedArgs = null;
            if (parameters != null && parameters.Length != 0)
            {
                convertedArgs = new object[parameters.Length];
                for (int count = 0; count < parameters.Length; count++)
                {
                    string name = parameters[count].Name;
                    if (!Args.ContainsKey(name) || Args[name] == null)
                    {
                        convertedArgs[count] = null;
                    }
                    else if (Args[name].GetType() == parameters[count].ParameterType)
                    {
                        convertedArgs[count] = Args[name];
                    }
                    else
                    {
                        try
                        {
                            convertedArgs[count] = DataConvert.Convert(Args[name], parameters[count].ParameterType, true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(
                                string.Format(
                                    "Call web service exception. Parameter type dismatched. " +
                                    "Url={0}, ClassName={1}, MethodName={2}. " +
                                    "The type of parameter \"{3}\" is {4} and value is {5}.",
                                    this.Url,
                                    this.ClassName,
                                    MethodName,
                                    name,
                                    parameters[count].ParameterType,
                                    Args[name]),
                                ex);
                        }
                    }
                }
            }
            return method.Invoke(this.ProxyObject, convertedArgs);
        }
    }
}
