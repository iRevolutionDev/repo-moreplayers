using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MorePlayers.Patches;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;

namespace MorePlayers
{
    [BepInPlugin("com.revolution.moreplayers", "MorePlayers", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;

        private readonly Harmony _harmony = new Harmony("com.revolution.moreplayers");

        private void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("MorePlayers");

            _harmony.PatchAll(typeof(TryJoiningRoomPatch));
            _harmony.PatchAll(typeof(HostLobbyPatch));
            
            MorePlayers.Config.Init(Config);
           
            Logger.LogInfo("MorePlayers loaded");
        }
    }
}