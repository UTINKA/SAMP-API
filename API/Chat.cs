using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace API
{
    public class Chat
    {
        private static Chat instance;

        private FileSystemWatcher watcher;
        private StreamReader reader;
        private FileInfo info;

        private const string CHATLOG_FILE = "chatlog.txt";
        private string chatlogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GTA San Andreas User Files\\SAMP\\");

        private bool blocked;

        public bool IsBlocked
        {
            get
            {
                return blocked;
            }
        }

        /// <summary>
        /// Chat-Message delegate
        /// </summary>
        /// <param name="time">When the message was sent</param>
        /// <param name="message">The message itself</param>
        public delegate void OnChatMessageReceived(DateTime time, String message);

        /// <summary>
        /// This Event will provide you with the latest chat-messages
        /// </summary>
        public event OnChatMessageReceived OnChatMessage;

        private Chat()
        {
            try
            {
                reader = new StreamReader(new FileStream(Path.Combine(chatlogPath, CHATLOG_FILE), FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.Default, true);
                reader.ReadToEnd();

                info = new FileInfo(Path.Combine(chatlogPath, CHATLOG_FILE));

                watcher = new FileSystemWatcher();
                watcher.Path = chatlogPath;
                watcher.Filter = CHATLOG_FILE;
                watcher.Changed += ChangeReceived;
                watcher.EnableRaisingEvents = true;

                AppDomain.CurrentDomain.DomainUnload += OnUnload;
            }
            catch (Exception)
            {
                chatlogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "GTA San Andreas User Files\\SAMP\\");
                reader = new StreamReader(new FileStream(Path.Combine(chatlogPath, CHATLOG_FILE), FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.Default, true);
                reader.ReadToEnd();

                info = new FileInfo(Path.Combine(chatlogPath, CHATLOG_FILE));

                watcher = new FileSystemWatcher();
                watcher.Path = chatlogPath;
                watcher.Filter = CHATLOG_FILE;
                watcher.Changed += ChangeReceived;
                watcher.EnableRaisingEvents = true;

                AppDomain.CurrentDomain.DomainUnload += OnUnload;
            }

            blocked = false;
        }

        internal void ChangeReceived(object ob, FileSystemEventArgs e)
        {
            info.Refresh();
            if (info.Length < reader.BaseStream.Position)
            {
                reader.BaseStream.Position = 0;
                reader.DiscardBufferedData();
            }
            String line = "";
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line == "")
                    continue;
                else
                {
                    List<String> splitted = line.Split(' ').ToList();
                    DateTime date = DateTime.Parse(splitted[0].Remove(0, 1).Remove(splitted[0].Length - 2));
                    splitted.RemoveAt(0);
                    if (OnChatMessage != null)
                        OnChatMessage(date, string.Join(" ", splitted));
                }
            }
        }

        public void Close()
        {

        }

        public static Chat GetInstance()
        {
            if (instance == null)
                instance = new Chat();

            return instance;
        }

        /// <summary>
        /// Check if the chat is open
        /// </summary>
        /// <returns>True if its open, else false</returns>
        public bool IsOpen()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            bool result = Memory.ReadBoolean(Memory.isChatOpen);

            return result;
        }

        /// <summary>
        /// Check if a dialog is open
        /// </summary>
        /// <returns>True if its open, else false</returns>
        public bool IsDialogOpen()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            bool result = Memory.ReadBoolean(Memory.isDialogOpen);

            return result;
        }

        /*
         
         SendChat(wText) {
     wText := "" wText
    
    if(!checkHandles())
        return false
    
    dwFunc:=0
    if(SubStr(wText, 1, 1) == "/") {
        dwFunc := dwSAMP + FUNC_SAMP_SENDCMD
    } else {
        dwFunc := dwSAMP + FUNC_SAMP_SENDSAY
    }
    
    callWithParams(hGTA, dwFunc, [["s", wText]], false)
    
    ErrorLevel := ERROR_OK
    return true
}*/



        /// <summary>
        /// Send a message/command to the server
        /// </summary>
        /// <param name="message">The message/command</param>
        /// <param name="args">Arguments for a command, e.g an ID</param>
        public void Send(string message, params object[] args)
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            if (message.Length != 0)
            {
                if (args.Length > 0)
                    message += " " + string.Join(" ", args);
                if (message[0] == '/')
                    Memory.Call(Memory.functionSendCommand, false, message);
                else
                    Memory.Call(Memory.functionSendSay, false, message);
            }
        }

        /// <summary>
        /// Add a new message in the SAMP chat (only local)
        /// </summary>
        /// <param name="text">The text to be written</param>
        /// <param name="color">A color in Hex</param>
        public void AddMessage(string text, string color = "FFFFFF")
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            Memory.Call(Memory.functionAddChatMessage, true, (int)Memory.chatMessage, "{" + color + "}" + text);
        }

        /// <summary>
        /// Add a new message in the SAMP chat (only local)
        /// </summary>
        /// <param name="text">The text to be written</param>
        /// <param name="color">A Color-Type</param>
        public void AddMessage(string text, Color color)
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            Memory.Call(Memory.functionAddChatMessage, true, (int)Memory.chatMessage, "{" + Util.ColorToHexRGB(color) + "}" + text);
        }

        /// <summary>
        /// Add a new message in the SAMP chat (only local)
        /// </summary>
        /// <param name="prefix">The prefix to be written</param>
        /// <param name="prefixColor">A prefix color in Hex</param>
        /// <param name="text">The text to be written</param>
        /// <param name="color">A color in Hex</param>
        public void AddMessage(string prefix, string prefixColor, string text, string color = "FFFFFF")
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            Memory.Call(Memory.functionAddChatMessage, true, (int)Memory.chatMessage, "{" + prefixColor + "}" + prefix + " {" + color + "}" + text);
        }

        /// <summary>
        /// Add a new message in the SAMP chat (only local)
        /// </summary>
        /// <param name="prefix">The prefix to be written</param>
        /// <param name="prefixColor">A Color-Type</param>
        /// <param name="text">The text to be written</param>
        /// <param name="color">A Color-Type</param>
        public void AddMessage(string prefix, Color prefixColor, string text, Color color)
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            Memory.Call(Memory.functionAddChatMessage, true, (int)Memory.chatMessage, "{" + Util.ColorToHexRGB(prefixColor) + "}" + prefix + " {" + Util.ColorToHexRGB(color) + "}" + text);
        }


        public void ShowDialog(DialogStyle style, string caption, string text, string button = "", string button2 = "")
        {
            List<byte> ptr = new List<byte>();
            ptr.Add(0xB9);
            ptr.AddRange(BitConverter.GetBytes((uint)Memory.dialog));
            Memory.Call(Memory.functionShowDialog, ptr.ToArray(), false, 1, (int)style, caption, text, button, button2, 0);
        }

        // UNDONE
        public void BlockChatInput()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            if (blocked)
            {
                Int32 nop = 0xA164;
                Memory.WriteMemory(Memory.functionSendSay, BitConverter.GetBytes(nop), 2);
                Memory.WriteMemory(Memory.functionSendCommand, BitConverter.GetBytes(nop), 2);
            }
            else
            {
                short nop = 0x04C2;
                Memory.WriteMemory(Memory.functionSendSay, BitConverter.GetBytes(nop), 2);
                Memory.WriteMemory(Memory.functionSendCommand, BitConverter.GetBytes(nop), 2);
            }
            blocked = !blocked;
        }

        void OnUnload(object sender, EventArgs e)
        {
            if (blocked)
            {

            }
        }
    }
}
