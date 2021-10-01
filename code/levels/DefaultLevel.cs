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
            /*RestartSpawn = new STBSpawn()
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(0, 1)
            };*/

            //Create<DefaultSky>();

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

            //Floor
            new LeavesBlooming()
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(-20, 10),
            };
            new LeavesBlooming()
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(20, 10)
            };
            new LeavesBlooming()
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(-20, -10)
            };
            new LeavesBlooming()
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(0, 0)
            };
            //await GameTask.DelaySeconds(1f);
            for (int x = -100; x <= 100; x++)
            {
                for (int y = -3; y <= 0; y++)
                {
                    new LeavesBlooming()
                    {
                        Position = GlobalSettings.GetBlockPosForGridCoordinates(x, y)
                    };
                }
                //await GameTask.DelaySeconds(0.05f);
            }


        }
    }
}
