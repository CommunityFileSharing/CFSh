using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThiccClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> Post(string uri, Dictionary<string, string> values)
        {            

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(uri, content);

            return await response.Content.ReadAsStringAsync();
        }


        static void Main(string[] args)
        {

        //public int Id { get; set; }
        //public int UserId { get; set; }
        //public int FreeSpace { get; set; }
        //public int UsedSpace { get; set; }
        //public string IP { get; set; }
        //public int Port { get; set; }
            var values = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "UserId", "1" },
                { "FreeSpace", "35234333" },
                { "UsedSpace", "0" },
                { "IP", "127.0.0.1" },
                { "Port", "8888" }
            };
            //Console.WriteLine("Sending request...");
            //Task task = Post("http://localhost:44367/Thicc", values);
            //task.Wait();
            //Console.WriteLine("Done");



            TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");

            counter = 0;
            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                handleClient client = new handleClient();
                client.startClient(clientSocket, Convert.ToString(counter));
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }
    }

    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;
        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();
            _isRunning = true;
            LoopClients();
        }
        public void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();
                // client found.
                // create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }
        public void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;
            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested
            Boolean bClientConnected = true;
            String sData = null;
            while (bClientConnected)
            {
                // reads from stream
                sData = sReader.ReadLine();
                // shows content on the console.
                Console.WriteLine("Client > " + sData);
                // to write something back.
                // sWriter.WriteLine("Meaningfull things here");
                // sWriter.Flush();
            }
        }
    }

    //Class to handle each client request separatly
    public class handleClient
    {
        TcpClient clientSocket;
        string clNo;

        private void Write(string data, string shardId)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(shardId + ".data"))
            {
                file.Write(data);
            }
        }
        private string Read(string ShardId)
        {
            try
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader(ShardId + ".data"))
                {
                    return file.ReadToEnd();
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            while ((true))
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    string dataFromClient = "";
                    byte[] inStream = new byte[10025];
                    while (true)
                    {
                        int len = networkStream.Read(inStream, 0, inStream.Length);
                        if (len == 0) break;
                        string newData = System.Text.Encoding.ASCII.GetString(inStream).Substring(0, len);
                        dataFromClient += newData;
                        if (newData.Length < inStream.Length) break;
                    }
                    string[] data = dataFromClient.Split('\n');
                    string[] command = data[0].Split(' ');
                    //Console.WriteLine(" >> " + command[0] + ", id: " + command[1] + ", len: " + data[1].Length);
                    if (command[0] == "recv")
                    {
                        // write to file
                        Write(data[1], command[1]);
                        string serverResponse = "ook";
                        byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Console.WriteLine(" >> " + serverResponse);
                    }
                    else if (command[0] == "send")
                    {
                        //read from file & send
                        string shardData = Read(command[1]);
                        if (shardData == null)
                        {
                            shardData = "err";
                        }
                        byte[] sendBytes = Encoding.ASCII.GetBytes(shardData);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                    }
                    else throw new Exception("Invalid command: " + command[0]);


                    //Console.WriteLine(" >> " + "From client-" + clNo + " " + dataFromClient);

                    
                    clientSocket.Close();
                    break;
                }
                catch (System.IO.IOException)
                {
                    Console.WriteLine("Connection closed");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                    break;
                }
            }
        }
    }
}
