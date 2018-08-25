using System.Diagnostics;

namespace API
{
    public static class API
    {
        /// <summary>
        /// Initialize manually the API (only need the first time)
        /// </summary>
        public static bool TryInit(string processName = "gta_sa")
        {
            return Memory.Init(processName);
        }

        public static bool TryInit(Process process)
        {
            return Memory.Init(process);
        }

        public static int Init(string processName = "gta_sa")
        {
            bool r = Memory.Init(processName);

            if (r == true)
                return 1;
            else return 0;
            return -1;
        }

        public static void Init(Process process)
        {
            Memory.Init(process);
        }

        /// <summary>
        /// Uninitialize manually the API 
        /// </summary>
        public static void UnInit()
        {
            Memory.UnInit();
        }
    }
}
