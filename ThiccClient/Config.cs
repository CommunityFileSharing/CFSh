using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ThiccClient
{
    public class Config
    {
        private static readonly string ConfigPath = "config.json";

        private string _ServerUrl = null;
        public string ServerUrl { 
            get { 
                if (_ServerUrl == null) { return "http://localhost:44367/Thicc"; }
                return _ServerUrl;
            }
            set
            {
                _ServerUrl = value;
            }
        }
        public int ThiccId { get; set; }
        public int UserId { get; set; }
        public string UserPass { get; set; }
        public string UserName { get; set; }
        public int Port { get; set; }
        public long DiskQuotaInBytes { get; set; }
        public string DataStorePath { get; set; }

        public int LogLevel = 3;

        private static long IntInput(string message, bool byteSize = false)
        {
            Console.WriteLine(message);
            long ret = int.MinValue;
            while (ret == int.MinValue)
            {
                try {
                    string input = Console.ReadLine();
                    if (byteSize)  ret = GetByteCount(input);
                    else ret = Convert.ToInt64(input);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid input");
                    Console.WriteLine(e.Message);
                }
            }
            return ret;
        }

        private static string StringInput(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        private static long GetByteCount(string input)
        {
            List<char> prefixes = new List<char>();
            char[] a = { 'K', 'M', 'G', 'T', 'P', 'E' };
            prefixes.AddRange(a);
            Regex r = new Regex(@"\b\d+[KkMmGgTtPpEe]\b");
            if (r.IsMatch(input))
            {
                char prefixChar = input[input.Length - 1].ToString().ToUpper().ToCharArray()[0];
                int prefixIndex = prefixes.IndexOf(prefixChar);
                long power = Convert.ToInt64(Math.Pow(10, 3 * (1 + prefixIndex)));
                long bbase = Convert.ToInt32(input.Remove(input.Length - 1));
                return bbase * power;
            }
            else 
            {
                Console.WriteLine("Valid size format: {number}{k,m,g,t,p,e}");
                throw new Exception();
            }
        }

        public static Config Load()
        {
            try {
                using (StreamReader file = new StreamReader(ConfigPath))
                {
                    return JsonSerializer.Deserialize<Config>(file.ReadToEnd());
                }
            }
            catch (FileNotFoundException)
            {
                Config config = new Config {
                    UserName = StringInput("Username: "),
                    UserPass = StringInput("Password: "),
                    Port = (int)IntInput("Network port: "),
                    DiskQuotaInBytes = IntInput("Allocated space: ", true),
                    DataStorePath = StringInput("Path ot store data: "),
                    UserId = int.MinValue,
                    ThiccId = int.MinValue
                };
                Save(config);
                return config;
            }
            
        }        

        public void UpdateSpace()
        {

        }

        public static void Save(Config config)
        {
            using (StreamWriter writer = new StreamWriter(ConfigPath, false))
            {
                writer.Write(JsonSerializer.Serialize(config));
            }
        }


    }
}
