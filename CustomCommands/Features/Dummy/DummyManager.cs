using MEC;
using Mirror;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Features.Dummy
{
	public static class DummyManager
	{
		public static int DummyID = 0;

		public static ReferenceHub CreateDummy(string Name)
		{
			var dummy = GameObject.Instantiate(NetworkManager.singleton.playerPrefab);
			DummyID++;
			int id = DummyID;

			var dcon = new DummyConnection(id);
			var hub = dummy.GetComponent<ReferenceHub>();

			NetworkServer.AddPlayerForConnection(dcon, dummy);

			try
			{
				hub.authManager.NetworkSyncedUserId = $"dummy{id}@server";
				hub.nicknameSync.Network_myNickSync = Name;
				hub.authManager.UserId = $"dummy{id}@server";
			}
			catch (Exception)
			{

			}

			return hub;
		}

		public static bool DestroyDummy(string ID)
		{
			foreach (var plr in Player.GetPlayers())
			{
				if (plr.UserId == ID)
				{
					plr.Kill();

					Timing.CallDelayed(0.2f, () =>
					{
						NetworkServer.RemovePlayerForConnection(plr.ReferenceHub.connectionToClient, true);
					});

					return true;
				}
			}

			return false;
		}
	}
}
