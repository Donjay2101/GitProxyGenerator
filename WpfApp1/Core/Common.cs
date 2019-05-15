using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Core
{
    public class Common
    {

        private readonly static Lazy<Common> _instance = new Lazy<Common>(true);


        public static Common Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public Common()
        {
            SetDictionary();
        }

        private Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public string BasePath
        {
            get
            {
                var assemblypath = Assembly.GetExecutingAssembly().Location;
                return assemblypath.Substring(0, assemblypath.LastIndexOf("\\"));
            }
        }

        private void SetDictionary()
        {
            var csvPath = $"{BasePath}\\constants\\code.csv";
            using (StreamReader reader = new StreamReader(csvPath))
            {
                string code = "";
                while ( (code  =reader.ReadLine()) != null)
                {
                    var splitData = code.Split(',');
                    _dictionary.Add(splitData[0], splitData[1]);
                }
            }
        }

        public string[] GetKeys()
        {
            return _dictionary.Keys.ToArray();
        }




        public string EscapeCharacter(string key)
        {
            return _dictionary[key];
        }

    }
}
