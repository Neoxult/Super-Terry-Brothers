using System.Collections.Generic;
using System.ComponentModel;

using Sandbox;

namespace TerryBros.LevelElements
{
    [Library("block"), AutoGenerate, Hammer.Skip]
    public class BlockAsset : Asset
    {
        public static IReadOnlyList<BlockAsset> All => _all;
        internal static List<BlockAsset> _all = new();

        [Property, Category("Model"), ResourceType("png")]
        public string ImagePath { get; set; }

        [Property, Category("Model"), ResourceType("vmdl")]
        public string ModelPath { get; set; }

        [Property, Category("Data")]
        public Vector3 BlockSize { get; set; } = new(1, 1, 1);

        public enum Categories
        {
            Block,
            Enemy,
            Obstacle,
            Trap,
            Goal,
            CheckPoint
        }

        [Property, Category("Category")]
        public Categories Category { get; set; } = Categories.Block;

        protected override void PostLoad()
        {
            base.PostLoad();

            if (!_all.Contains(this))
            {
                _all.Add(this);
            }
        }

        public static BlockAsset GetByName(string name)
        {
            foreach (BlockAsset asset in All)
            {
                if (name == asset.Name)
                {
                    return asset;
                }
            }

            return null;
        }

        public static bool Contains(string name) => GetByName(name) != null;
    }
}
