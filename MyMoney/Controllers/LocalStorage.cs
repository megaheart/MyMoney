using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using LiteDB;

namespace MyMoney.Controllers
{
    public static class LocalStorage
    {
        public class KeyValue
        {
            [BsonId]
            public int Id { set; get; }
            public string Key { set; get; }
            public object Value { set; get; }
        }
        public static void SetItem<T>(string key, T value)
        {
            if (key.Length > 30) throw new Exception("Item's key must include less than 31 characters");
            var litedb = new LiteDatabase(StringResource.DatabaseConnection);
            var collection = litedb.GetCollection<KeyValue>("localStorage");
            collection.EnsureIndex(x => x.Key, true);
            var result = collection.FindOne(x => x.Key == key);
            if (result == null)
            {
                result = new KeyValue()
                {
                    Key = key,
                    Value = value
                };
                collection.Insert(result);
            }
            else if (value == null)
            {
                collection.Delete(result.Id);
            }
            else
            {
                result.Value = value;
                collection.Update(result);
            }
            litedb.Dispose();
        }
        public static void SetItemAsync<T>(string key, T value)
        {
            var bson = BsonMapper.Global.Serialize(value);
            if (key.Length > 30) throw new Exception("Item's key must include less than 31 characters");
            new Thread(() =>
            {
                var litedb = new LiteDatabase(StringResource.DatabaseConnection);
                var collection = litedb.GetCollection<KeyValue>("localStorage");
                collection.EnsureIndex(x => x.Key, true);
                var result = collection.FindOne(x => x.Key == key);
                if (result == null)
                {
                    result = new KeyValue()
                    {
                        Key = key,
                        Value = bson
                    };
                    collection.Insert(result);
                }
                else if (bson == null)
                {
                    collection.Delete(result.Id);
                }
                else
                {
                    result.Value = bson;
                    collection.Update(result);
                }
                litedb.Dispose();
            }).Start();
        }
        public static bool TryGetItem<T>(string key, out T value)
        {
            var litedb = new LiteDatabase(StringResource.DatabaseConnection);
            var reader = litedb.Execute("SELECT $.Value FROM localStorage WHERE Key='" + key + "' LIMIT 1");
            if (reader.Read())
            {
                value = BsonMapper.Global.Deserialize<T>(reader.Current["Value"]);
                reader.Dispose();
                litedb.Dispose();
                return true;
            }
            else
            {
                value = default(T);
                reader.Dispose();
                litedb.Dispose();
                return false;
            }
        }
    }
}
