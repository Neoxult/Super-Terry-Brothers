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

        [Property, Category("Display"), ResourceType("png")]
        public string IconPath { get; set; }

        [Property, Category("Model"), ResourceType("vmdl")]
        public string ModelPath { get; set; }

        private string _material_path;
        [Property, Category("Model"), ResourceType("vmat")]
        public string MaterialPath {
            get => _material_path;
            set
            {
                _material_path = value;
                Mat = Material.Load(_material_path).CreateCopy();
                Mat.OverrideTexture("Color", Tex);
            }
        }

        private string _texture_path;
        [Property, Category("Model"), ResourceType("png")]
        public string TexturePath {
            get => _texture_path;
            set
            {
                _texture_path = value;
                Tex = Texture.Find(_texture_path);
                Mat.OverrideTexture("Color", Tex);
            }
        }

        [Property, Category("Data")]
        public Vector3 BlockSize { get; set; } = new(1, 1, 1);
        
        public Material Mat { get; private set; }
        public Texture Tex { get; private set; }

        public enum Categories
        {
            Block,
            Enemy,
            Obstacle,
            Trap,
            Goal,
            CheckPoint
        }

        [Property, Category("Data")]
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
