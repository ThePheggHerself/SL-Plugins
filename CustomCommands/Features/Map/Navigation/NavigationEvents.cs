using CustomCommands.Features.Map.Navigation.NavMeshComponents;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace CustomCommands.Features.Map.Navigation
{
	public class NavigationEvents
	{
		[PluginEvent]
		public void MapGeneratedEvent(MapGeneratedEvent ev)
		{
			var rooms = GameObject.Find("LightRooms");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.voxelSize = 0.08f;
				meshSurface.BuildNavMesh();
			}

			rooms = GameObject.Find("HeavyRooms");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.voxelSize = 0.08f;
				meshSurface.BuildNavMesh();
			}

			rooms = GameObject.Find("EntranceRooms");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.voxelSize = 0.08f;
				meshSurface.BuildNavMesh();
			}

			rooms = GameObject.Find("Outside");
			if (rooms != null)
			{
				var meshSurface = rooms.AddComponent<NavMeshSurface>();
				meshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
				meshSurface.voxelSize = 0.08f;
				meshSurface.BuildNavMesh();
			}
		}
	}
}
