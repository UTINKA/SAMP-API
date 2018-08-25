using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace API
{
    internal class Memory
    {
        #region DLLImport
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, uint lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        #endregion

        private const int RESERVE = 25;

        // GTA and more
        private static uint pid = 0;
        private static Process gtaProcess;
        public static IntPtr handle = IntPtr.Zero;
        public static uint sampModule = 0;
        private static uint gtaModule = 0;

        public static string _processName = "gta_sa";
        private static bool isInit = false;

        private static IntPtr allocMemory = IntPtr.Zero;
        private static IntPtr[] parameterMemory = new IntPtr[RESERVE];

        // Read addresses
        #region SAMP specific and used for more classes
        private static uint OFFSET_SAMP = 0x212A80;
        private static uint OFFSET_PLAYER_POOL = 0x3D9;
        private static uint OFFSET_PLAYER = 0x14;
        #endregion
        #region Need for Game class
        public static uint FRAMES_PER_SECOND = 0xB729A0;
        #endregion
        #region Need for Player class
        // Player
        private static uint OFFSET_PLAYER_BASE = 0xB6F5F0;
        private static uint playerBase = 0;
        public static uint PLAYER_MONEY = 0x0B7CE54;
        public static uint PLAYER_WEAPON = 0x0BAA410;


        // Health and armor
        private static uint OFFSET_PLAYER_HEALTH = 0x540;
        private static uint OFFSET_PLAYER_ARMOR = 0x548;

        public static uint playerHealth = 0;
        public static uint playerArmor = 0;

        // Position
        private static uint OFFSET_PLAYER_MATRIX = 0x14;
        private static uint OFFSET_PLAYER_POSITION_ROTATION = 0x14;
        private static uint OFFSET_PLAYER_POSITION_X = 0x30;
        private static uint OFFSET_PLAYER_POSITION_Y = 0x34;
        private static uint OFFSET_PLAYER_POSITION_Z = 0x38;

        private static uint playerPosition = 0;
        public static uint playerPositionRotation = 0;
        public static uint playerPositionX = 0;
        public static uint playerPositionY = 0;
        public static uint playerPositionZ = 0;

        // Location
        private static uint OFFSET_PLAYER_LOCATION = 0x2F;
        public static uint playerLocation = 0;

        // SAMP informations
        private static uint OFFSET_PLAYER_NAME = 0x2123F7;
        public static uint playerName = 0;

        private static uint OFFSET_PLAYER_ID = 0x4;
        public static uint playerId = 0;

        //public static uint playerNameLength = 
        #endregion
        #region Need for Vehicle class
        // Car
        public static uint OFFSET_VEHICLE_BASE = 0xBA18FC;

        public static uint OFFSET_VEHICLE_MODEL_ID = 0x22;

        public static uint OFFSET_VEHICLE_DAMAGE = 0x4C0;

        public static uint OFFSET_VEHICLE_SPEED_X = 0x44;
        public static uint OFFSET_VEHICLE_SPEED_Y = 0x48;
        public static uint OFFSET_VEHICLE_SPEED_Z = 0x4C;

        public static uint OFFSET_VEHICLE_COLLIDE_STATUS = 0xD8;

        public static uint OFFSET_VEHICLE_LOCKSTATE = 0x4F8;
        public static uint OFFSET_VEHICLE_DRIVER = 0x460; // Address to the pointer of the driver(+0x04 for other passenger pointer)
        public static uint OFFSET_VEHICLE_ENGINESTATE = 0x428;
        public static uint OFFSET_VEHICLE_SIRENSTATE = 0x42D;

        private static uint OFFSET_LOCAL_PLAYER = 0x22;
        private static uint OFFSET_VEHICLE_ID = 0x8;

        public static uint vehicleId = 0;

        #endregion
        #region Need for Statistic class
        // Stats
        public static uint STATÌSTIC_FOOT_METER = 0xB7938C;
        public static uint STATÌSTIC_VEHICLE_METER = 0xB79390;
        public static uint STATÌSTIC_BIKE_METER = 0xB79394;
        public static uint STATÌSTIC_HELICOPTER_METER = 0xB793A0;
        public static uint STATÌSTIC_SHIP_METER = 0xB79398;
        public static uint STATÌSTIC_SWIM_METER = 0xB793E8;
        #endregion
        #region Need for Chat class
        // Chat
        private static uint OFFSET_CHAT_MESSAGE = 0x212A6C;
        private static uint OFFSET_CHAT = 0x21a10c;
        private static uint OFFSET_CHAT_OPEN = 0x55;

        public static uint chatMessage = 0;
        private static uint chat = 0;
        public static uint isChatOpen = 0;

        // Dialog
        private static uint OFFSET_DIALOG = 0x6B9C0;
        private static uint OFFSET_DIALOG_OPEN = 0x28;

        public static uint dialog = 0;
        public static uint isDialogOpen = 0;
        #endregion
        #region Need for RemotePlayer class
        public static uint OFFSET_REMOTE_PLAYERS = 0x2E;
        public static uint OFFSET_REMOTE_PLAYER_DATA = 0x08;
        public static uint OFFSET_REMOTE_PLAYER_SCORE = 0x04;
        public static uint OFFSET_REMOTE_PLAYER_NAME_LENGTH = 0x24;
        public static uint OFFSET_REMOTE_PLAYER_NAME = 0x14;

        public static uint structSamp = 0;
        private static uint structSampPools = 0;
        public static uint structPlayerPool = 0;
        #endregion
        #region Need for World class
        public static uint WORLD_WEATHER = 0xC81320;
        #endregion
        // Function addresses
        #region Need for Chat class
        // Chat
        private static uint OFFSET_FUNCTION_SEND_TEXT = 0x57f0;
        private static uint OFFSET_FUNCTION_SEND_COMMAND = 0x65c60;
        private static uint OFFSET_FUNCTION_ADD_MESSAGE = 0x64520;
        private static uint OFFSET_FUNCTION_SHOW_DIALOG = 0x6B9C0;
        private static uint OFFSET_FUNCTION_UPDATE_PLAYER_DATA = 0x7D10;

        public static uint functionSendSay = 0;
        public static uint functionSendCommand = 0;
        public static uint functionAddChatMessage = 0;
        public static uint functionShowDialog = 0;
        public static uint functionUpdatePlayerData = 0;
        #endregion
        #region Need for Player class
        private static uint OFFSET_FUNCTION_PLAY_AUDIO_STREAM = 0x62da0;
        private static uint OFFSET_FUNCTION_STOP_AUDIO_STREAM = 0x629a0;

        public static uint functionPlayAudioStream = 0;
        public static uint functionStopAudioStream = 0;
        #endregion

        internal static bool Init(string processName = "gta_sa")
        {
            try
            {
                _processName = processName;

                Process[] processes = Process.GetProcessesByName(_processName);

                if (processes.Length > 0 && !isInit)
                {
                    gtaProcess = processes[0];
                    gtaProcess.EnableRaisingEvents = true;
                    gtaProcess.Exited += OnGtaExited;

                    pid = (uint)processes[0].Id;
                    ProcessModuleCollection modules = processes[0].Modules;

                    foreach (ProcessModule item in modules)
                    {
                        if (item.ModuleName == "samp.dll")
                        {
                            sampModule = (uint)item.BaseAddress;
                        }
                        else if (item.ModuleName == _processName + ".exe")
                        {
                            gtaModule = (uint)item.BaseAddress;
                        }
                    }

                    handle = OpenProcess(0x1F0FFF, 1, pid);

                    //// Allocate
                    allocMemory = VirtualAllocEx(handle, IntPtr.Zero, RESERVE * 1024, 0x1000 | 0x2000, 0x40);
                    int x = Marshal.GetLastWin32Error();

                    for (int i = 0; i < parameterMemory.Length; i++)
                    {
                        parameterMemory[i] = allocMemory + (1024 * i);
                    }

                    InitVariables();

                    isInit = true;
                    return true;
                }
                return false;
            }
            catch (Exception)
            { }

            return true;
        }

        internal static bool Init(Process process)
        {
            try
            {
                _processName = process.ProcessName;

                if (!isInit)
                {
                    gtaProcess = process;
                    gtaProcess.EnableRaisingEvents = true;
                    gtaProcess.Exited += OnGtaExited;

                    pid = (uint)process.Id;
                    ProcessModuleCollection modules = process.Modules;

                    foreach (ProcessModule item in modules)
                    {
                        if (item.ModuleName == "samp.dll")
                        {
                            sampModule = (uint)item.BaseAddress;
                        }
                        else if (item.ModuleName == _processName + ".exe")
                        {
                            gtaModule = (uint)item.BaseAddress;
                        }
                    }

                    handle = OpenProcess(0x1F0FFF, 1, pid);

                    //// Allocate
                    allocMemory = VirtualAllocEx(handle, IntPtr.Zero, RESERVE * 1024, 0x1000 | 0x2000, 0x40);
                    int x = Marshal.GetLastWin32Error();

                    for (int i = 0; i < parameterMemory.Length; i++)
                    {
                        parameterMemory[i] = allocMemory + (1024 * i);
                    }

                    InitVariables();

                    isInit = true;
                    return true;
                }
            }
            catch (Exception)
            { }

            return false;
        }

        private static void InitVariables()
        {

            // Variables
            //string version = ReadString(sampModule + 0xCEAB4, 7);
            OFFSET_SAMP = 0x212AB8;

            OFFSET_CHAT_MESSAGE = 0x21a0e4;
            OFFSET_CHAT = 0x21a10c;
            OFFSET_DIALOG = 0x6B9C0;

            OFFSET_FUNCTION_SEND_TEXT = 0x57f0;
            OFFSET_FUNCTION_SEND_COMMAND = 0x65c60;
            OFFSET_FUNCTION_ADD_MESSAGE = 0x64520;
            OFFSET_FUNCTION_SHOW_DIALOG = 0x6B9C0;
            OFFSET_FUNCTION_UPDATE_PLAYER_DATA = 0x8a10;

            OFFSET_REMOTE_PLAYER_NAME_LENGTH = 0x27;
            OFFSET_REMOTE_PLAYER_NAME = 0x17;
            OFFSET_REMOTE_PLAYER_SCORE = 0x3;

            OFFSET_PLAYER_NAME = 0x219A6F;

            #region SAMP specified
            structSamp = BitConverter.ToUInt32(ReadMemory(sampModule + OFFSET_SAMP, 4), 0);
            structSampPools = BitConverter.ToUInt32(ReadMemory(structSamp + OFFSET_PLAYER_POOL, 4), 0);
            structPlayerPool = BitConverter.ToUInt32(ReadMemory(structSampPools + OFFSET_PLAYER, 4), 0);
            #endregion
            #region Player
            // Base of Player
            playerBase = BitConverter.ToUInt32(ReadMemory(OFFSET_PLAYER_BASE, 4), 0);

            // HP
            playerHealth = playerBase + OFFSET_PLAYER_HEALTH;
            playerArmor = playerBase + OFFSET_PLAYER_ARMOR;

            // Position
            playerPosition = BitConverter.ToUInt32(ReadMemory(playerBase + OFFSET_PLAYER_MATRIX, 4), 0);
            playerPositionRotation = playerPosition + OFFSET_PLAYER_POSITION_ROTATION;
            playerPositionX = playerPosition + OFFSET_PLAYER_POSITION_X;
            playerPositionY = playerPosition + OFFSET_PLAYER_POSITION_Y;
            playerPositionZ = playerPosition + OFFSET_PLAYER_POSITION_Z;

            // Interior Boolean
            playerLocation = playerBase + OFFSET_PLAYER_LOCATION;

            // SAMP informations
            playerName = sampModule + OFFSET_PLAYER_NAME;
            playerId = structPlayerPool + OFFSET_PLAYER_ID;
            #endregion

            #region Chat
            // Chat
            while ((chatMessage = BitConverter.ToUInt32(ReadMemory((uint)sampModule + OFFSET_CHAT_MESSAGE, 4), 0)) == 0)
            {

            }
            //System.Threading.Thread.Sleep(100);
            while ((chat = BitConverter.ToUInt32(ReadMemory((uint)sampModule + OFFSET_CHAT, 4), 0)) == 0)
            {

            }
            //System.Threading.Thread.Sleep(100);

            isChatOpen = chat + OFFSET_CHAT_OPEN;
            #endregion
            #region Dialog
            // Dialog
            dialog = BitConverter.ToUInt32(ReadMemory((uint)sampModule + OFFSET_DIALOG, 4), 0);

            isDialogOpen = dialog + OFFSET_DIALOG_OPEN;
            #endregion
            #region Vehicle
            vehicleId = ReadUInteger(structPlayerPool + OFFSET_LOCAL_PLAYER) + OFFSET_VEHICLE_ID;
            #endregion
            #region Player Infos

            #endregion
            #region World
            #endregion

            // Functions
            #region Functions
            functionSendSay = sampModule + OFFSET_FUNCTION_SEND_TEXT;
            functionSendCommand = sampModule + OFFSET_FUNCTION_SEND_COMMAND;
            functionAddChatMessage = sampModule + OFFSET_FUNCTION_ADD_MESSAGE;
            functionShowDialog = sampModule + OFFSET_FUNCTION_SHOW_DIALOG;
            functionUpdatePlayerData = sampModule + OFFSET_FUNCTION_UPDATE_PLAYER_DATA;

            functionPlayAudioStream = sampModule + OFFSET_FUNCTION_PLAY_AUDIO_STREAM;
            functionStopAudioStream = sampModule + OFFSET_FUNCTION_STOP_AUDIO_STREAM;
            #endregion
        }

        internal static void UnInit()
        {
            if (isInit && handle != IntPtr.Zero)
            {
                Process[] processes = Process.GetProcessesByName("gta_sa");
                if (processes.Length > 0 && processes[0].Id == pid)
                {
                    CloseHandle(handle);
                }

                isInit = false;
                pid = 0;
                handle = IntPtr.Zero;
                sampModule = 0;
                gtaModule = 0;
            }
        }

        internal static bool ReadBoolean(uint address)
        {
            byte[] bytes = ReadMemory(address, 1);

            bool result = BitConverter.ToBoolean(bytes, 0);

            return result;
        }

        internal static string ReadString(uint address, uint length)
        {
            byte[] bytes = ReadMemory(address, length);

            string result = Encoding.ASCII.GetString(bytes);

            return result;
        }

        internal static int ReadInteger(uint address)
        {
            byte[] bytes = ReadMemory(address, 4);

            int result = BitConverter.ToInt32(bytes, 0);

            return result;
        }

        internal static Int16 ReadInteger16(uint address)
        {
            byte[] bytes = ReadMemory(address, 2);

            Int16 result = BitConverter.ToInt16(bytes, 0);

            return result;
        }

        internal static uint ReadUInteger(uint address)
        {
            byte[] bytes = ReadMemory(address, 4);

            uint result = BitConverter.ToUInt32(bytes, 0);

            return result;
        }

        internal static float ReadFloat(uint address)
        {
            byte[] bytes = ReadMemory(address, 4);

            float result = BitConverter.ToSingle(bytes, 0);

            return result;
        }

        internal static byte ReadByte(uint address)
        {
            byte[] bytes = ReadMemory(address, 1);

            byte result = bytes[0];

            return result;
        }

        internal static byte[] ReadMemory(uint address, uint size)
        {
            byte[] bytes = new byte[size];
            uint bytesReaded = 0;

            ReadProcessMemory(handle, (IntPtr)address, bytes, size, ref bytesReaded);

            return bytes;
        }

        internal static void WriteBoolean(uint address, bool boolean)
        {
            if (boolean)
                WriteMemory(address, new byte[] { 0 }, 1);
            else
                WriteMemory(address, new byte[] { 1 }, 1);
        }

        internal static void WriteByte(uint address, byte value)
        {
            WriteMemory(address, new byte[] { value }, 1);
        }

        internal static bool WriteMemory(uint address, byte[] bytes, uint size)
        {
            uint bytesWritten = 0;
            bool result = false;

            if (WriteProcessMemory(handle, (IntPtr)address, bytes, size, ref bytesWritten))
                result = true;

            return result;
        }

        internal static bool WriteString(uint address, string text)
        {
            byte[] bytes = Encoding.Default.GetBytes(text);

            return WriteMemory(address, bytes, (uint)512);
        }

        internal static bool WriteFloat(uint address, float dec)
        {
            byte[] bytes = BitConverter.GetBytes(dec);

            return WriteMemory(address, bytes, (uint)512);
        }

        internal static void Call(uint address, bool stackClear, params object[] parameter)
        {
            List<byte> data = new List<byte>();

            int usedParameters = 0;
            for (int i = parameter.Length - 1; i >= 0; i--)
            {
                IntPtr memoryAddress = IntPtr.Zero;
                Type type = parameter[i].GetType();
                if (type == typeof(string) && usedParameters <= parameterMemory.Length - 1)
                {
                    memoryAddress = parameterMemory[usedParameters];
                    if (!WriteString((uint)memoryAddress, (string)parameter[i]))
                        return;
                    usedParameters++;
                }
                else if (type == typeof(uint))
                {
                    memoryAddress = new IntPtr(Convert.ToUInt32(parameter[i]));
                }
                else if (type == typeof(int))
                {
                    memoryAddress = new IntPtr(Convert.ToInt32(parameter[i]));
                }
                else if (type == typeof(Single) && usedParameters <= parameterMemory.Length - 1)
                {
                    memoryAddress = parameterMemory[usedParameters];
                    if (!WriteFloat((uint)memoryAddress, (float)parameter[i]))
                        return;
                    usedParameters++;
                }
                else
                    return;

                data.Add(0x68);
                data.AddRange(BitConverter.GetBytes((uint)memoryAddress));
            }

            data.Add(0xE8);
            int offset = (int)address - ((int)parameterMemory[parameterMemory.Length - 1] + (parameter.Length * 5 + 5));
            data.AddRange(BitConverter.GetBytes(offset));

            if (stackClear)
            {
                data.AddRange(new byte[] { 0x83, 0xC4 });
                data.Add(Convert.ToByte(parameter.Length * 4));
            }
            data.Add(0xC3);

            if (!WriteMemory((uint)parameterMemory[parameterMemory.Length - 1], data.ToArray(), (uint)data.Count))
                return;

            IntPtr thread = CreateRemoteThread(handle, IntPtr.Zero, 0, (uint)parameterMemory[parameterMemory.Length - 1], IntPtr.Zero, 0, IntPtr.Zero);
            WaitForSingleObject(thread, 0xFFFFFFFF);
        }

        internal static void Call(uint address, byte[] thisCall, bool stackClear, params object[] parameter)
        {
            List<byte> data = new List<byte>();

            data.AddRange(thisCall);

            int usedParameters = 0;
            for (int i = parameter.Length - 1; i >= 0; i--)
            {
                IntPtr memoryAddress = IntPtr.Zero;
                Type type = parameter[i].GetType();
                if (type == typeof(string) && usedParameters <= parameterMemory.Length - 1)
                {
                    memoryAddress = parameterMemory[usedParameters];
                    if (!WriteString((uint)memoryAddress, (string)parameter[i]))
                        return;
                    usedParameters++;
                }
                else if (type == typeof(uint))
                {
                    memoryAddress = new IntPtr(Convert.ToUInt32(parameter[i]));
                }
                else if (type == typeof(int))
                {
                    memoryAddress = new IntPtr(Convert.ToInt32(parameter[i]));
                }
                else if (type == typeof(Single) || type == typeof(Double) || type == typeof(float))
                {
                    if (type == typeof(Single))
                        memoryAddress = new IntPtr(Convert.ToInt32(BitConverter.ToInt32(BitConverter.GetBytes((Single)parameter[i]), 0)));
                    else if (type == typeof(Double))
                        memoryAddress = new IntPtr(Convert.ToInt32(BitConverter.ToInt32(BitConverter.GetBytes((Double)parameter[i]), 0)));
                    else if (type == typeof(float))
                        memoryAddress = new IntPtr(Convert.ToInt32(BitConverter.ToInt32(BitConverter.GetBytes((float)parameter[i]), 0)));
                }
                /*
            else if (type == typeof(Single) && usedParameters <= parameterMemory.Length - 1)
            {
                memoryAddress = parameterMemory[usedParameters];
                if (!WriteFloat((uint)memoryAddress, (float)parameter[i]))
                    return;
                usedParameters++;
            }*/
                else
                    return;

                data.Add(0x68);
                data.AddRange(BitConverter.GetBytes((uint)memoryAddress));
            }

            data.Add(0xE8);
            int offset = (int)address - ((int)parameterMemory[parameterMemory.Length - 1] + ((parameter.Length * 5 + 5) + thisCall.Length));
            data.AddRange(BitConverter.GetBytes(offset));

            if (stackClear)
            {
                data.AddRange(new byte[] { 0x83, 0xC4 });
                data.Add(Convert.ToByte(parameter.Length * 4));
            }
            data.Add(0xC3);

            if (!WriteMemory((uint)parameterMemory[parameterMemory.Length - 1], data.ToArray(), (uint)data.Count))
                return;

            IntPtr thread = CreateRemoteThread(handle, IntPtr.Zero, 0, (uint)parameterMemory[parameterMemory.Length - 1], IntPtr.Zero, 0, IntPtr.Zero);
            WaitForSingleObject(thread, 0xFFFFFFFF);
        }

        private static void OnGtaExited(object sender, EventArgs e)
        {
            isInit = false;
        }


        internal static bool IsInit
        {
            get { return isInit; }
        }

        internal static IntPtr[] ParameterMemory
        {
            get { return parameterMemory; }
        }
    }
}
