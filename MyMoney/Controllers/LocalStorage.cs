using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace MyMoney.Controllers
{
    public static class LocalStorage
    {
        public static void SetItem<T>(string key, T value)
        {
            var litedb = new LiteDatabase(StringResource.DatabaseConnection);
            var collection = litedb.GetCollection("localStorage");
            var bson = new BsonDocument()
            {
                ["Key"] = key,
                ["Value"] = BsonMapper.Global.ToDocument(value)
            };
            if (!collection.Update(bson))
            {
                collection.Insert(bson);
                collection.EnsureIndex("$.Key", true);
            }
            litedb.Dispose();
        }
        public static bool TryGetItem<T>(string key, out T value)
        {
            var litedb = new LiteDatabase(StringResource.DatabaseConnection);
            var reader = litedb.Execute("SELECT $.Value FROM localStorage WHERE Key='" + key + "' LIMIT 1");
            if (reader.Read())
            {
                value = BsonMapper.Global.Deserialize<T>(reader.Current);
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
