﻿using System;
using System.Collections.Generic;
using System.Linq;
using TinYard.API.Interfaces;
using TinYard.Framework.API.Interfaces;
using TinYard.Framework.Impl.Factories;
using TinYard.Impl.VO;

namespace TinYard.Impl.Mappers
{
    public class ValueMapper : IMapper
    {
        public event Action<IMappingObject> OnValueMapped;

        protected List<IMappingObject> _mappingObjects = new List<IMappingObject>();

        public IMappingFactory MappingFactory { get { return _mappingFactory; } }
        protected IMappingFactory _mappingFactory;

        public ValueMapper()
        {
            _mappingFactory = new MappingValueFactory(this);
        }

        public IMappingObject Map<T>()
        {
            var mappingObj = new MappingObject(this).Map<T>();

            if (OnValueMapped != null)
                mappingObj.OnValueMapped += ( mapping ) => OnValueMapped.Invoke(mapping);

            _mappingObjects.Add(mappingObj);
            return mappingObj;
        }

        public IMappingObject GetMapping<T>()
        {
            Type type = typeof(T);

            return GetMapping(type);
        }

        public IMappingObject GetMapping(Type type)
        {
            var value = _mappingObjects.FirstOrDefault(mapping => mapping.MappedType.IsAssignableFrom(type));

            return value;
        }

        public IReadOnlyList<IMappingObject> GetAllMappings()
        {
            return _mappingObjects.AsReadOnly();
        }

        public T GetMappingValue<T>()
        {
            var mappedValue = GetMapping<T>()?.MappedValue;
            return mappedValue is T ? (T)mappedValue : default(T);
        }

        public object GetMappingValue(Type type)
        {
            return GetMapping(type)?.MappedValue;
        }
    }
}
