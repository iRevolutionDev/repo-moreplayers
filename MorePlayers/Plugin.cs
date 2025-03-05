using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;

namespace MorePlayers
{
    [BepInPlugin("com.revolution.moreplayers", "MorePlayers", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource _logSource;
        private static ConfigEntry<int> _maxPlayers;

        private readonly Harmony _harmony = new Harmony("com.revolution.moreplayers");

        private void Awake()
        {
            _logSource = BepInEx.Logging.Logger.CreateLogSource("MorePlayers");

            _harmony.PatchAll(typeof(TryJoiningRoomPatch));
            _harmony.PatchAll(typeof(HostLobbyPatch));

            _maxPlayers = Config.Bind("Settings", "MaxPlayers", 10, "Max players in a room");

            _logSource.LogInfo("MorePlayers loaded");
        }

        [HarmonyPatch(typeof(NetworkConnect), "TryJoiningRoom")]
        public class TryJoiningRoomPatch
        {
            private static bool Prefix(ref string ___RoomName)
            {
                _logSource.LogInfo($"Trying to join room {___RoomName}");

                if (string.IsNullOrEmpty(___RoomName))
                {
                    _logSource.LogError("Room name is empty");
                    return false;
                }

                if (NetworkConnect.instance == null)
                {
                    _logSource.LogError("NetworkConnect.instance is null");
                    return false;
                }

                PhotonNetwork.JoinOrCreateRoom(___RoomName, new RoomOptions
                {
                    MaxPlayers = _maxPlayers.Value
                }, TypedLobby.Default);

                return true;
            }
        }

        [HarmonyPatch(typeof(SteamManager), "HostLobby")]
        public class HostLobbyPatch
        {
            private static bool Prefix()
            {
                HostLobbyAsync();
                return false;
            }

            private static async void HostLobbyAsync()
            {
                try
                {
                    var lobby = await SteamMatchmaking.CreateLobbyAsync(_maxPlayers.Value);
                    if (lobby == null)
                    {
                        _logSource.LogError("Failed to create lobby");
                        return;
                    }

                    lobby.Value.SetPublic();
                    lobby.Value.SetJoinable(false);
                }
                catch (Exception e)
                {
                    _logSource.LogError(e);
                }
            }
        }
    }
}