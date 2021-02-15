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
            collection.EnsureIndex("$.Key", true);
            collection.Insert(new BsonDocument()
            {
                ["Key"] = key,
                ["Value"] = BsonMapper.Global.ToDocument(value)
            });
            litedb.Dispose();
        }
        public static T GetItem<T>(string key)
        {
            var litedb = new LiteDatabase(StringResource.DatabaseConnection);
            var reader = litedb.Execute("SELECT $.Value FROM localStorage WHERE Key='" + key + "' LIMIT 1");
            reader.Read();
            var doc = reader.Current;
            reader.Dispose();
            litedb.Dispose();
            return BsonMapper.Global.Deserialize<T>(doc);
        }
    }
}
