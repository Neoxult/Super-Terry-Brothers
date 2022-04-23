using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Utils;

namespace TerryBros.Levels
{
    public partial class Level
    {
        private static int _currentPacketHash = -1;
        private static int _packetCount;
        private static byte[][] _packetData;

        public static void ServerSendData(Dictionary<string, List<Vector2>> dict)
        {
            byte[] levelDataJson = Compression.Compress(dict);
            int splitLength = 150;
            int splitCount = (int) MathF.Ceiling((float) levelDataJson.Length / splitLength);

            for (int i = 0; i < splitCount; i++)
            {
                int length = Math.Clamp(levelDataJson.Length - i * splitLength, 1, splitLength);
                byte[] bytes = levelDataJson[(i * splitLength)..length];

                ServerSendPartialData(levelDataJson.GetHashCode(), i, splitCount, bytes.StringArray());
            }
        }

        [ServerCmd(Name = "stb_send_partialleveldata")]
        public static void ServerSendPartialData(int packetHash, int packetNum, int maxPackets, string partialLevelData)
        {
            if (!ConsoleSystem.Caller?.HasPermission("import") ?? true)
            {
                return;
            }

            byte[] bytes = partialLevelData.ByteArray();

            ProceedPartialData(packetHash, packetNum, maxPackets, bytes);
            ClientSendPartialData(packetHash, packetNum, maxPackets, bytes);
        }

        [ClientRpc]
        public static void ClientSendPartialData(int packetHash, int packetNum, int maxPackets, byte[] partialLevelData)
        {
            ProceedPartialData(packetHash, packetNum, maxPackets, partialLevelData);
        }

        public static void ProceedPartialData(int packetHash, int packetNum, int maxPackets, byte[] partialLevelData)
        {
            if (_currentPacketHash != packetHash)
            {
                _packetCount = 0;
                _packetData = new byte[maxPackets][];

                _currentPacketHash = packetHash;
            }

            _packetData[packetNum] = partialLevelData;
            _packetCount++;

            if (_packetCount == maxPackets)
            {
                _currentPacketHash = -1;

                STBGame.CurrentLevel?.Clear();

                Level level = new();
                level.Import(Compression.Decompress<Dictionary<string, List<Vector2>>>(CombineByteArrays(_packetData.ToArray())));

                if (Host.IsServer)
                {
                    level.Restart();
                    STBGame.ClientRestartLevel();
                }

                STBGame.CurrentLevel = level;
            }
        }

        private static byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] combinedArray = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;

            foreach (byte[] array in arrays)
            {
                array.CopyTo(combinedArray, offset);

                offset += array.Length;
            }

            return combinedArray;
        }

        [ServerCmd(Name = "stb_clear")]
        public static void ServerClear()
        {
            STBGame.CurrentLevel?.Clear();
            STBGame.CurrentLevel = null;

            ClientClear();
        }

        [ClientRpc]
        public static void ClientClear()
        {
            STBGame.CurrentLevel?.Clear();
            STBGame.CurrentLevel = null;
        }
    }
}
