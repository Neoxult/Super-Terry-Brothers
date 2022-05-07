using System.Collections.Generic;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Utils;

namespace TerryBros.Levels
{
    public partial class Level
    {
        [ConVar.ClientData("mapdata")]
        public static string MapData { get; set; } = null;

        public static void SyncData(Dictionary<string, string> dict)
        {
            Host.AssertClient();

            MapData = Compression.Compress(dict).StringArray();
        }

        [Event.Tick.Server]
        public static void ServerSendData()
        {
            Host.AssertServer();

            foreach (Client client in Client.All)
            {
                if (!client.HasPermission("import"))
                {
                    continue;
                }

                string mapData = client.GetClientData("mapdata");

                if (string.IsNullOrWhiteSpace(mapData))
                {
                    continue;
                }

                byte[] bytes = mapData.ByteArray();

                if (bytes == null || bytes.Length == 0 || mapData == STBGame.CurrentLevel?.Data)
                {
                    continue;
                }

                ClientResetMapData(To.Single(client));

                ProceedData(bytes);
                ClientSendData(bytes);

                break;
            }
        }

        [ClientRpc]
        public static void ClientResetMapData()
        {
            MapData = null;
        }

        [ClientRpc]
        public static void ClientSendData(byte[] mapData)
        {
            ProceedData(mapData);
        }

        public static void ProceedData(byte[] mapData)
        {
            STBGame.CurrentLevel?.Clear();

            new Level().Import(mapData.StringArray());
        }
    }
}
