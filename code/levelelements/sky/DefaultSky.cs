using Sandbox;
namespace TerryBros.LevelElements
{
    /// <summary>
	/// Skybox
	/// </summary>
    [Library("env_sky_sidescroller")]
    public partial class DefaultSky : Sky
    {
        public override Material FetchSkyMaterial() => Material.Load("materials/sky/default_sky.vmat");
    }
}
