using Mirror;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Features.Map.SurfaceLightFix
{
	public class LightFixEvents
	{
		[PluginEvent()]
		public void SpawnLights(MapGeneratedEvent ev)
		{
			GameObject obj = new GameObject();
			obj.AddComponent<SurfaceLightObject>();
			NetworkServer.Spawn(obj);
		}
	}
}
