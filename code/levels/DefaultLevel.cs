using Sandbox;

using TerryBros.LevelElements;
using TerryBros.Settings;

namespace TerryBros.Levels
{
    public partial class DefaultLevel : Level
    {
        public DefaultLevel() : base()
        {
            RestartSpawn = new STBSpawn(new Vector3(0, 0, 40));

            Create<DefaultSky>();

            EnvironmentLightEntity light = Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(-1, 1, -4), GlobalSettings.UpwardDir);
            light.Brightness = 2f;

            light = Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(1, 0.5f, -1), GlobalSettings.UpwardDir);
            light.Brightness = 2f;
        }

        public override void Build()
        {
            //CreateBox<Brick>(0, 0);
            CreateWallFromTo<Brick>(-30, -2, -10, 0);
            CreateWallFromTo<Brick>(-6, -2, 20, 0);
            CreateWallFromTo<Brick>(24, -2, 40, 0);
            CreateStair<LogTop>(5, 1, 14, true);
            CreateStair<Brick>(19, 1, 13, false);
            CreateWall<Brick>(-30, 1, 2, 3);
            CreateWall<Brick>(-30, 9, 2, 4);
            CreateWall<Brick>(-29, 1, 2, 3);
            CreateWall<Brick>(-29, 9, 2, 4);
            CreateCheckPoint(-12, 1);
            CreateGoal(-20, 1);
        }
    }
}
