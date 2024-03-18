#pragma warning disable IDE0051 // Remove unused private members
using AdminToys;
using MapGeneration;
using Mirror;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Events
{
	public class SurfaceLightingFix
	{
		[PluginEvent(ServerEventType.MapGenerated)]
		public void SpawnLights()
		{
			GameObject obj = new GameObject();
			obj.AddComponent<SurfaceLightObject>();
			NetworkServer.Spawn(obj);
		}

		public class SurfaceLightObject : MonoBehaviour
		{
			private RoomLightController controller;
			private LightSourceToy surfaceLight;
			private float fadeTimer = 0f;
			private const float fadeDuration = 2f;
			private const float lightIntensity = 50f;


			private void Start()
			{
				controller = RoomLightController.Instances.First(x => x.Room.Name == RoomName.Outside);

				var lightGO = GameObject.Instantiate(NetworkClient.prefabs.First(r => r.Value.name == "LightSourceToy").Value);
				lightGO.transform.position = new Vector3(135, 1024, -43);
				NetworkServer.Spawn(lightGO);
				surfaceLight = lightGO.GetComponent<LightSourceToy>();

				surfaceLight.NetworkLightIntensity = lightIntensity;
				surfaceLight.NetworkLightRange = 250;
				surfaceLight.NetworkLightColor = Color.white;
				surfaceLight.NetworkLightShadows = false;
			}


			private void Update()
			{
				if (NetworkServer.active)
				{
					float targetIntensity = controller.LightsEnabled ? lightIntensity : 0f;

					fadeTimer += Time.deltaTime;

					float currentIntensity = Mathf.Lerp(surfaceLight.NetworkLightIntensity, targetIntensity, fadeTimer / fadeDuration);
					surfaceLight.NetworkLightIntensity = currentIntensity;

					if (currentIntensity == targetIntensity)
					{
						fadeTimer = 0f;
					}

					surfaceLight.NetworkLightColor = AlphaWarheadController.InProgress ? Color.red : Color.white;
				}
			}
		}
	}
}

