using BepInEx.Configuration;

namespace MorePlayers
{
    public static class Config
    {
        public static ConfigEntry<int> MaxPlayers;
        
        public static void Init(ConfigFile config)
        {
            MaxPlayers = config.Bind("Settings", "MaxPlayers", 10, "Max players in a room");
        }
    }
}