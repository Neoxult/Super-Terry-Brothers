using Sandbox;

namespace TerryBros.Utils
{
    public class Debug
    {
        public static void LogServerOrClient()
        {
            Log.Info($"{(Host.IsServer ? "Server" : Host.IsClient? "Client" : "---")} is executing this code.");
        }
    }
}
