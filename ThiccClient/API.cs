using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ThiccClient
{
    class API
    {
        private static readonly HttpClient client = new HttpClient();
        private static async Task<string> Post(string uri, Dictionary<string, string> values)
        {

            string myJson = "{'Username': 'myusername','Password':'pass'}";

            var response = await client.PostAsync(uri, new StringContent(myJson, Encoding.UTF8, "application/json"));

            return await response.Content.ReadAsStringAsync();
        }

        public static int Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public static int ThiccLogin(int userId)
        {
            throw new NotImplementedException();
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
