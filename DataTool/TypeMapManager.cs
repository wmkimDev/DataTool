using System;
using System.Collections.Generic;

namespace DataTool
{
    public class TypeMapManager
    {
        private static TypeMapManager? _instance;
        public static  TypeMapManager  Instance => _instance ??= new TypeMapManager();

        private Dictionary<string, string> _typeMap;

        private TypeMapManager()
        {
            _typeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "int", "int" },
                { "bool", "bool" },
                { "long", "long" },
                { "float", "float" },
                { "double", "double" },
                { "string", "string" },
                { "datetime", "datetime" },
                { "timespan", "timespan" }
            };
        }

        public IReadOnlyDictionary<string, string> TypeMap => _typeMap;

        public void UpdateTypeMap(Dictionary<string, string> newTypeMap)
        {
            _typeMap = new Dictionary<string, string>(newTypeMap, StringComparer.OrdinalIgnoreCase);
        }
    }
}