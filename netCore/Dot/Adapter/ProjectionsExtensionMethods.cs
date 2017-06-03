using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Adapter
{
    public static class ProjectionsExtensionMethods
    {
        /// <summary>
        /// Project a type using a DTO
        /// </summary>
        /// <typeparam name="TProjection">The dto projection</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The projected type
        /// </returns>
        public static TProjection ProjectedAs<TProjection>(this object item)
            where TProjection : class,new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TProjection>(item);
        }

        /// <summary>
        /// projected a enumerable collection of items
        /// </summary>
        /// <typeparam name="TProjection">The dtop projection type</typeparam>
        /// <param name="items">the collection of entity items</param>
        /// <returns>Projected collection</returns>
        public static List<TProjection> ProjectedAsCollection<TProjection>(this IEnumerable<object> items)
            where TProjection : class,new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TProjection>>(items);
        }

        /// <summary>
        /// Adapts to specific target.
        /// </summary>
        /// <typeparam name="TProjection">The type of the projection.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="target">The target.</param>
        public static void AdaptTo<TProjection>(this object item, TProjection target)
            where TProjection : class
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            adapter.Adapt<TProjection>(item, target);
        }

        public static Type GetMapTargetType(this Type type)
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.GetMapTarget(type);
        }

        public static Type GetMapTargetType(this object type)
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.GetMapTarget(type.GetType());
        }

        public static object GetMapTarget(this object instance, Type distinationType)
        {
            if (instance == null)
                throw new ArgumentNullException("待转换的对象不能为空");

            if (distinationType == null)
                throw new ArgumentNullException("转换的目标类型不能为空");

            var adapter = TypeAdapterFactory.CreateAdapter();

            return adapter.GetMapTarget(instance, instance.GetType(), distinationType);
        }

        public static TOut CreateObject<TOut>(this object source)
            where TOut : class ,new()
        {
            if (source == null)
            {
                return default(TOut);
            }

            TOut tempOut= new TOut();

            var tOutPropertys =  tempOut.GetType().GetProperties();
            var tSourcePropertys = source.GetType().GetProperties();

            if (tSourcePropertys.Count() == 0)
                return default(TOut);

            foreach (var tSourceProperty in tSourcePropertys)
            {
                var tempOutProperty =
                    tOutPropertys.FirstOrDefault(
                        pre => pre.Name == tSourceProperty.Name && pre.PropertyType == tSourceProperty.PropertyType);

                if (tempOutProperty == null)
                    continue;

                //通过反射深复制
                tempOutProperty.SetValue(tempOut, tSourceProperty.GetValue(source, null), null);
            }

            return tempOut;
        }

      
    }
}
