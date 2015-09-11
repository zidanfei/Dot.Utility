using System;
namespace Dot.Adapter
{
    /// <summary>
    /// Base contract for map dto to aggregate or aggregate to dto.
    /// <remarks>
    /// This is a  contract for work with "auto" mappers ( automapper,emitmapper,valueinjecter...)
    /// or adhoc mappers
    /// </remarks>
    /// </summary>
    public interface ITypeAdapter
    {
        /// <summary>
        ///  将源类型转换为目标类型的适配器
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>目标类型</returns>
        TTarget Adapt<TSource, TTarget>(TSource source)
            where TTarget : class,new()
            where TSource : class;

        /// <summary>
        /// 将object对象转换为目标类型对象
        /// </summary>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>目标类型</returns>
        TTarget Adapt<TTarget>(object source) where TTarget : class,new();

        /// <summary>
        /// 将源类型转换为目标类型的适配器
        /// </summary>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// 目标类型
        /// </returns>
        TTarget Adapt<TTarget>(object source, TTarget target) where TTarget : class;

        /// <summary>
        /// 根据映射的源类型，查找映射的目标类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Type GetMapTarget(Type type);

        /// <summary>
        /// 根据源类型映射为目标类型的实例
        /// </summary>
        /// <param name="instance">源实例</param>
        /// <param name="sourceType">源实例类型</param>
        /// <param name="distinationType">目标实例类型</param>
        /// <returns></returns>
        object GetMapTarget(object instance, Type sourceType, Type distinationType);
    }
}
