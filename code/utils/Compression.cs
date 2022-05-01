// Compression code by kurozael
// https://github.com/kurozael/sbox-gamelib/blob/main/utility/Compression.cs

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;

namespace TerryBros.Utils
{
	public static class Compression
	{
		public static byte[] Compress<T>(T data)
		{
			using MemoryStream compressStream = new();
			using DeflateStream deflateStream = new(compressStream, CompressionMode.Compress);

			byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(data);

			deflateStream.Write(serialized);
			deflateStream.Close();

			return compressStream.ToArray();
		}

		public static T Decompress<T>(byte[] bytes)
		{
			using MemoryStream uncompressStream = new();

			using (MemoryStream compressStream = new(bytes))
			{
				using DeflateStream deflateStream = new(compressStream, CompressionMode.Decompress);
				deflateStream.CopyTo(uncompressStream);
			}

			return JsonSerializer.Deserialize<T>(uncompressStream.ToArray());
		}

        public static byte[] CombineByteArrays(params byte[][] arrays)
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
	}
}
