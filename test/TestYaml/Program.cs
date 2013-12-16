using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.RepresentationModel.Serialization;

namespace TestYaml
{
    class Program
    {
        public static void Main(string[] args)
        {
            var config = AppConfig.Load();
            Console.WriteLine(config.Name);
            config.Name = "Eason";
            config.Save();
            Console.ReadLine();
        }
    }
}
