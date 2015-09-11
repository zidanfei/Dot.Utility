using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Compile
{
    public partial class ReflectHelper
    {
        /// <summary>
        /// 根据对象实例获取对象内部的属性信息
        /// </summary>
        /// <param name="unityProxyObject">对象实例</param>
        /// <param name="propertyName">对象属性名称-支持私有变量</param>
        /// <returns></returns>
        public static object GetInstanceProperty(object unityProxyObject, string propertyName)
        {
            MemberInfo[] minss = unityProxyObject.GetType().GetMembers(BindingFlags.CreateInstance |
                                                   BindingFlags.Static |
                                                  BindingFlags.NonPublic | BindingFlags.GetField
                                                  | BindingFlags.GetProperty | BindingFlags.Instance
                                                  | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.FlattenHierarchy);

            //tempService.GetType().InvokeMember("get_ClassName", BindingFlags.InvokeMethod, null, Tb, null, null, null, null).ToString();
            if (minss != null && minss.Length > 0)
            {
                var tempMemberInfo = minss.Where(pre => pre.Name == propertyName).FirstOrDefault();
                var tempFieldInfo = tempMemberInfo as FieldInfo;
                var tempValue = tempFieldInfo.GetValue(unityProxyObject);
                return tempValue;
            }

            return null;
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object DeepClone(object instance)
        {
            return instance;
        }

        #region Lamda表达式-反射支持--高性能

        public Func<object, object> LmdGet(Type entityType, string propName)
        {
            #region 通过方法取值
            var p = entityType.GetProperty(propName);
            //对象实例
            var param_obj = Expression.Parameter(typeof(object), "obj");
            //值
            //var param_val = Expression.Parameter(typeof(object), "val");
            //转换参数为真实类型
            var body_obj = Expression.Convert(param_obj, entityType);

            //调用获取属性的方法
            var body = Expression.Call(body_obj, p.GetGetMethod());
            return Expression.Lambda<Func<object, object>>(body, param_obj).Compile();
            #endregion

            #region 表达式取值
            //var p = entityType.GetProperty(propName);
            ////lambda的参数u
            //var param_u = Expression.Parameter(entityType, "u");
            ////lambda的方法体 u.Age
            //var pGetter = Expression.Property(param_u, p);
            ////编译lambda
            //LmdGetProp = Expression.Lambda<Func<TestData, int>>(pGetter, param_u).Compile();
            #endregion
        }

        public Action<object, object> LmdSet(Type entityType, string propName)
        {
            var p = entityType.GetProperty(propName);
            //对象实例
            var param_obj = Expression.Parameter(typeof(object), "obj");
            //值
            var param_val = Expression.Parameter(typeof(object), "val");
            //转换参数为真实类型
            var body_obj = Expression.Convert(param_obj, entityType);
            var body_val = Expression.Convert(param_val, p.PropertyType);
            //调用给属性赋值的方法
            var body = Expression.Call(body_obj, p.GetSetMethod(), body_val);
            return Expression.Lambda<Action<object, object>>(body, param_obj, param_val).Compile();
        }

        #endregion

        #region Emit表达式-反射支持--高性能

        public delegate void SetValueDelegateHandler(object entity, object value);
        public SetValueDelegateHandler EmitSetValue;
        public void SetPropertyValueEmit(Type entityType, string propertyName)
        {
            //Type entityType = entity.GetType();
            Type parmType = typeof(object);
            // 指定函数名
            string methodName = "set_" + propertyName;
            // 搜索函数，不区分大小写 IgnoreCase
            var callMethod = entityType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic);
            // 获取参数
            var para = callMethod.GetParameters()[0];
            // 创建动态函数
            DynamicMethod method = new DynamicMethod("EmitCallable", null, new Type[] { entityType, parmType }, entityType.Module);
            // 获取动态函数的 IL 生成器
            var il = method.GetILGenerator();
            // 创建一个本地变量，主要用于 Object Type to Propety Type
            var local = il.DeclareLocal(para.ParameterType, true);
            // 加载第 2 个参数【(T owner, object value)】的 value
            il.Emit(OpCodes.Ldarg_1);
            if (para.ParameterType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, para.ParameterType);// 如果是值类型，拆箱 string = (string)object;
            }
            else
            {
                il.Emit(OpCodes.Castclass, para.ParameterType);// 如果是引用类型，转换 Class = object as Class
            }
            il.Emit(OpCodes.Stloc, local);// 将上面的拆箱或转换，赋值到本地变量，现在这个本地变量是一个与目标函数相同数据类型的字段了。
            il.Emit(OpCodes.Ldarg_0); // 加载第一个参数 owner
            il.Emit(OpCodes.Ldloc, local);// 加载本地参数
            il.EmitCall(OpCodes.Callvirt, callMethod, null);//调用函数
            il.Emit(OpCodes.Ret); // 返回
            /* 生成的动态函数类似：
            * void EmitCallable(T owner, object value)
            * {
            * T local = (T)value;
            * owner.Method(local);
            * }
            */

            EmitSetValue = method.CreateDelegate(typeof(SetValueDelegateHandler)) as SetValueDelegateHandler;
        }

        public static object GetPropertyValueEmit(object entity, PropertyInfo property)
        {
            DynamicMethod method = new DynamicMethod("GetValue", property.PropertyType, new Type[] { entity.GetType() });

            ILGenerator ilGenerator = method.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);
            ilGenerator.Emit(OpCodes.Ret);

            method.DefineParameter(1, ParameterAttributes.In, "target");
            var getFunc = (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
            return getFunc(entity);
        }

        #endregion

        #region 复制对象

        /// <summary>
        /// 深复制对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="RealObject">待复制的对象</param>
        /// <returns></returns>
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }



        #endregion
   

        /// <summary>
        /// Get the internal template content from the commonlibrary assembly.
        /// </summary>
        /// <param name="assemblyFolderPath">"CommonLibrary.Notifications.Templates."</param>
        /// <param name="fileName">"welcome.html"</param>
        /// <returns>String with internal template content.</returns>
        public static string GetInternalFileContent(string assemblyFolderPath, string fileName)
        {
            Assembly current = Assembly.GetExecutingAssembly();

            Stream stream = current.GetManifestResourceStream(assemblyFolderPath + fileName);
            if (stream == null)
            {
                return string.Empty;
            }
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            return content;
        }

        /// <summary>
        /// Set object properties on T using the properties collection supplied.
        /// The properties collection is the collection of "property" to value.
        /// </summary>
        /// <typeparam name="T">A class type.</typeparam>
        /// <param name="obj">Object whose properties will be set.</param>
        /// <param name="properties">List of key/value pairs with property names and values.</param>
        public static void SetProperties<T>(T obj, IList<KeyValuePair<string, string>> properties) where T : class
        {
            // Validate
            if (obj == null)
            {
                return;
            }

            foreach (KeyValuePair<string, string> propVal in properties)
            {
                SetProperty<T>(obj, propVal.Key, propVal.Value);
            }
        }

        /// <summary>
        /// Set the object properties using the prop name and value.
        /// </summary>
        /// <typeparam name="T">A class type.</typeparam>
        /// <param name="obj">Object whose property will be set.</param>
        /// <param name="propName">Property name to set.</param>
        /// <param name="propVal">Property value to set.</param>
        public static void SetProperty<T>(object obj, string propName, object propVal) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("Object containing properties to set is null");

            if (obj.GetType().GetProperties().Where(pre => pre.Name.Equals(propName)).Count() == 0)
                throw new ArgumentNullException("Property name not supplied");

            // Remove spaces.
            propName = propName.Trim();
            if (string.IsNullOrEmpty(propName))
            {
                throw new ArgumentException("Property name is empty.");
            }

            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propName);

            // Correct property with write access 
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                // Check same Type
                if (CanConvertToCorrectType(propertyInfo, propVal))
                {
                    object convertedVal = ConvertToSameType(propertyInfo, propVal);
                    propertyInfo.SetValue(obj, convertedVal, null);
                }
            }
        }

        /// <summary>
        /// Set the object properties using the prop name and value.
        /// </summary>
        /// <param name="obj">Object whose property will be set.</param>
        /// <param name="propName">Property name to set.</param>
        /// <param name="propVal">Property value to set.</param>
        public static void SetProperty(object obj, string propName, object propVal)
        {
            // Remove spaces.
            propName = propName.Trim();
            if (string.IsNullOrEmpty(propName))
            {
                throw new ArgumentException("Property name is empty.");
            }

            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propName);

            // Correct property with write access 
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(obj, propVal, null);
            }
        }

        /// <summary>
        /// Set the object properties using the prop name and value.
        /// </summary>
        /// <param name="obj">Object whose property will be set.</param>
        /// <param name="prop">Property information.</param>
        /// <param name="propVal">Property value to set.</param>
        /// <param name="catchException">Try to catch any exception and
        /// not throw it to the caller.</param>
        public static void SetProperty(object obj, PropertyInfo prop, object propVal, bool catchException)
        {
            // Correct property with write access 
            if (prop != null && prop.CanWrite)
            {
                if (!catchException)
                    prop.SetValue(obj, propVal, null);
                else
                {
                    try
                    {
                        prop.SetValue(obj, propVal, null);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Set the property value using the string value.
        /// </summary>
        /// <param name="obj">Object whose property will be set.</param>
        /// <param name="prop">Property information.</param>
        /// <param name="propVal">Property value to set.</param>
        public static void SetProperty(object obj, PropertyInfo prop, string propVal)
        {
            if (obj == null)
                throw new ArgumentNullException("对象不能为空");

            if (obj.GetType().GetProperties().Where(pre => pre.Name.Equals(prop.Name)).Count() == 0)
                throw new ArgumentNullException("当前类型的对象不包含属性：" + prop.Name);

            // Correct property with write access 
            if (prop != null && prop.CanWrite)
            {
                // Check same Type
                if (CanConvertToCorrectType(prop, propVal))
                {
                    object convertedVal = ConvertToSameType(prop, propVal);
                    prop.SetValue(obj, convertedVal, null);
                }
            }
        }

        /// <summary>
        /// Get the property value.
        /// </summary>
        /// <param name="obj">Object whose property will be retrieved.</param>
        /// <param name="propName">Name of property to retrieve.</param>
        /// <returns>Property value.</returns>
        public static object GetPropertyValue(object obj, string propName)
        {
            if (obj == null)
                throw new ArgumentNullException("对象不能为空");

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("属性名称不能为空");

            if (obj.GetType().GetProperties().Where(pre => pre.Name.Equals(propName)).Count() == 0)
                throw new ArgumentNullException("当前类型的对象不包含属性：" + propName);

            propName = propName.Trim();

            PropertyInfo property = obj.GetType().GetProperty(propName);
            if (property == null)
                return null;

            return property.GetValue(obj, null);
        }

        /// <summary>
        /// Get all the property values.
        /// </summary>
        /// <param name="obj">Object whose properties will be retrieved.</param>
        /// <param name="properties">List of properties to retrieve.</param>
        /// <returns>List with property values.</returns>
        public static IList<object> GetPropertyValues(object obj, IList<string> properties)
        {
            IList<object> propertyValues = new List<object>();

            foreach (string property in properties)
            {
                PropertyInfo propInfo = obj.GetType().GetProperty(property);
                object val = propInfo.GetValue(obj, null);
                propertyValues.Add(val);
            }
            return propertyValues;
        }

        /// <summary>
        /// Get all the properties.
        /// </summary>
        /// <param name="obj">Object whose properties will be retrieved.</param>
        /// <param name="propsDelimited">Delimited list with properties to retrieve.</param>
        /// <returns>List of property values.</returns>
        public static IList<PropertyInfo> GetProperties(object obj, string propsDelimited)
        {
            return GetProperties(obj.GetType(), propsDelimited.Split(','));
        }

        /// <summary>
        /// Get property information for a type.
        /// </summary>
        /// <param name="type">Type whose property names to retrieve.</param>
        /// <param name="props">Array with property names to look for.</param>
        /// <returns>List with property information of found properties.</returns>
        public static IList<PropertyInfo> GetProperties(Type type, string[] props)
        {
            PropertyInfo[] allProps = type.GetProperties();
            List<PropertyInfo> propToGet = new List<PropertyInfo>();
            IList<string> propsMap = props.ToList<string>();
            foreach (PropertyInfo prop in allProps)
            {
                if (propsMap.Contains(prop.Name))
                    propToGet.Add(prop);
            }
            return propToGet;
        }

        /// <summary>
        /// Get all the properties.
        /// </summary>
        /// <param name="type">Type whose property names to retrieve.</param>
        /// <param name="props">Array with property names to look for.</param>
        /// <param name="flags">Flags to use when searching for properties.</param>
        /// <returns>List with property information of found properties.</returns>
        public static IList<PropertyInfo> GetProperties(Type type, string[] props, BindingFlags flags)
        {
            PropertyInfo[] allProps = type.GetProperties(flags);
            List<PropertyInfo> propToGet = new List<PropertyInfo>();
            IList<string> propsMap = props.ToList<string>();
            foreach (PropertyInfo prop in allProps)
            {
                if (propsMap.Contains(prop.Name))
                    propToGet.Add(prop);
            }
            return propToGet;
        }

        /// <summary>
        /// Gets the property value safely, without throwing an exception.
        /// If an exception is caught, null is returned.
        /// </summary>
        /// <param name="obj">Object to look into.</param>
        /// <param name="propInfo">Information of property to retrieve.</param>
        /// <returns>Retrieved property value.</returns>
        public static object GetPropertyValueSafely(object obj, PropertyInfo propInfo)
        {
            if (obj == null)
                throw new ArgumentNullException("对象不能为空");

            if (propInfo == null)
                return null;

            object result = null;
            try
            {
                result = propInfo.GetValue(obj, null);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Gets all the properties of an object.
        /// </summary>
        /// <param name="obj">Object to look into.</param>
        /// <param name="criteria">Matching criteria.</param>
        /// <returns>List with information of matched properties.</returns>
        public static IList<PropertyInfo> GetAllProperties(object obj, Predicate<PropertyInfo> criteria)
        {
            if (obj == null)
            {
                return null;
            }
            return GetProperties(obj.GetType(), criteria);
        }

        /// <summary>
        /// Get the properties of a type.
        /// </summary>
        /// <param name="type">Type to look into.</param>
        /// <param name="criteria">Matching criteria.</param>
        /// <returns>List of information of matched properties.</returns>
        public static IList<PropertyInfo> GetProperties(Type type, Predicate<PropertyInfo> criteria)
        {
            IList<PropertyInfo> allProperties = new List<PropertyInfo>();
            PropertyInfo[] properties = type.GetProperties();
            if (properties == null || properties.Length == 0)
            {
                return null;
            }

            // Now check for all writeable properties.
            foreach (PropertyInfo property in properties)
            {
                // Only include writable properties and ones that are not in the exclude list.
                bool okToAdd = (criteria == null) ? true : criteria(property);
                if (okToAdd)
                {
                    allProperties.Add(property);
                }
            }
            return allProperties;
        }

        /// <summary>
        /// Gets all the writable properties of an object.
        /// </summary>
        /// <param name="obj">Object to look into.</param>
        /// <param name="propsToExclude">Dictionary with properties to exclude.</param>
        /// <returns>List with information of matched properties.</returns>
        public static IList<PropertyInfo> GetWritableProperties(object obj, StringDictionary propsToExclude)
        {
            IList<PropertyInfo> props = GetWritableProperties(obj.GetType(),
                 delegate(PropertyInfo property)
                 {
                     bool okToAdd = propsToExclude == null ? property.CanWrite : (property.CanWrite && !propsToExclude.ContainsKey(property.Name));
                     return okToAdd;
                 });
            return props;
        }

        /// <summary>
        /// Gets all the properties of a type.
        /// </summary>
        /// <param name="propsToExclude">Dictionary with properties to exclude.</param>
        /// <param name="typ">Type to look into.</param>
        /// <returns>List with information of matched properties.</returns>
        public static IList<PropertyInfo> GetProperties(StringDictionary propsToExclude, Type typ)
        {
            IList<PropertyInfo> props = GetWritableProperties(typ,
                 delegate(PropertyInfo property)
                 {
                     bool okToAdd = propsToExclude == null ? true : (!propsToExclude.ContainsKey(property.Name));
                     return okToAdd;
                 });
            return props;
        }

        /// <summary>
        /// Gets all the properties of the object as dictionary of property names to propertyInfo.
        /// </summary>
        /// <param name="obj">Object to look into.</param>
        /// <param name="criteria">Matching criteria.</param>
        /// <returns>Dictionary with property name and information of matched properties.</returns>
        public static IDictionary<string, PropertyInfo> GetPropertiesAsMap(object obj, Predicate<PropertyInfo> criteria)
        {
            IList<PropertyInfo> matchedProps = GetProperties(obj.GetType(), criteria);
            IDictionary<string, PropertyInfo> props = new Dictionary<string, PropertyInfo>();

            // Now check for all writeable properties.
            foreach (PropertyInfo prop in matchedProps)
            {
                props.Add(prop.Name, prop);
            }
            return props;
        }

        /// <summary>
        /// Get all the properties.
        /// </summary>
        /// <param name="type">Type to look into.</param>
        /// <param name="flags">Flags to use when looking for properties.</param>
        /// <param name="isCaseSensitive">True to use the property name in the
        /// dictionary with its lower-cased value.</param>
        /// <returns>Dictionary with property name and information of found properties.</returns>
        public static IDictionary<string, PropertyInfo> GetPropertiesAsMap(Type type, BindingFlags flags, bool isCaseSensitive)
        {
            PropertyInfo[] allProps = type.GetProperties(flags);
            IDictionary<string, PropertyInfo> propsMap = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo prop in allProps)
            {
                if (isCaseSensitive)
                    propsMap[prop.Name] = prop;
                else
                    propsMap[prop.Name.Trim().ToLower()] = prop;
            }

            return propsMap;
        }

        /// <summary>
        /// Get all the properties.
        /// </summary>
        /// <typeparam name="T">Type to look into.</typeparam>
        /// <param name="flags">Flags to use when looking for properties.</param>
        /// <param name="isCaseSensitive">True to use the property name in the
        /// dictionary with its lower-cased value.</param>
        /// <returns>Dictionary with property name and information of found properties.</returns>
        public static IDictionary<string, PropertyInfo> GetPropertiesAsMap<T>(BindingFlags flags, bool isCaseSensitive)
        {
            Type type = typeof(T);
            return GetPropertiesAsMap(type, flags, isCaseSensitive);
        }

        /// <summary>
        /// Get the propertyInfo of the specified property name.
        /// </summary>
        /// <param name="type">Type to look into.</param>
        /// <param name="propertyName">Name of property.</param>
        /// <returns>Information of property.</returns>
        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            IList<PropertyInfo> props = GetProperties(type,
                delegate(PropertyInfo property)
                {
                    return property.Name == propertyName;
                });
            return props[0];
        }

        /// <summary>
        /// Gets a list of all the writable properties of the class associated with the object.
        /// </summary>
        /// <param name="type">Type to look into.</param>
        /// <param name="criteria">Matching criteria.</param>
        /// <remarks>This method does not take into account, security, generics, etc.
        /// It only checks whether or not the property can be written to.</remarks>
        /// <returns>List with information of matching properties.</returns>
        public static IList<PropertyInfo> GetWritableProperties(Type type, Predicate<PropertyInfo> criteria)
        {
            IList<PropertyInfo> props = GetProperties(type,
                 delegate(PropertyInfo property)
                 {
                     // Now determine if it can be added based on criteria.
                     bool okToAdd = (criteria == null) ? property.CanWrite : (property.CanWrite && criteria(property));
                     return okToAdd;
                 });
            return props;
        }

        /// <summary>
        /// Invokes the method on the object provided.
        /// </summary>
        /// <param name="obj">The object containing the method to invoke</param>
        /// <param name="methodName">arguments to the method.</param>
        /// <param name="parameters">Parameters to pass when invoking the method.</param>
        public static object InvokeMethod(object obj, string methodName, object[] parameters)
        {
            if (obj == null)
                throw new ArgumentNullException("对象不能为空");

            if (obj.GetType().GetMethods().Where(pre => pre.Name.Equals(methodName)).Count() == 0)
                throw new ArgumentNullException("当前类型的对象不包含方法：" + methodName);

            methodName = methodName.Trim();

            // Validate.
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Method name not provided.");
            }

            MethodInfo method = obj.GetType().GetMethod(methodName);
            object output = method.Invoke(obj, parameters);
            return output;
        }

        /// <summary>
        /// Copies the property value from the source to destination.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="destination">Destination object.</param>
        /// <param name="prop">Information of property whose value
        /// is to be copied from the source to the target object.</param>
        public static void CopyPropertyValue(object source, object destination, PropertyInfo prop)
        {
            object val = prop.GetValue(source, null);
            prop.SetValue(destination, val, null);
        }

        /// <summary>
        /// Get the description attribute from the assembly associated with <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type who's assembly's description should be obtained.</param>
        /// <param name="defaultVal">Default value to use if description is not available.</param>
        /// <returns>String with assembly information description.</returns>
        public static string GetAssemblyInfoDescription(Type type, string defaultVal)
        {
            // Get the assembly object.
            Assembly assembly = type.Assembly;

            // See if the Assembly Description is defined.
            bool isDefined = Attribute.IsDefined(assembly, typeof(AssemblyDescriptionAttribute));
            string description = defaultVal;

            if (isDefined)
            {
                AssemblyDescriptionAttribute adAttr = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly,
                    typeof(AssemblyDescriptionAttribute));

                if (adAttr != null)
                    description = adAttr.Description;
            }
            return description;

        }

        /// <summary>
        /// Gets the attributes of the specified type applied to the class.
        /// </summary>
        /// <typeparam name="T">Type of attributes to look for.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>List with class custom attributes.</returns>
        public static IList<T> GetClassAttributes<T>(object obj)
        {
            // Check
            if (obj == null)
                return new List<T>();

            object[] attributes = obj.GetType().GetCustomAttributes(typeof(T), false);

            IList<T> attributeList = new List<T>();

            // iterate through the attributes, retrieving the 
            // properties
            foreach (Object attribute in attributes)
            {
                attributeList.Add((T)attribute);
            }
            return attributeList;
        }

        /// <summary>
        /// Loads widgets from the assembly name supplied.
        /// </summary>
        /// <typeparam name="T">Type of attributes to look for.</typeparam>
        /// <param name="assemblyName">The name of the assembly to load widgets from.</param>
        /// <param name="action">A callback for the datatype and widgetattribute.</param>
        /// <returns>List with key/value pair with attributes.</returns>
        public static IList<KeyValuePair<Type, T>> GetClassAttributesFromAssembly<T>(string assemblyName, Action<KeyValuePair<Type, T>> action)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            var types = assembly.GetTypes();
            var components = new List<KeyValuePair<Type, T>>();
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(typeof(T), false);
                if (attributes != null && attributes.Length > 0)
                {
                    var pair = new KeyValuePair<Type, T>(type, (T)attributes[0]);
                    components.Add(pair);
                    if (action != null)
                        action(pair);
                }
            }
            return components;
        }

        /// <summary>
        /// Get a list of property info's that have the supplied attribute applied to it.
        /// </summary>
        /// <typeparam name="T">Type of attributes to look for.</typeparam>
        /// <param name="obj">Object to look into.</param>
        /// <returns>Pair of key/value items with properties.</returns>
        public static IDictionary<string, KeyValuePair<T, PropertyInfo>> GetPropsWithAttributes<T>(object obj) where T : Attribute
        {
            // Check
            if (obj == null)
                return new Dictionary<string, KeyValuePair<T, PropertyInfo>>();
            Dictionary<string, KeyValuePair<T, PropertyInfo>> map = new Dictionary<string, KeyValuePair<T, PropertyInfo>>();

            //IList<PropertyInfo> props = ReflectionUtils.GetAllProperties(obj, null);
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(T), true);
                if (attrs != null && attrs.Length > 0)
                    map[prop.Name] = new KeyValuePair<T, PropertyInfo>(attrs[0] as T, prop);
            }
            return map;
        }

        /// <summary>
        /// Get a list of property info's that have the supplied attribute applied to it.
        /// </summary>
        /// <typeparam name="T">Type of attributes to look for.</typeparam>
        /// <param name="obj">Object to look into.</param>
        /// <returns>List of properties found.</returns>
        public static List<PropertyInfo> GetPropsOnlyWithAttributes<T>(object obj) where T : Attribute
        {
            // Check
            if (obj == null)
                return new List<PropertyInfo>();
            List<PropertyInfo> matchedProps = new List<PropertyInfo>();

            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(T), true);
                if (attrs != null && attrs.Length > 0)
                    matchedProps.Add(prop);
            }
            return matchedProps;
        }

        /// <summary>
        /// Get a list of property info's that have the supplied attribute applied to it.
        /// </summary>
        /// <typeparam name="T">Type of attribute to look for.</typeparam>
        /// <param name="obj">Object to look into.</param>
        /// <returns>List with key/value pairs with property info.</returns>
        public static List<KeyValuePair<T, PropertyInfo>> GetPropsWithAttributesList<T>(object obj) where T : Attribute
        {
            // Check
            if (obj == null)
                return new List<KeyValuePair<T, PropertyInfo>>();
            List<KeyValuePair<T, PropertyInfo>> map = new List<KeyValuePair<T, PropertyInfo>>();

            IList<PropertyInfo> props = GetAllProperties(obj, null);
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(T), true);
                if (attrs != null && attrs.Length > 0)
                    map.Add(new KeyValuePair<T, PropertyInfo>(attrs[0] as T, prop));
            }
            return map;
        }

        /// <summary>
        /// Gets all the properties associated with the supplied types that have the attribute applied to them.
        /// </summary>
        /// <typeparam name="TPropAttrib">The type of the attribute that properties should have</typeparam>
        /// <param name="types">The list of types to search properties for.</param>
        /// <param name="action">Callback</param>
        /// <returns>List with key/value pairs with property info.</returns>
        public static IList<KeyValuePair<PropertyInfo, TPropAttrib>> GetPropertiesWithAttributesOnTypes<TPropAttrib>(IList<Type> types, Action<Type, KeyValuePair<PropertyInfo, TPropAttrib>> action) where TPropAttrib : Attribute
        {
            var propertyAttributes = new List<KeyValuePair<PropertyInfo, TPropAttrib>>();
            foreach (var type in types)
            {
                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    var attributes = prop.GetCustomAttributes(typeof(TPropAttrib), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        var pair = new KeyValuePair<PropertyInfo, TPropAttrib>(prop, attributes[0] as TPropAttrib);
                        propertyAttributes.Add(pair);
                        action(type, pair);
                    }
                }
            }
            return propertyAttributes;
        }

        /// <summary>
        /// Checks whether or not the supplied text can be converted
        /// to a specific type.
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="val">The value to test for conversion to the type
        /// associated with the property</param>
        /// <returns>True if </returns>
        public static bool CanConvertTo<T>(string val)
        {
            return CanConvertTo(typeof(T), val);
        }

        /// <summary>
        /// Checks whether or not the supplied text can be converted
        /// to a specific type.
        /// </summary>
        /// <param name="type">The type to convert val to</param>
        /// <param name="val">The value to test for conversion to the type
        /// associated with the property</param>
        /// <returns>True if the conversion can be performed.</returns>
        public static bool CanConvertTo(Type type, string val)
        {
            // Data could be passed as string value.
            // Try to change type to check type safety.                    
            try
            {
                if (type == typeof(int))
                {
                    int result = 0;
                    if (int.TryParse(val, out result))
                        return true;

                    return false;
                }
                else if (type == typeof(string))
                {
                    return true;
                }
                else if (type == typeof(double))
                {
                    double d = 0;
                    if (double.TryParse(val, out d))
                        return true;

                    return false;
                }
                else if (type == typeof(long))
                {
                    long l = 0;
                    if (long.TryParse(val, out l))
                        return true;

                    return false;
                }
                else if (type == typeof(float))
                {
                    float f = 0;
                    if (float.TryParse(val, out f))
                        return true;

                    return false;
                }
                else if (type == typeof(bool))
                {
                    bool b = false;
                    if (bool.TryParse(val, out b))
                        return true;

                    return false;
                }
                else if (type == typeof(DateTime))
                {
                    DateTime d = DateTime.MinValue;
                    if (DateTime.TryParse(val, out d))
                        return true;

                    return false;
                }
                else if (type.BaseType == typeof(Enum))
                {
                    Enum.Parse(type, val, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            //Conversion worked.
            return true;
        }

        /// <summary>
        /// Check to see if can convert to appropriate type.
        /// </summary>
        /// <param name="propInfo">Property to check.</param>
        /// <param name="val">Instance of object with property.</param>
        /// <returns>True if the conversion can be performed.</returns>
        public static bool CanConvertToCorrectType(PropertyInfo propInfo, object val)
        {
            // Data could be passed as string value.
            // Try to change type to check type safety.                    
            try
            {
                if (propInfo.PropertyType == typeof(int))
                {
                    int i = Convert.ToInt32(val);
                }
                else if (propInfo.PropertyType == typeof(double))
                {
                    double d = Convert.ToDouble(val);
                }
                else if (propInfo.PropertyType == typeof(long))
                {
                    double l = Convert.ToInt64(val);
                }
                else if (propInfo.PropertyType == typeof(float))
                {
                    double f = Convert.ToSingle(val);
                }
                else if (propInfo.PropertyType == typeof(bool))
                {
                    bool b = Convert.ToBoolean(val);
                }
                else if (propInfo.PropertyType == typeof(DateTime))
                {
                    DateTime d = Convert.ToDateTime(val);
                }
                else if (propInfo.PropertyType.BaseType == typeof(Enum) && val is string)
                {
                    Enum.Parse(propInfo.PropertyType, (string)val, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            //Conversion worked.
            return true;
        }

        /// <summary>
        /// Checks whether or not the supplied string can be converted
        /// to the type designated by the supplied property.
        /// </summary>
        /// <param name="propInfo">The property representing the type to convert 
        /// val to</param>
        /// <param name="val">The value to test for conversion to the type
        /// associated with the property</param>
        /// <returns>True if the conversion can be performed.</returns>
        public static bool CanConvertToCorrectType(PropertyInfo propInfo, string val)
        {
            // Data could be passed as string value.
            // Try to change type to check type safety.                    
            try
            {
                if (propInfo.PropertyType == typeof(int))
                {
                    int result = 0;
                    if (int.TryParse(val, out result))
                        return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof(string))
                {
                    return true;
                }
                else if (propInfo.PropertyType == typeof(double))
                {
                    double d = 0;
                    if (double.TryParse(val, out d))
                        return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof(long))
                {
                    long l = 0;
                    if (long.TryParse(val, out l))
                        return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof(float))
                {
                    float f = 0;
                    if (float.TryParse(val, out f))
                        return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof(bool))
                {
                    bool b = false;
                    if (bool.TryParse(val, out b))
                        return true;

                    return false;
                }
                else if (propInfo.PropertyType == typeof(DateTime))
                {
                    DateTime d = DateTime.MinValue;
                    if (DateTime.TryParse(val, out d))
                        return true;

                    return false;
                }
                else if (propInfo.PropertyType.BaseType == typeof(Enum))
                {
                    Enum.Parse(propInfo.PropertyType, val, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            //Conversion worked.
            return true;
        }

        /// <summary>
        /// Convert the val from string type to the same time as the property.
        /// </summary>
        /// <param name="propInfo">Property representing the type to convert to</param>
        /// <param name="val">val to convert</param>
        /// <returns>converted value with the same time as the property</returns>
        public static object ConvertToSameType(PropertyInfo propInfo, object val)
        {
            object convertedType = null;

            if (propInfo.PropertyType == typeof(int))
            {
                convertedType = Convert.ChangeType(val, typeof(int));
            }
            else if (propInfo.PropertyType == typeof(double))
            {
                convertedType = Convert.ChangeType(val, typeof(double));
            }
            else if (propInfo.PropertyType == typeof(long))
            {
                convertedType = Convert.ChangeType(val, typeof(long));
            }
            else if (propInfo.PropertyType == typeof(float))
            {
                convertedType = Convert.ChangeType(val, typeof(float));
            }
            else if (propInfo.PropertyType == typeof(bool))
            {
                convertedType = Convert.ChangeType(val, typeof(bool));
            }
            else if (propInfo.PropertyType == typeof(DateTime))
            {
                convertedType = Convert.ChangeType(val, typeof(DateTime));
            }
            else if (propInfo.PropertyType == typeof(string))
            {
                convertedType = Convert.ChangeType(val, typeof(string));
            }
            else if (propInfo.PropertyType.BaseType == typeof(Enum) && val is string)
            {
                convertedType = Enum.Parse(propInfo.PropertyType, (string)val, true);
            }
            return convertedType;
        }

        /// <summary>
        /// Determine if the type of the property and the val are the same
        /// </summary>
        /// <param name="propInfo">Property whose type is to be compared.</param>
        /// <param name="val">Object whose type is to be compared.</param>
        /// <returns>True if the property and the object are of the same type.</returns>
        public static bool IsSameType(PropertyInfo propInfo, object val)
        {
            // Quick Validation.
            if (propInfo.PropertyType == typeof(int) && val is int)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof(bool) && val is bool)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof(string) && val is string)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof(double) && val is double)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof(long) && val is long)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof(float) && val is float)
            {
                return true;
            }
            if (propInfo.PropertyType == typeof(DateTime) && val is DateTime)
            {
                return true;
            }
            if (propInfo.PropertyType is object && propInfo.PropertyType.GetType() == val.GetType())
            {
                return true;
            }

            return false;
        }
    }
}
