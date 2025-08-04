using System;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;

namespace MorePlayers.Patches
{
    [HarmonyPatch(typeof(NetworkConnect), "TryJoiningRoom")]
    public class TryJoiningRoomPatch
    {
        private static bool Prefix(ref string ___RoomName)
        {
            Plugin.Logger.LogInfo($"Trying to join room {___RoomName}");

            if (string.IsNullOrEmpty(___RoomName))
            {
                Plugin.Logger.LogError("Room name is empty");
                return false;
            }

            PhotonNetwork.JoinOrCreateRoom(___RoomName, new RoomOptions
            {
                MaxPlayers = Config.MaxPlayers.Value
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
                var lobby = await SteamMatchmaking.CreateLobbyAsync(Config.MaxPlayers.Value);
                if (lobby == null)
                {
                    Plugin.Logger.LogError("Failed to create lobby");
                    return;
                }

                lobby.Value.SetPublic();
                lobby.Value.SetJoinable(false);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }
    }
}