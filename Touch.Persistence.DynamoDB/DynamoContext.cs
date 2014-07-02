using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Touch.Persistence
{
    sealed class DynamoContext : IContext
    {
        #region .ctor
        internal DynamoContext(AmazonDynamoDBClient client, bool consistentReads)
        {
            if (client == null) throw new ArgumentNullException("client");
            _client = client;

            _consistentReads = consistentReads;
        }
        #endregion

        #region Data
        /// <summary>
        /// DynamoDB client.
        /// </summary>
        private readonly AmazonDynamoDBClient _client;

        /// <summary>
        /// Consistent reads.
        /// </summary>
        private readonly bool _consistentReads;

        /// <summary>
        /// Data contract table names.
        /// </summary>
        private readonly ConcurrentDictionary<Type, string> _tableNames = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Data contract hash key names.
        /// </summary>
        private readonly ConcurrentDictionary<Type, string> _hashKeyNames = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Data contract properties
        /// </summary>
        private readonly ConcurrentDictionary<Type, IEnumerable<KeyValuePair<string, PropertyInfo>>> _typeProperties = new ConcurrentDictionary<Type, IEnumerable<KeyValuePair<string, PropertyInfo>>>();
        #endregion

        #region IContext implementation
        public T Get<T>(string hashKey) where T : class, IDocument
        {
            var hashKeyName = GetHashKeyName(typeof(T));
            var request = new GetItemRequest
            {
                TableName = GetTableName(typeof(T)),
                Key = new Dictionary<string, AttributeValue> { { hashKeyName, new AttributeValue { S = hashKey } } },
                ConsistentRead = _consistentReads,
            };

            var result = _client.GetItem(request);
            if (result.Item == null) return null;

            return (T)GetContract(typeof(T), result.Item);
        }

        public void Delete<T>(T target) where T : class, IDocument
        {
            Delete<T>(target.HashKey);
        }

        public void Delete<T>(string hashKey) where T : class, IDocument
        {
            var hashKeyName = GetHashKeyName(typeof(T));
            var request = new DeleteItemRequest
            {
                TableName = GetTableName(typeof(T)),
                Key = new Dictionary<string, AttributeValue> { { hashKeyName, new AttributeValue { S = hashKey } } }
            };

            _client.DeleteItem(request);
        }

        public void Store(IDocument target)
        {
            var request = new PutItemRequest
            {
                TableName = GetTableName(target.GetType()),
                Item = GetValues(target)
            };

            _client.PutItem(request);
        }

        #endregion

        #region Private methods
        private void ProccessType(Type type)
        {
            if (_tableNames.ContainsKey(type)) return;

            //Get table name from DataContractAttribute.Name property
            var contractAttribute =
                    type.GetCustomAttributes(typeof(DataContractAttribute), false).FirstOrDefault() as
                    DataContractAttribute;

            if (contractAttribute == null || string.IsNullOrEmpty(contractAttribute.Name))
                throw new ArgumentException("Invalid data contract: missing DataContractAttribute");

            _tableNames[type] = contractAttribute.Name;

            //Get type's properties
            var validProperties = (from property in type.GetProperties()
                                   where property.CanWrite && property.CanRead
                                   select property).ToList();

            var typeProperties = (from property in validProperties
                                  let memberAttribute =
                                    property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault()
                                    as DataMemberAttribute
                                  where memberAttribute != null
                                  let propertyName = !string.IsNullOrEmpty(memberAttribute.Name)
                                    ? memberAttribute.Name
                                    : property.Name
                                  select new KeyValuePair<string, PropertyInfo>(propertyName, property)).ToList();

            var hashProperty = (from property in validProperties
                                let memberAttribute =
                                  property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault()
                                  as DataMemberAttribute
                                where memberAttribute != null && property.Name == "HashKey"
                                select memberAttribute.Name).SingleOrDefault();

            if (string.IsNullOrWhiteSpace(hashProperty))
                throw new ArgumentException("Invalid data contract: missing DataMemberAttribute name for HashKey property");

            _typeProperties[type] = typeProperties;
            _hashKeyNames[type] = hashProperty;
        }

        /// <summary>
        /// Get table name.
        /// </summary>
        /// <param name="dataContract">Data contract type.</param>
        /// <returns>Table name or <c>null</c> if table not found.</returns>
        private string GetTableName(Type dataContract)
        {
            ProccessType(dataContract);
            return _tableNames[dataContract];
        }

        /// <summary>
        /// Get hash key name.
        /// </summary>
        /// <param name="dataContract">Data contract type.</param>
        /// <returns>Hash key name or <c>null</c> if type not found.</returns>
        private string GetHashKeyName(Type dataContract)
        {
            ProccessType(dataContract);
            return _hashKeyNames[dataContract];
        }

        private Dictionary<string, AttributeValue> GetValues(IDocument instance)
        {
            var dataContract = instance.GetType();
            ProccessType(dataContract);

            var propertyValues = from data in _typeProperties[dataContract]
                                 select new KeyValuePair<string, object>(data.Key, data.Value.GetValue(instance, null));

            var result = new Dictionary<string, AttributeValue>();

            foreach (var property in propertyValues)
            {
                if (property.Value == null)
                    continue;

                var type = property.Value.GetType();

                if (type.IsValueType && Activator.CreateInstance(type) == property.Value)
                    continue;

                var value = new AttributeValue();

                if (type == typeof(string))
                    value.S = (string)property.Value;
                else if (type == typeof(List<string>))
                    value.SS = (List<string>)property.Value;
                else if (IsNumericType(type))
                    value.N = property.Value.ToString();
                else
                    value.S = property.Value.ToString();

                result[property.Key] = value;
            }

            return result;
        }

        private IDocument GetContract(Type dataContract, Dictionary<string, AttributeValue> values)
        {
            ProccessType(dataContract);

            var instance = (IDocument)Activator.CreateInstance(dataContract);

            var propertyMap = from data in _typeProperties[dataContract]
                              join prop in values on data.Key equals prop.Key
                              select new { property = data.Value, value = prop.Value };

            foreach (var pair in propertyMap)
                pair.property.SetValue(instance, GetValue(pair.value, pair.property.PropertyType), null);

            return instance;
        }

        static private object GetValue(AttributeValue attributeValue, Type targetType)
        {
            if (targetType == typeof(string))
                return attributeValue.S;

            if (IsNumericType(targetType))
                return Convert.ChangeType(attributeValue.N, targetType);

            if (targetType == typeof(List<string>))
                return attributeValue.SS;

            if (targetType == typeof(Guid))
                return Guid.Parse(attributeValue.S);

            if (targetType == typeof(Guid?))
                return !string.IsNullOrEmpty(attributeValue.S)
                    ? Guid.Parse(attributeValue.S)
                    : default(Guid?);

            throw new ArgumentException("Type is not supported: " + targetType.FullName);
        }

        /// <summary>
        /// Check if type is a numeric type.
        /// </summary>
        static private bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                case TypeCode.Object:
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var subtype = Nullable.GetUnderlyingType(type);
                            return IsNumericType(subtype);
                        }

                        return false;
                    }
            }

            return false;
        }
        #endregion
    }
}
