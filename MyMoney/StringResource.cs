using System;
using System.Collections.Generic;
using System.Text;

namespace MyMoney
{
    /// <summary>
    /// Store unsual strings
    /// </summary>
    public static class StringResource
    {
        /// <summary>
        /// The location of application in storage
        /// </summary>
        public static string AppLocationFolder { get; private set; } = Environment.CurrentDirectory;
        /// <summary>
        /// The location of application's "Contents" folder in storage
        /// </summary>
        public static string AppContentsFolder { get; private set; } = Environment.CurrentDirectory + @"\Contents";
        /// <summary>
        /// The location of unit config file in storage
        /// </summary>
        public static string UnitsConfigFile { get; private set; } = AppContentsFolder + @"\unitsConfig.json";
        /// <summary>
        /// The location of database file in storage
        /// </summary>
        public static string DatabaseFile { get; private set; } = @"D:\Develop_Programs\MyMoney_v2.db";
        /// <summary>
        /// The connection string for LiteDB
        /// </summary>
        public static string DatabaseConnection { get; private set; } = $"Filename={DatabaseFile};Connection=Shared";
    }
}
