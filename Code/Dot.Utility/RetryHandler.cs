using System;
using System.Net;
using System.Net.Sockets;
public delegate void MethodToMaxErrorCountHandler(Object instance, System.Reflection.MethodInfo method, Exception ex);

public delegate Boolean MethodErrorHandler(Object instance, System.Reflection.MethodInfo method, Exception ex, Int32 count);

public delegate void OperationFinishedHandler(Object returnValue);


namespace Dot.Utility
{
    public static class Common
    {
        delegate Object AsynchronousHandler(Object instance, String function, Object[] args, Boolean isPublic, Boolean isStatic);
        static AsynchronousHandler ah;

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public const Int32 MAX_RETRY_TIME = 5;

        /// <summary>
        /// 当函数发生错误并达到最大重试次数时触发
        /// </summary>
        public static event MethodToMaxErrorCountHandler MethodToMaxErrorCount;


        /// <summary>
        /// 当函数发生错误时触发, 当达到最大重试次数时不触发此事件
        /// </summary>
        public static event MethodErrorHandler MethodError;


        /// <summary>
        /// 根据函数名调用函数
        /// </summary>
        /// <param name="instance">调用对象, 如果是静态函数, 可忽略此参数</param>
        /// <param name="function">欲调用的函数名</param>
        /// <param name="args">参数</param>
        /// <param name="isPublic">指示该函数是否为公共函数</param>
        /// <param name="isStatic">指示该函数是否为静态函数</param>
        /// <returns>函数调用返回值, 如果函数没有返回值或者调用出错则返回NULL,</returns>
        public static Object TestInvokeMethod(Object instance, String function, Object[] args, Boolean isPublic, Boolean isStatic)
        {
            Type type = instance.GetType();
            Int32 errorRecord = 0;
            String recName = type.Name + "." + function;

            Object value = AppDomain.CurrentDomain.GetData(recName);

            if (value != null)
                errorRecord = (Int32)value;

            System.Reflection.BindingFlags flag = System.Reflection.BindingFlags.Instance;
            flag |= isPublic ? System.Reflection.BindingFlags.Public : System.Reflection.BindingFlags.NonPublic;
            if (isStatic) flag |= System.Reflection.BindingFlags.Static;

            Type[] types = new Type[args.Length];

            for (Int32 i = 0; i < args.Length; i++)
                types[i] = args[i].GetType();

            System.Reflection.MethodInfo target = type.GetMethod(function, types);

            try
            {
                Object result = target.Invoke(instance, args);

                AppDomain.CurrentDomain.SetData(recName, null);

                return result;
            }
            catch (Exception ex)
            {

                errorRecord++;

                AppDomain.CurrentDomain.SetData(recName, errorRecord);

                if (MethodError != null)
                {
                    if (!MethodError(instance, target, ex, errorRecord))
                    {
                        if (MethodToMaxErrorCount != null)
                            MethodToMaxErrorCount(instance, target, ex);

                        return null;
                    }
                }

                if (errorRecord < MAX_RETRY_TIME)
                {
                    return TestInvokeMethod(instance, function, args, isPublic, isStatic);
                }
                else
                {
                    AppDomain.CurrentDomain.SetData(recName, null);

                    if (MethodToMaxErrorCount != null)
                        MethodToMaxErrorCount(instance, target, ex);

                    return null;
                }
            }
        }

        /// <summary>
        /// 根据函数名异步调用函数
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="function"></param>
        /// <param name="args"></param>
        /// <param name="isPublic"></param>
        /// <param name="isStatic"></param>
        public static void AsynchronousTestInvokeMethod(Object instance, String function, Object[] args, Boolean isPublic, Boolean isStatic, OperationFinishedHandler ofh)
        {

            ah = ah ?? new AsynchronousHandler(TestInvokeMethod);

            ah.BeginInvoke(instance, function, args, isPublic, isStatic, new AsyncCallback(Callback), ofh);
        }

        private static void Callback(IAsyncResult rs)
        {
            if (rs.IsCompleted)
            {
                Object result = ah.EndInvoke(rs);

                OperationFinishedHandler h = rs.AsyncState as OperationFinishedHandler;
                h.Invoke(result);
            }
        }

        /// <summary>
        /// 检查指定的地址及其端口是否可用
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Boolean CheckURI(String uri, Int32 port)
        {

            IPAddress ip = IPAddress.Parse(uri);


            try
            {
                IPEndPoint point = new IPEndPoint(ip, port);
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(point);

                sock.Disconnect(true);
                sock.Close();

                Console.WriteLine("连接{0}成功!", point);

                return true;

            }
            catch (SocketException e)
            {


                switch (e.ErrorCode)
                {
                    case (10013):
                        Console.WriteLine("连接{0}失败, 禁止访问. (10013)");
                        break;
                    case (10014):
                        Console.WriteLine("连接{0}失败, IP地址不正确. (10014)");
                        break;
                    case (10036):
                    case (10037):
                        Console.WriteLine("操作正在进行中, 请勿重复操作. ({0})", e.ErrorCode);
                        break;
                    case (10041):
                        Console.WriteLine("协议不匹配. (10041)");
                        break;
                    case (10042):
                        Console.WriteLine("协议不可用. (10042)");
                        break;
                    case (10043):
                        Console.WriteLine("不支持的协议. (10043)");
                        break;
                    case (10048):
                        Console.WriteLine("给定的URI已经被占用. (10048)");
                        break;
                    case (10050):
                    case (10051):
                        Console.WriteLine("无网络. ({0})", e.ErrorCode);
                        break;
                    case (10061):
                        Console.WriteLine("连接被拒绝. (10061)");
                        break;
                    default:
                        Console.WriteLine("连接{0}失败. (未知原因{0})", e.ErrorCode);
                        break;
                }

                return false;
            }
        }
    }
}