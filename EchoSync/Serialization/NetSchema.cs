using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EchoSync.Serialization
{
    public class NetSchema
    {
        public Type ObjectType { get; }
        public List<NetPropertyInfo> Properties { get; }

        public NetSchema(Type objectType, List<NetPropertyInfo> properties)
        {
            ObjectType = objectType;
            Properties = properties;
        }
    }
    
    public static class NetSchemaBuilder
    {
        public static NetSchema CreateSchema<T>()
        {
            var objectType = typeof(T);
            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(NetPropertyAttribute), inherit: true))
                .Select(p =>
                {
                    var attribute = p.GetCustomAttribute<NetPropertyAttribute>();
                    return new NetPropertyInfo(p);
                })
                .ToList();

            return new NetSchema(objectType, properties);
        }
    }
}