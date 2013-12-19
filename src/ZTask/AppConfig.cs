using System;
using System.IO;
using YamlDotNet.RepresentationModel.Serialization;

namespace ZTask
{
    public class AppConfig
    {
        public String Proxy { get; set; }
        public String Background { get; set; }
        public String TextForeground { get; set; }

        public static String ConfigFilePath = "./config.yaml";

        private static AppConfig _instance;
        public static AppConfig Load()
        {
            if (_instance == null)
            {
                if (File.Exists(ConfigFilePath))
                {
                    using (var reader = new StreamReader(File.OpenRead(ConfigFilePath)))
                    {
                        _instance = new Deserializer().Deserialize<AppConfig>(reader);
                    }
                }
                else
                {
                    _instance = new AppConfig();
                }
            }
            
            return _instance;
        }

        public void Save()
        {
            using (var writer = new StreamWriter(File.OpenWrite(ConfigFilePath)))
            {
                new Serializer().Serialize(writer, this);
            }
        }
    }
}