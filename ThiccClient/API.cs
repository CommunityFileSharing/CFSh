﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ThiccClient
{
    class API
    {
        private static int userId = 0;
        private static int thiccId = 0;
        private static readonly HttpClient client = new HttpClient();
        private static async Task<string> Post(string uri, Dictionary<string, string> values)
        {

            string myJson = "{'Username': 'myusername','Password':'pass'}";

            var response = await client.PostAsync(uri, new StringContent(myJson, Encoding.UTF8, "application/json"));

            return await response.Content.ReadAsStringAsync();
        }

        public static int Login(Config config)
        {
            int id = userId;
            userId += 1;

            string content = @"{{
                ""username"": ""{0}"",
                ""password"": ""{1}""
            }}";

            content = string.Format(content, config.UserName, config.UserPass);
            client.PostAsync("http://localhost:44367/api/Users/authenticate", new StringContent(content, Encoding.UTF8, "application/json"));

            return id;
        }

        public static int ThiccLogin(Config config)
        {
            int id = thiccId;
            thiccId += 1;

            ThiccClient myself = new ThiccClient {
                UserId = config.UserId,
                IP = "127.0.0.1", //GetLocalIPAddress(),
                Port = config.Port,
                FreeSpace = 1000000
            };
            
            HttpResponseMessage response = client.PostAsync("http://localhost:5000/api/Thicc", new StringContent(JsonSerializer.Serialize(myself), Encoding.UTF8, "application/json")).Result;
            string message = response.Content.ReadAsStringAsync().Result;

            ThiccClient b = JsonSerializer.Deserialize<ThiccClient>(message);

            return b.Id;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        public static void SignalAwake(int Id, long freeSpace)
        {

            //Console.WriteLine("Sending request...");
            //Task task = Post("http://localhost:44367/Thicc", values);
            //task.Wait();
            //Console.WriteLine("Done");
        }
    }
}
