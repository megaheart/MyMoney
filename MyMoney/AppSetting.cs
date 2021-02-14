﻿using MyMoney.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MyMoney
{
    public class AppSetting
    {
        public AppSetting()
        {
        }
        public void Setup()
        {
            Config();
        }
        private void Config()
        {
            AmountHelper.Initialize(JsonSerializer.Deserialize<unitConfig>(File.ReadAllText(@"Contents/unitsConfig.json")));
            LiteDB.BsonMapper.Global.RegisterType<Amount>(
                serialize: a => a.Serialize(),
                deserialize: bson => Amount.Parse(bson.AsString)
                );

        }
    }
}