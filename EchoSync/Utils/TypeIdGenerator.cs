using System;
using System.Collections.Generic;

namespace EchoSync.Utils
{
    public class TypeIdGenerator
    {
        private static readonly Dictionary<Type, int> _typeIDMap = new Dictionary<Type, int>();
        private static int _nextTypeId = 1;
        
        public static int GetOrRegisterTypeId(Type type)
        {
            if (_typeIDMap.TryGetValue(type, out var id))
            {
                return id;
            }

            id = _nextTypeId++;
            _typeIDMap[type] = id;
            return id;
        }
    }
}