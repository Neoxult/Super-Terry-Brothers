using Sandbox;
namespace TerryBros.Objects
{
    /// <summary>
	/// Skybox
	/// </summary>
    [Library("env_sky_sidescroller")]
    public partial class defaultSky : Sky
    {
        public override Material FetchSkyMaterial()
        {
            return Material.Load("materials/sky/default_sky.vmat");
        }
    }
}
