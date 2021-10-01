using Sandbox;

using TerryBros.LevelElements;
using TerryBros.Settings;

namespace TerryBros.Levels
{
    public partial class DefaultLevel : Level
    {
        public DefaultLevel() : base()
        {
        }

        public override void Build()
        {
            RestartSpawn = new STBSpawn
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(0, 1)
            };

            Create<DefaultSky>();

            //TODO: Properly set lights up in local space
            EnvironmentLightEntity light = Create<EnvironmentLightEntity>();
            //light.Position = GlobalSettings.ConvertLocalToGlobalCoordinates(new Vector3(1, 4, -1));
            //light.Rotation = Rotation.LookAt(GlobalSettings.GroundPos - light.Position, GlobalSettings.UpwardDir);
            light.Rotation = Rotation.LookAt(new Vector3(-1, 1, -4), GlobalSettings.UpwardDir);
            light.Brightness = 2f;

            light = Create<EnvironmentLightEntity>();
            //light.Position = GlobalSettings.ConvertLocalToGlobalCoordinates(new Vector3(-1, 1, -0.5f));
            //light.Rotation = Rotation.LookAt(GlobalSettings.GroundPos - light.Position, GlobalSettings.UpwardDir);
            light.Rotation = Rotation.LookAt(new Vector3(1, 0.5f, -1), GlobalSettings.UpwardDir);
            light.Brightness = 2f;

            //CreateBox<Brick>(0, -3);
            //Floor
            CreateWallFromTo<Brick>(-10, -2, 10, 0);
            CreateWallFromTo<Brick>(14, -2, 29, 0);
            CreateWallFromTo<Brick>(33, -2, 50, 0);
            CreateWallFromTo<Brick>(54, -2, 60, 0);

            //Platform
            CreateWall<Brick>(4, 3, 3, 1);
            CreateWall<Brick>(5, 4, 1, 2);

            CreateStair<LogTop>(20, 1, 4);
            CreateStair<Brick>(24, 1, 3, false);

            //StartWall
            CreateWall<Brick>(-10, 1, 2, 3);
            CreateWall<Brick>(-10, 9, 2, 4);

            //EndWall
            CreateWall<Brick>(59, 1, 2, 3);
            CreateWall<Brick>(59, 9, 2, 4);

            CreateCheckPoint(23, 5);
            CreateCheckPoint(35, 1);
            CreateCheckPoint(48, 1);
            CreateGoal(55, 1);
        }
    }
}
