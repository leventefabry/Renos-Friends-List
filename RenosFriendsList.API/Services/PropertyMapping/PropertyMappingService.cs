using System;
using System.Collections.Generic;
using System.Linq;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models.Dog;

namespace RenosFriendsList.API.Services.PropertyMapping
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _dogPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "Name", new PropertyMappingValue(new List<string> { "Name" }) },
            { "BodyType", new PropertyMappingValue(new List<string> { "BodyType" }) },
            { "Gender", new PropertyMappingValue(new List<string> { "Gender" }) }
        };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<DogDto, Dog>(_dogPropertyMapping));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception("Cannot find exact property mapping instance " +
                                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }
    }
}
