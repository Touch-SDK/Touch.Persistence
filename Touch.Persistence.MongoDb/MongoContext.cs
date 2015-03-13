using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Touch.Persistence
{
    sealed internal class MongoContext : IContext
    {
        #region .ctor
        public MongoContext(MongoDatabase db)
        {
            _db = db;
        } 
        #endregion

        #region Data
        private readonly MongoDatabase _db;
        private readonly ConcurrentDictionary<Type, string> _tableNames = new ConcurrentDictionary<Type, string>();
        #endregion

        #region Public methods
        public void RegisterType<T>()
        {
            var type = typeof(T);

            if (BsonClassMap.IsClassMapRegistered(type)) return;

            var contractAttribute =
                type.GetCustomAttributes(typeof(DataContractAttribute), false).FirstOrDefault() as
                DataContractAttribute;

            if (contractAttribute != null && !string.IsNullOrEmpty(contractAttribute.Name))
                _tableNames[type] = contractAttribute.Name;

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

            BsonClassMap.RegisterClassMap<T>(map =>
            {
                foreach (var pair in typeProperties)
                {
                    map.MapProperty(pair.Value.Name).SetElementName(pair.Key);

                    var propertyAttribute =
                        pair.Value.PropertyType.GetCustomAttributes(typeof(DataContractAttribute), false).FirstOrDefault() as
                        DataContractAttribute;

                    if (propertyAttribute != null)
                    {
                        var method = GetType().GetMethod("RegisterType");
                        var generic = method.MakeGenericMethod(pair.Value.PropertyType);
                        generic.Invoke(this, null);
                    }
                }
            });
        }

        private string GetTableName(Type dataContract)
        {
            return _tableNames[dataContract];
        }
        #endregion

        #region IContext members
        public void Delete<T>(string hashKey)
            where T : class, IDocument
        {
            var table = GetTableName(typeof(T));
            var collection = _db.GetCollection(table);

            var query = MongoDB.Driver.Builders.Query.EQ("_id", hashKey);
            var result = collection.Remove(query);

            if (!result.Ok)
                throw new OperationCanceledException(result.ErrorMessage);
        }

        public void Delete<T>(T target)
            where T : class, IDocument
        {
            Delete<T>(target.HashKey);
        }

        public T Get<T>(string hashKey)
            where T : class, IDocument
        {
            var table = GetTableName(typeof(T));
            var collection = _db.GetCollection(table);

            return collection.FindOneByIdAs<T>(BsonValue.Create(hashKey));
        }

        public IEnumerable<T> Query<T>(KeyValuePair<string, string> condition)
            where T : class, IDocument
        {
            throw new NotSupportedException();
        }

        public void Store(IDocument target)
        {
            var table = GetTableName(target.GetType());
            var collection = _db.GetCollection(table);

            var document = BsonDocument.Create(target);

            if (target.HashKey != null)
                document["_id"] = target.HashKey;

            var result = collection.Save(document);

            if (!result.Ok)
                throw new OperationCanceledException(result.ErrorMessage);

            if (result.Upserted != null)
                target.HashKey = result.Upserted.AsString;
        }
        #endregion
    }
}
