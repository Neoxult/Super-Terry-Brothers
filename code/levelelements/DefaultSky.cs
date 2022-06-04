using Sandbox;

namespace TerryBros.LevelElements
{
    /// <summary>
	/// Skybox
	/// </summary>
    [Library("stb_sky_sidescroller"), HideInEditor]
    public partial class DefaultSky : Sky
    {
        public override void Spawn()
        {
            Skyname = "materials/sky/default_sky.vmat";
        }
    }
}
