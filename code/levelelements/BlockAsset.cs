using System.Collections.Generic;
using System.ComponentModel;

using Sandbox;

namespace TerryBros.LevelElements
{
    [Library("block"), AutoGenerate, Hammer.Skip]
    public class BlockAsset : GameResource
    {
        public static IReadOnlyList<BlockAsset> All => _all;
        internal static List<BlockAsset> _all = new();

        [Property, Category("Display"), ResourceType("png")]
        public string IconPath { get; set; }

        [Property, Category("Model"), ResourceType("vmdl")]
        public string ModelPath { get; set; }

        [Property, Category("Model"), ResourceType("vmat")]
        public string MaterialPath { get; set; }

        [Property, Category("Model"), ResourceType("png")]
        public string TexturePath { get; set; }

        [Property, Category("Data")]
        public Vector3 BlockSize { get; set; } = new(1, 1, 1);

        public Material Material
        {
            get
            {
                if (_material == null)
                {
                    _material = Material.Load(MaterialPath).CreateCopy();
                    _material.OverrideTexture("Color", Texture);
                }

                return _material;
            }
        }
        private Material _material;

        public Texture Texture
        {
            get
            {
                if (_texture == null)
                {
                    _texture = Texture.Find(TexturePath);
                    Material.OverrideTexture("Color", _texture);
                }

                return _texture;
            }
        }
        private Texture _texture;

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

            Precache.Add(MaterialPath);
            Precache.Add(ModelPath);
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
