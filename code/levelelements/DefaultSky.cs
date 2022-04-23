using Sandbox;
namespace TerryBros.LevelElements
{
    /// <summary>
	/// Skybox
	/// </summary>
    [Library("stb_sky_sidescroller"), Hammer.Skip]
    public partial class DefaultSky : Sky
    {
        public override Material FetchSkyMaterial() => Material.Load("materials/sky/default_sky.vmat");
    }
}
