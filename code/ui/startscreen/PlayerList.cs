using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.StartScreen
{
    [UseTemplate]
    public partial class PlayerList : Panel
    {
        public Dictionary<Client, Panel> ClientPanels { get; set; } = new();

        public Panel WrapperPanel { get; set; }

        public void AddClient(Client client)
        {
            if (ClientPanels.ContainsKey(client))
            {
                return;
            }

            Panel playerWrapper = WrapperPanel.Add.Panel("player-wrapper");
            playerWrapper.Add.Label(client.Name, "name");

            ClientPanels.Add(client, playerWrapper);

            if (!Local.Client.HasPermission("kick") || Local.Client == client)
            {
                return;
            }

            playerWrapper.Add.Button("X", "kick", () =>
            {
                client.Kick();
            });
        }

        public void RemoveClient(Client client)
        {
            if (!ClientPanels.TryGetValue(client, out Panel wrapperPanel))
            {
                return;
            }

            wrapperPanel.Delete();
            ClientPanels.Remove(client);
        }

        [Event("OnClientConnected")]
        public void OnClientConnected(Client client)
        {
            AddClient(client);
        }

        [Event("OnClientDisconnected")]
        public void OnClientDisconnected(Client client, NetworkDisconnectionReason _)
        {
            RemoveClient(client);
        }
    }
}
