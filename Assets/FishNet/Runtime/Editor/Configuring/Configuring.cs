﻿using System;
using System.IO;
using System.Xml.Serialization;
#if UNITY_EDITOR
#endif

namespace FishNet.Configuring
{
    public class Configuration
    {
        /// <summary>
        ///     File name for configuration disk data.
        /// </summary>
        public const string CONFIG_FILE_NAME = "FishNet.Config.XML";

        /// <summary>
        /// </summary>
        private static ConfigurationData _configurations;

        /// <summary>
        ///     ConfigurationData to use.
        /// </summary>
        public static ConfigurationData Configurations
        {
            get
            {
                if (_configurations == null)
                    _configurations = LoadConfigurationData();
                if (_configurations == null)
                    throw new Exception(
                        "Fish-Networking Configurations could not be loaded. Certain features such as code-stripping may not function.");
                return _configurations;
            }
            private set => _configurations = value;
        }

        /// <summary>
        ///     Returns the path for the configuration file.
        /// </summary>
        /// <returns></returns>
        internal static string GetAssetsPath(string additional = "")
        {
            var a = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            if (additional != "")
                a = Path.Combine(a, additional);
            return a;
        }

        /// <summary>
        ///     Returns FishNetworking ConfigurationData.
        /// </summary>
        /// <returns></returns>
        internal static ConfigurationData LoadConfigurationData()
        {
            //return new ConfigurationData();
            if (_configurations == null || !_configurations.Loaded)
            {
                var configPath = GetAssetsPath(CONFIG_FILE_NAME);
                //string configPath = string.Empty;
                //File is on disk.
                if (File.Exists(configPath))
                {
                    FileStream fs = null;
                    try
                    {
                        var serializer = new XmlSerializer(typeof(ConfigurationData));
                        fs = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        _configurations = (ConfigurationData) serializer.Deserialize(fs);
                    }
                    finally
                    {
                        fs?.Close();
                    }

                    _configurations.Loaded = true;
                }
                else
                {
                    //If null then make a new instance.
                    if (_configurations == null)
                        _configurations = new ConfigurationData();
                    //Don't unset loaded, if its true then it should have proper info.
                    //_configurationData.Loaded = false;
                }
            }

            return _configurations;
        }
    }
}