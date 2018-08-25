namespace API
{
    public class Statistic
    {
        private static Statistic instance;

        private Statistic() { }

        public static Statistic GetInstance()
        {
            if (instance == null)
                instance = new Statistic();

            return instance;
        }

        /// <summary>
        /// Get the runned meters
        /// </summary>
        /// <returns>Meters</returns>
        public float GetFeetMeters()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            float result = Memory.ReadFloat(Memory.STATÌSTIC_FOOT_METER);

            return result;
        }

        /// <summary>
        /// Get the drive meters with normal vehicles
        /// </summary>
        /// <returns>Meters</returns>
        public float GetVehicleMeters()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            float result = Memory.ReadFloat(Memory.STATÌSTIC_VEHICLE_METER);

            return result;
        }

        /// <summary>
        /// Get the drive meters with bikes
        /// </summary>
        /// <returns>Meters</returns>
        public float GetBikeMeters()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            float result = Memory.ReadFloat(Memory.STATÌSTIC_BIKE_METER);

            return result;
        }

        /// <summary>
        /// Get the flown meters
        /// </summary>
        /// <returns>Meters</returns>
        public float GetHelicopterMeters()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            float result = Memory.ReadFloat(Memory.STATÌSTIC_HELICOPTER_METER);

            return result;
        }

        /// <summary>
        /// Get the drive meters with ships
        /// </summary>
        /// <returns>Meters</returns>
        public float GetShipMeters()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            float result = Memory.ReadFloat(Memory.STATÌSTIC_SHIP_METER);

            return result;
        }

        /// <summary>
        /// Get the swimmed meters
        /// </summary>
        /// <returns>Meters</returns>
        public float GetSwimMeters()
        {
            if (!Memory.IsInit)
                Memory.Init(Memory._processName);

            float result = Memory.ReadFloat(Memory.STATÌSTIC_SWIM_METER);

            return result;
        }
    }
}
