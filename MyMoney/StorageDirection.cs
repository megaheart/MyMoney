using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney
{
    public class StorageDirection
    {
        public static string AppLocationFolder { get; private set; } = Environment.CurrentDirectory;
        public static string AppContentsFolder { get; private set; } = Environment.CurrentDirectory + @"/Contents";
        public static string UnitsConfigFile { get; private set; } = AppContentsFolder + @"/unitsConfig.json";
        public static string DatabaseFile { get; private set; } = @"D:\Develop_Programs\MyMoney_v2.db";

    }
}
