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

        private readonly static int _splitLength = 510 - "ServerSendPartialData".Length - 3 * (int.MaxValue.ToString().Length + 4) - 3; // 510 max length including cmd name length + 3 extra chars per argument // -int => +1

        public static void ServerSendData(Dictionary<string, List<Vector2>> dict)
        {
            byte[] levelDataJson = Compression.Compress(dict);
            string stringArray = levelDataJson.StringArray();
            int stringArrayLength = stringArray.Length;
            int splitCount = (int) MathF.Ceiling((float) stringArrayLength / _splitLength);

            for (int i = 0, length = 0; i < splitCount; i++)
            {
                length += Math.Clamp(stringArrayLength - i * _splitLength, 1, _splitLength);

                ServerSendPartialData(levelDataJson.GetHashCode(), i, splitCount, stringArray[(i * _splitLength)..length]);
            }
        }

        [ServerCmd]
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

                new Level().Import(Compression.Decompress<Dictionary<string, List<Vector2>>>(Compression.CombineByteArrays(_packetData.ToArray())));
            }
        }
    }
}
