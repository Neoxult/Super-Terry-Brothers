using System.Text;
using System;
using System.Collections.Generic;


/*
TODO
-   add xShift and yShift to have uint only in compressed data
*/

namespace TerryBros.Levels
{
    public partial class Level
    {
        public class CompressedData
        {
            public Vector2 Position { get; set; }

            public Vector2 Size { get; set; }

            public CompressedData(Vector2 position, Vector2 size)
            {
                Position = position;
                Size = size;
            }

            public CompressedData(int x, int y, int w, int h) : this(new(x, y), new(w, h)) { }

            public CompressedData(Vector2 position, int w, int h) : this(position, new(w, h)) { }

            public string Export()
            {
                string str = $"{Position.x},{Position.y}";

                if (Size.x != 1 || Size.y != 1)
                {
                    str += ',';

                    if (Size.x != 1)
                    {
                        str += Size.x;
                    }

                    if (Size.y != 1)
                    {
                        str += ',';
                        str += Size.y;
                    }
                }

                return str;
            }

            public static CompressedData Import(string str)
            {
                string[] splits = str.Split(',');

                Vector2 position = new(int.Parse(splits[0]), int.Parse(splits[1]));

                int width = 1;

                if (splits.Length > 2 && !string.IsNullOrEmpty(splits[2]))
                {
                    width = int.Parse(splits[2]);
                }

                int height = 1;

                if (splits.Length > 3 && !string.IsNullOrEmpty(splits[3]))
                {
                    height = int.Parse(splits[3]);
                }

                return new(position, width, height);
            }

            public static List<CompressedData> Decompress(string str)
            {
                List<CompressedData> list = new();

                string[] splits = str.Split('|');

                for (int i = 0; i < splits.Length; i++)
                {
                    list.Add(Import(splits[i]));
                }

                return list;
            }

            public static string Compress(List<CompressedData> compressedList)
            {
                StringBuilder stringBuilder = new();

                foreach (CompressedData compressedData in compressedList)
                {
                    stringBuilder.Append(compressedData.Export());
                    stringBuilder.Append('|');
                }

                stringBuilder.Remove(stringBuilder.Length - 1, 1);

                return stringBuilder.ToString();
            }
        }

        public static List<Vector3> Decompress(string compressedString)
        {
            List<Vector3> list = new();

            foreach (CompressedData compressedData in CompressedData.Decompress(compressedString))
            {
                for (int x = 0; x < (int) compressedData.Size.x; x++)
                {
                    int xPos = (int) compressedData.Position.x + x;

                    for (int y = 0; y < (int) compressedData.Size.y; y++)
                    {
                        list.Add(new(xPos, 0, (int) compressedData.Position.y + y));
                    }
                }
            }

            return list;
        }

        public static Dictionary<string, List<Vector3>> Decompress(Dictionary<string, string> compressedDict)
        {
            Dictionary<string, List<Vector3>> dict = new();

            foreach (KeyValuePair<string, string> keyValuePair in compressedDict)
            {
                dict.Add(keyValuePair.Key, Decompress(keyValuePair.Value));
            }

            return dict;
        }

        public static Dictionary<string, string> Compress(Dictionary<string, List<Vector2>> dict)
        {
            Dictionary<string, string> compressedDict = new();

            foreach (KeyValuePair<string, List<Vector2>> keyValuePair in dict)
            {
                List<Vector2> vector2List = new(keyValuePair.Value);
                List<CompressedData> compressedList = new();

                // transform coord system
                List<int> xCoords = new();
                List<int> yCoords = new();

                foreach (Vector2 v2 in vector2List)
                {
                    if (!xCoords.Contains((int) v2.x))
                    {
                        xCoords.Add((int) v2.x);
                    }

                    if (!yCoords.Contains((int) v2.y))
                    {
                        yCoords.Add((int) v2.y);
                    }
                }

                xCoords.Sort();
                yCoords.Sort();

                int xShift = -Math.Min(0, xCoords[0]);
                int yShift = -Math.Min(0, yCoords[0]);

                Vector2 shiftVector2 = new(xShift, yShift);

                bool[,] coords = new bool[xCoords[^1] + 1 + xShift, yCoords[^1] + 1 + yShift];

                foreach (int x in xCoords)
                {
                    foreach (int y in yCoords)
                    {
                        coords[x + xShift, y + yShift] = false;
                    }
                }

                foreach (Vector2 v2 in vector2List)
                {
                    coords[(int) v2.x + xShift, (int) v2.y + yShift] = true;
                }

                while (vector2List.Count > 0)
                {
                    List<Vector2> blocks = new()
                    {
                        vector2List[0]
                    };
                    int width = 1;
                    int height = 1;

                    if (vector2List.Count > 1)
                    {
                        // find greatest blocks quad
                        int startIndex = 0;

                        for (int i = 0; i < vector2List.Count; i++)
                        {
                            Vector2 v2 = vector2List[i] + shiftVector2;
                            List<Vector2> greatestBlock = SearchGreatestBlock(coords, (int) v2.x, (int) v2.y, out int w, out int h);

                            if (width * height < w * h)
                            {
                                startIndex = i;
                                width = w;
                                height = h;
                            }
                        }

                        Vector2 finalVector2 = vector2List[startIndex] + shiftVector2;

                        blocks = SearchGreatestBlock(coords, (int) finalVector2.x, (int) finalVector2.y, out int _, out int _);

                        for (int i = 0; i < blocks.Count; i++)
                        {
                            blocks[i] -= shiftVector2;
                        }
                    }

                    List<Vector2> removeList = new();

                    foreach (Vector2 v2 in blocks)
                    {
                        coords[(int) v2.x + xShift, (int) v2.y + yShift] = false;

                        foreach (Vector2 originalVector2 in vector2List)
                        {
                            if (originalVector2.x == v2.x && originalVector2.y == v2.y)
                            {
                                removeList.Add(originalVector2);

                                break;
                            }
                        }
                    }

                    compressedList.Add(new(blocks[0], width, height));

                    foreach (Vector2 removeVector2 in removeList)
                    {
                        vector2List.Remove(removeVector2);
                    }
                }

                compressedDict.Add(keyValuePair.Key, CompressedData.Compress(compressedList));
            }

            return compressedDict;
        }

        private static List<Vector2> SearchGreatestBlock(bool[,] coords, int x, int y, out int width, out int height)
        {
            List<Vector2> vector2s = new();

            width = 1;
            height = 1;

            Vector2 target = new(x, y);

            int maxY = coords.GetUpperBound(1);

            for (int loopX = x; loopX <= coords.GetUpperBound(0); loopX++)
            {
                if (!coords[loopX, y])
                {
                    break;
                }

                for (int loopY = y; loopY <= coords.GetUpperBound(1); loopY++)
                {
                    if (!coords[loopX, loopY])
                    {
                        if (loopY - 1 < maxY)
                        {
                            maxY = loopY - 1;
                        }

                        break;
                    }

                    if (loopY > maxY)
                    {
                        break;
                    }

                    int currentWidth = loopX - x + 1;
                    int currentHeight = loopY - y + 1;

                    if (currentWidth * currentHeight > width * height)
                    {
                        width = currentWidth;
                        height = currentHeight;
                        target = new(loopX, loopY);
                    }
                }
            }

            for (int loopX = x; loopX <= (int) target.x; loopX++)
            {
                for (int loopY = y; loopY <= (int) target.y; loopY++)
                {
                    vector2s.Add(new(loopX, loopY));
                }
            }

            return vector2s;
        }
    }
}
