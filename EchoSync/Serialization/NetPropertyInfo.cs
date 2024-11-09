using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EchoSync.Serialization
{
    public class NetPropertyInfo
    {
        public PropertyInfo Property { get; private set; }

        public Func<object, object> Getter { get; private set; }

        public Action<object, object> Setter { get; private set; }
        
        public NetPropertyInfo(PropertyInfo property)
        {
            Property = property;
            Getter = CompileGetter(property);
            Setter = CompileSetter(property);
        }

        private static Func<object, object> CompileGetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var castInstance = Expression.Convert(instance, property.DeclaringType);
            var propertyAccess = Expression.Property(castInstance, property);
            var castResult = Expression.Convert(propertyAccess, typeof(object));

            return Expression.Lambda<Func<object, object>>(castResult, instance).Compile();
        }

        private static Action<object, object> CompileSetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var castInstance = Expression.Convert(instance, property.DeclaringType);
            var castValue = Expression.Convert(value, property.PropertyType);
            var propertyAssign = Expression.Assign(Expression.Property(castInstance, property), castValue);

            return Expression.Lambda<Action<object, object>>(propertyAssign, instance, value).Compile();
        }
    }
}