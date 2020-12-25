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

        public object Environment { get { return _environment; } set { _environment = value; }}
        private object _environment;

        public ValueMapper()
        {
            _mappingFactory = new MappingValueFactory(this);
        }

        public IMappingObject Map<T>()
        {
            return Map<T>(null, null);
        }

        public IMappingObject Map<T>(object environment)
        {
            return Map<T>(environment, null);
        }

        public IMappingObject Map<T>(string mappingName)
        {
            return Map<T>(null, mappingName);
        }

        public IMappingObject Map<T>(object environment, string mappingName)
        {
            if(environment == null)
                environment = Environment;

            var mappingObj = new MappingObject(this, environment).Map<T>(mappingName);

            if (OnValueMapped != null)
                mappingObj.OnValueMapped += ( mapping ) => OnValueMapped.Invoke(mapping);

            _mappingObjects.Add(mappingObj);
            return mappingObj;
        }

        public IMappingObject GetMapping<T>()
        {
            Type type = typeof(T);

            return GetMapping(type, null, null);
        }

        public IMappingObject GetMapping<T>(object environment)
        {
            Type type = typeof(T);

            return GetMapping(type, environment, null);
        }

        public IMappingObject GetMapping<T>(string mappingName)
        {
            Type type = typeof(T);

            return GetMapping(type, null, mappingName);
        }

        public IMappingObject GetMapping<T>(object environment, string mappingName)
        {
            Type type = typeof(T);

            return GetMapping(type, environment, mappingName);
        }

        public IMappingObject GetMapping(Type type)
        {
            return GetMapping(type, null, null);
        }

        public IMappingObject GetMapping(Type type, object environment)
        {
            return GetMapping(type, environment, null);
        }

        public IMappingObject GetMapping(Type type, string mappingName)
        {
            return GetMapping(type, null, mappingName);
        }

        public IMappingObject GetMapping(Type type, object environment, string mappingName)
        {            
            if(environment == null)
                environment = Environment;

            var filteredMappings = FilterByEnvironment(_mappingObjects, environment);

            if(!String.IsNullOrWhiteSpace(mappingName))
            {
                filteredMappings = FilterByName(filteredMappings, mappingName);
            }

            return FilterByType(filteredMappings, type).FirstOrDefault();
        }

        public IReadOnlyList<IMappingObject> GetAllMappings()
        {
            return _mappingObjects.AsReadOnly();
        }

        public IReadOnlyList<IMappingObject> GetAllNamedMappings()
        {
            var mappingObjects = GetAllMappings();

            var namedMappingObjects = mappingObjects.Where(mapping => !string.IsNullOrWhiteSpace(mapping.Name));

            return namedMappingObjects.ToList().AsReadOnly();
        }

        public T GetMappingValue<T>()
        {
            Type type = typeof(T);
            var value = GetMappingValue(type);
            return value is T ? (T)value : default(T);
        }

        public T GetMappingValue<T>(object environment)
        {
            Type type = typeof(T);
            var value = GetMappingValue(type, environment);
            return value is T ? (T)value : default(T);
        }

        public T GetMappingValue<T>(string mappingName)
        {
            Type type = typeof(T);
            var value = GetMappingValue(type, mappingName);
            return value is T ? (T)value : default(T);
        }

        public T GetMappingValue<T>(object environment, string mappingName)
        {
            Type type = typeof(T);
            var value = GetMappingValue(type, environment, mappingName);
            return value is T ? (T)value : default(T);
        }

        public object GetMappingValue(Type type)
        {
            return GetMapping(type)?.MappedValue;
        }

        public object GetMappingValue(Type type, object environment)
        {
            return GetMapping(type, environment)?.MappedValue;
        }

        public object GetMappingValue(Type type, string mappingName)
        {
            return GetMapping(type, mappingName)?.MappedValue;
        }

        public object GetMappingValue(Type type, object environment, string mappingName)
        {
            return GetMapping(type, environment, mappingName)?.MappedValue;
        }

        private IEnumerable<IMappingObject> FilterByEnvironment(IEnumerable<IMappingObject> setToFilter, object filterEnvironment)
        {
            return setToFilter.Where(mapping => mapping.Environment == filterEnvironment);
        }

        private IEnumerable<IMappingObject> FilterByName(IEnumerable<IMappingObject> setToFilter, string filterName)
        {
            return setToFilter.Where(mapping => mapping.Name == filterName);
        }

        private IEnumerable<IMappingObject> FilterByType(IEnumerable<IMappingObject> setToFilter, Type filterType)
        {
            return setToFilter.Where(mapping => mapping.MappedType.IsAssignableFrom(filterType));
        }
    }
}
