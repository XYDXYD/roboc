using Simulation.Hardware.Weapons;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class CubeRaycaster : MonoBehaviour
	{
		private int _rcLayer;

		[Inject]
		internal CubeRaycastInfo raycastInfo
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		public CubeRaycaster()
			: this()
		{
		}

		private void Awake()
		{
			_rcLayer = ((1 << GameLayers.BUILD_COLLISION) | (1 << GameLayers.CUBE_FLOOR_LAYER) | (1 << GameLayers.LOCAL_PLAYER_CUBES) | (1 << GameLayers.BUILDCOLLISION_UNVERIFIED));
		}

		private void FixedUpdate()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() * 0.5f, (float)Screen.get_height() * 0.5f, 0f);
			Ray val2 = Camera.get_main().ScreenPointToRay(val);
			InstantiatedCube hitCube = null;
			RaycastHit hit = default(RaycastHit);
			if (Physics.Raycast(val2, ref hit, 20f, _rcLayer) && hit.get_rigidbody() != null)
			{
				MachineCell cellAt = machineMap.GetCellAt(GridScaleUtility.WorldToGrid(hit.get_rigidbody().get_position(), TargetType.Player));
				if (cellAt != null)
				{
					hitCube = cellAt.info;
				}
			}
			raycastInfo.UpdateInfo(hit, hitCube);
		}
	}
}
