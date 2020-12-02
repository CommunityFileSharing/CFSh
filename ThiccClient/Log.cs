using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ThiccClient
{
    public class Log
    {
        private static readonly object locker = new object();

        public static int LogLevel = 3;

        public static void Write(Severity severity, string message)
        {
            if ((int)severity <= LogLevel)
            lock (locker)
            {
                StreamWriter SW;
                Message m = new Message(severity, message);
                SW = File.AppendText("Log.txt");
                SW.WriteLine(JsonSerializer.Serialize(m) + '\n');
                SW.Close();
            }
        }
    }

    public enum Severity
    {
        Critical,
        Error,
        Warning,
        Info,
        Debug
    }


    class Message
    {
        Severity _severity;
        public string Severity { 
            get
            {
                return _severity.ToString();
            }
        }
        public string message { get; set; }
        DateTime timestamp { get; set; }

        public Message(Severity severity, string msg)
        {            
            _severity = severity;
            message = msg;
            timestamp = DateTime.Now;
        }
    }
}
