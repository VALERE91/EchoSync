using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EchoSync.Serialization;

namespace EchoSync.RPC
{
    public class RpcMethodParameter
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        
        public RpcMethodParameter(ParameterInfo info)
        {
            Name = info.Name;
            Type = info.ParameterType;
        }
    }
    
    public class RpcMethod
    {
        public string MethodeName { get; private set; }

        public List<RpcMethodParameter> ParametersInfos { get; set; }

        private readonly MethodInfo _info;
        
        public RpcMethod(MethodInfo info, ServerRpcAttribute attribute)
        {
            _info = info;
            MethodeName = info.Name;
            var parameterInfos = info.GetParameters();
            ParametersInfos = parameterInfos.Select(p => new RpcMethodParameter(p)).ToList();
        }
        
        public void Invoke(object target, Span<byte> parameters)
        {
            var parametersList = new List<object>();
            
            var bitStream = new BitStream(parameters);
            var reader = new EchoBitStream();
            
            foreach (var parameterInfo in ParametersInfos)
            {
                object param = reader.Read(parameterInfo.Type, ref bitStream);
                parametersList.Add(param);
            }
            
            _info.Invoke(target, parametersList.ToArray());
        }
        
        public void SerializeParameters(ref BitStream bitStream, object[] parameters)
        {
            var writer = new EchoBitStream();
            
            for (int i = 0; i < parameters.Length; i++)
            {
                writer.Write(ParametersInfos[i].Type, ref bitStream, parameters[i]);
            }
        }
    }
    
    public static class RpcMethodInfoBuilder
    {
        public static List<RpcMethod> CreateRpcMethodInfo<T>()
        {
            var objectType = typeof(T);
            return objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(ServerRpcAttribute), inherit: true))
                .Select(p =>
                {
                    var attribute = p.GetCustomAttribute<ServerRpcAttribute>();
                    return new RpcMethod(p, attribute); 
                })
                .ToList();
        }
    }
}