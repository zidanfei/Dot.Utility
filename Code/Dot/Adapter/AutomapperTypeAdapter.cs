using AutoMapper;
using Dot.IOC;
using System;
using System.Linq;

namespace Dot.Adapter
{
    /// <summary>
    /// Automapper type adapter implementation
    /// </summary>
    [Export(typeof(ITypeAdapter))]
    public class AutomapperTypeAdapter : ITypeAdapter
    {
        public AutomapperTypeAdapter()
        {

        }

        #region ITypeAdapter Members

        /// <summary>
        ///  将源类型转换为目标类型的适配器
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>目标类型</returns>
        public TTarget Adapt<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            return Mapper.Map<TSource, TTarget>(source);
        }

        /// <summary>
        /// 将object对象转换为目标类型对象
        /// </summary>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>目标类型</returns>
        public TTarget Adapt<TTarget>(object source) where TTarget : class, new()
        {
            return Mapper.Map<TTarget>(source);
        }

        public TTarget Adapt<TTarget>(object source, TTarget target) where TTarget : class
        {
            return Mapper.Map(source, target, source.GetType(), typeof(TTarget)) as TTarget;
        }

        public Type GetMapTarget(Type type)
        {
            var tempMap= Mapper.GetAllTypeMaps().Where(pre => pre.SourceType == type).FirstOrDefault();

            return tempMap == null ? null : tempMap.DestinationType;
        }

        public object GetMapTarget(object instance, Type sourceType, Type distinationType)
        {
            return Mapper.Map(instance, sourceType, distinationType);
        }

        #endregion
    }
}
