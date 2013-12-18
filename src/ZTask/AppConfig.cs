using System;
using System.IO;
using YamlDotNet.RepresentationModel.Serialization;

namespace ZTask
{
    public class AppConfig
    {
        public String Proxy { get; set; }

        public static String ConfigFilePath = "./config.yaml";

        private static AppConfig _instance;
        public static AppConfig Load()
        {
            if (_instance != null)
            {
                return _instance;
            }

            if (File.Exists(ConfigFilePath))
            {
                using (var reader = new StreamReader(File.OpenRead(ConfigFilePath)))
                {
                    return new Deserializer().Deserialize<AppConfig>(reader);
                }
            }

            return new AppConfig();
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