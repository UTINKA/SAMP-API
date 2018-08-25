namespace API
{
    public class World
    {
        private static World instance;

        private World() { }

        public static World GetInstance()
        {
            if (instance == null)
                instance = new World();

            return instance;
        }

        /// <summary>
        /// Get the current weather
        /// </summary>
        /// <returns>Weather name</returns>
        public string GetWeather()
        {
            if (!Memory.IsInit)
                Memory.Init();

            string weatherName = "Unknown";
            short weatherId = Memory.ReadInteger16(Memory.WORLD_WEATHER);

            switch (weatherId)
            {
                case 0:
                    weatherName = "EXTRASUNNY_LA";
                    break;
                case 1:
                    weatherName = "SUNNY_LA";
                    break;
                case 2:
                    weatherName = "EXTRASUNNY_SMOG_LA";
                    break;
                case 3:
                    weatherName = "SUNNY_SMOG_LA";
                    break;
                case 4:
                    weatherName = "CLOUDY_LA";
                    break;
                case 5:
                    weatherName = "SUNNY_SF";
                    break;
                case 6:
                    weatherName = "EXTRASUNNY_SF";
                    break;
                case 7:
                    weatherName = "CLOUDY_SF";
                    break;
                case 8:
                    weatherName = "RAINY_SF";
                    break;
                case 9:
                    weatherName = "FOGGY_SF";
                    break;
                case 10:
                    weatherName = "SUNNY_VEGAS";
                    break;
                case 11:
                    weatherName = "EXTRASUNNY_VEGAS";
                    break;
                case 12:
                    weatherName = "CLOUDY_VEGAS";
                    break;
                case 13:
                    weatherName = "EXTRASUNNY_COUNTRYSIDE";
                    break;
                case 14:
                    weatherName = "SUNNY_COUNTRYSIDE";
                    break;
                case 15:
                    weatherName = "CLOUDY_COUNTRYSIDE";
                    break;
                case 16:
                    weatherName = "RAINY_COUNTRYSIDE";
                    break;
                case 17:
                    weatherName = "EXTRASUNNY_DESERT";
                    break;
                case 18:
                    weatherName = "SUNNY_DESERT";
                    break;
                case 19:
                    weatherName = "SANDSTORM_DESERT";
                    break;
                case 20:
                    weatherName = "UNDERWATER";
                    break;
                case 21:
                    weatherName = "EXTRACOLOURS_1";
                    break;
                case 22:
                    weatherName = "EXTRACOLOURS_2";
                    break;
                case 33:
                    weatherName = "DARK_CLOUDY_BROWN";
                    break;
                case 34:
                    weatherName = "BLUE_PURPLE_REGULAR";
                    break;
                case 35:
                    weatherName = "DULL_BROWN";
                    break;
                case 39:
                    weatherName = "EXTREM_BRIGHT";
                    break;
                case 43:
                    weatherName = "DARK_TOXIC_CLOUDS";
                    break;
                case 44:
                    weatherName = "BLACK_WHITE_SKY";
                    break;
                case 45:
                    weatherName = "BLACK_PURPLE_SKY";
                    break;
            }

            if (weatherName == "Unknown")
            {
                if (weatherId >= 23 && weatherId <= 26)
                    weatherName = "PALE_ORANGE";
                else if (weatherId >= 27 && weatherId <= 29)
                    weatherName = "FRESH_BLUE";
                else if (weatherId >= 30 && weatherId <= 32)
                    weatherName = "DARK_CLOUDY_TEAL";
                else if (weatherId >= 36 && weatherId <= 38)
                    weatherName = "BRIGHT_FOGGY_ORANGE";
                else if (weatherId >= 40 && weatherId <= 42)
                    weatherName = "BLUE_PURPLE_CLOUDY";
            }

            return weatherName;
        }
    }
}
