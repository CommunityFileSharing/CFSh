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
        static void Main(string[] args)
        {
            Config Config = Config.Load();
            Log.Write(Severity.Info, "Configuration loaded");

            if (Config.UserId == int.MinValue)
            {
                Log.Write(Severity.Info, "User ID not found, attempting login");
                Config.UserId = API.Login(Config);
                Log.Write(Severity.Info, "Login successful");
            }
            if (Config.ThiccId == int.MinValue)
            {
                Log.Write(Severity.Info, "ThiccClient ID not found, advertising to server " + Config.ServerUrl);
                Config.ThiccId = API.ThiccLogin(Config);
                Log.Write(Severity.Info, "Client login successful, out ID is " + Config.ThiccId);
            }
            Config.UpdateSpace();
            Config.Save(Config);
            
            Directory.CreateDirectory(Config.DataStorePath);
            long freespace = Convert.ToInt64(Config.DiskQuotaInBytes) - GetDirectorySize(Config.DataStorePath);
            API.SignalAwake(Config.ThiccId, freespace);


            TcpListener serverSocket = new TcpListener(IPAddress.Parse(API.GetLocalIPAddress()), Config.Port);
            
            serverSocket.Start();
            Log.Write(Severity.Info, "Server Started");

            int counter = 0;
            while (true)
            {
                counter += 1;
                TcpClient clientSocket = serverSocket.AcceptTcpClient();
                Log.Write(Severity.Info, "Spawning client: conn" + counter);
                ClientHandler client = new ClientHandler(Config, "conn" + counter);
                client.startClient(clientSocket, Convert.ToString(counter));
            }
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }
        static long GetDirectorySize(string p)
        {
            string[] a = Directory.GetFiles(p, "*.*");
            long b = 0;
            foreach (string name in a)
            {
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            return b;
        }
    }

    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;
        public TcpServer(int port, string dataPath)
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
                Console.WriteLine("Connection > " + sData);
                // to write something back.
                // sWriter.WriteLine("Meaningfull things here");
                // sWriter.Flush();
            }
        }
    }

    //Class to handle each client request separatly
    public class ClientHandler
    {
        TcpClient clientSocket;
        string clNo;
        Config config;

        private string ShardPath(string ShardId)
        {
            return Path.Combine(config.DataStorePath, ShardId + ".data");
        }

        public ClientHandler(Config _config, string _clNo)
        {
            config = _config;
            clNo = _clNo;
        }

        private void Write(string data, string ShardId)
        {
            using (StreamWriter file = new StreamWriter(ShardPath(ShardId)))
            {
                file.Write(data);
            }
        }
        private string Read(string ShardId)
        {
            try
            {
                using (StreamReader file = new StreamReader(ShardPath(ShardId)))
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
                        string newData = Encoding.ASCII.GetString(inStream).Substring(0, len);
                        dataFromClient += newData;
                        if (newData.Length < inStream.Length) break;
                    }
                    string[] data = dataFromClient.Split('\n');
                    string[] command = data[0].Split(' ');
                    //Console.WriteLine(" >> " + command[0] + ", id: " + command[1] + ", len: " + data[1].Length);
                    if (command[0] == "recv")
                    {
                        // write to file
                        Log.Write(Severity.Info, clNo + "|Recieving data, shard ID is " + command[1]);
                        Write(data[1], command[1]);
                        string serverResponse = "ook";
                        byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Log.Write(Severity.Info, clNo + "|Recieving successful");
                    }
                    else if (command[0] == "send")
                    {
                        //read from file & send
                        Log.Write(Severity.Info, clNo + "|Sending data, shard ID is " + command[1]);
                        string shardData = Read(command[1]);
                        if (shardData == null)
                        {
                            shardData = "err";
                        }
                        byte[] sendBytes = Encoding.ASCII.GetBytes(shardData);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Log.Write(Severity.Info, clNo + "|Shard " + command[1]+" uploaded");
                    }
                    else if (command[0] == "ping")
                    {
                        Log.Write(Severity.Info, clNo + "|Ping request");
                        byte[] sendBytes = Encoding.ASCII.GetBytes("pong");
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Log.Write(Severity.Info, clNo + "|Pong sent");
                    }
                    else throw new Exception("Invalid command: " + command[0]);


                    //Console.WriteLine(" >> " + "From client-" + clNo + " " + dataFromClient);

                    
                    clientSocket.Close();
                    break;
                }
                catch (IOException)
                {
                    Console.WriteLine(clNo + "|Connection closed");
                    break;
                }
                catch (Exception ex)
                {
                    Log.Write(Severity.Error, clNo + "|"+ex.ToString());
                    break;
                }
            }
        }
    }
}
