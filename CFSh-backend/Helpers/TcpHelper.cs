using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CFSh_backend.Helpers
{
    public class TcpHelper
    {
        public static void ToThiccClient(string IP, int Port, string FileShard, int FileShardId)
        {
            string returndata = "";
            try
            {
                System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
                clientSocket.Connect(IP, Port);
                NetworkStream serverStream = clientSocket.GetStream();
                // Send "receive" command to Thick client
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("recv " + FileShardId + "\n" + FileShard);
                serverStream.Write(outStream, 0, outStream.Length);
                //serverStream.Flush();
                // Send actual data
                //outStream = System.Text.Encoding.ASCII.GetBytes(FileShard);
                //serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();


                byte[] inStream = new byte[10025];
                while (returndata != "ook")
                {                    
                    int len = serverStream.Read(inStream, 0, inStream.Length);
                    if (len == 0) break;
                    string newData = System.Text.Encoding.ASCII.GetString(inStream).Substring(0, len);
                    returndata = newData;
                    System.Diagnostics.Debug.WriteLine("ReturnData: " + returndata);
                    if (returndata.Length < inStream.Length) break;
                }

                clientSocket.Close();
            }
            catch (SocketException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            if (!string.Equals(returndata, "ook"))
            {
                System.Diagnostics.Debug.WriteLine("|" + returndata + "|");
                throw new Exception(returndata);
            }

        }
        public static string FromThiccClient(string IP, int Port, int FileShardId)
        {
            try
            {
                System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
                clientSocket.Connect(IP, Port);
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("send " + FileShardId);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                string returndata = "";
                byte[] inStream = new byte[10025];
                while (true)
                {
                    int len = serverStream.Read(inStream, 0, inStream.Length);
                    if (len == 0) break;
                    returndata += System.Text.Encoding.ASCII.GetString(inStream).Substring(0,len);
                }
                clientSocket.Close();
                if (returndata.StartsWith("err")) throw new System.IO.FileNotFoundException();
                return returndata;
            }
            catch (SocketException e)
            {
                return "error: " + e.Message;
            }

        }
    }
}
