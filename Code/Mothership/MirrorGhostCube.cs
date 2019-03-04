using Simulation.Hardware.Weapons;
using System.Globalization;
using UnityEngine;

namespace Mothership
{
	internal class MirrorGhostCube : GhostCube
	{
		private int _initialRotation;

		private int _initialZOffset;

		protected override void CreateGhostCube(CubeTypeID cube)
		{
			cube = GetMirrorCubeId(cube);
			DestroyCube();
			_lastCube = cube;
			BuildCube(cube);
			_initialRotation = base.cubeList.CubeTypeDataOf(cube).cubeData.initialMirrorRotation;
			_initialZOffset = base.cubeList.CubeTypeDataOf(cube).cubeData.initialMirrorZOffset;
			ShowGhostCube(this.get_gameObject().get_activeSelf());
		}

		protected override void Initialize()
		{
			base.mirrorMode.OnMirrorLineMoved += HandleOnMirrorLineMoved;
			base.cubeCaster.requiresMirrorMode = true;
			base.mirrorMode.CurrentLinePosition(out _centreLineOffset, out _fullLineWidthOffset);
		}

		private CubeTypeID GetMirrorCubeId(CubeTypeID cube)
		{
			PersistentCubeData cubeData = base.cubeList.CubeTypeDataOf(cube).cubeData;
			string mirrorCubeId = cubeData.mirrorCubeId;
			if (uint.TryParse(mirrorCubeId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result))
			{
				return result;
			}
			return cube;
		}

		protected override void ComputeGridPos(RaycastHit hit)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			base.ComputeGridPos(hit);
			base.hitGridPos = GetMirrorGridPos(base.hitGridPos);
			base.adjacentGridPos = GetMirrorGridPos(base.adjacentGridPos);
		}

		protected override void UpdateGhostInfo()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			RaycastHit hit = base.raycastInfo.hit;
			bool flag = IsCentreGridPosition(base.hitGridPos);
			bool flag2 = CubeNormalIsAxisAlligned(base.raycastInfo.hit);
			bool flag3 = base.machineMap.IsCellTaken(base.hitGridPos);
			bool flag4 = base.machineMap.IsPosValid(base.hitGridPos);
			Vector3 val = hit.get_normal();
			val.x = 0f - val.x;
			float groundPlaceHitNormalZ_Rotation = base.cubeList.CubeTypeDataOf(_lastCube).cubeData.groundPlaceHitNormalZ_Rotation;
			if (base.raycastInfo.hitCube == null && groundPlaceHitNormalZ_Rotation != 0f)
			{
				Int3 @int = base.machineMap.FindGridLocFromHit(base.raycastInfo.hit, 1);
				val = Quaternion.Euler(0f, 0f, groundPlaceHitNormalZ_Rotation) * val;
				if (groundPlaceHitNormalZ_Rotation <= 90f && @int.x > GetCentreLine())
				{
					val = -val;
				}
			}
			bool flag5 = base.machineBuilder.CanPlaceCubeOnHitSide(val, base.selectedCube.selectedCubeID);
			RaycastHit hit2 = base.raycastInfo.hit;
			_canShow = (hit2.get_collider() != null && flag2 && !flag3 && !flag && flag4 && flag5);
			bool flag6 = base.machineBuilder.CheckIfCanPlaceCube(base.hitGridPos, base.cubeCaster);
			base.cubeCaster.UpdateCaster(flag6 && _canShow, flag2, !flag4, flag5);
			base.adjacentCube = null;
			if (base.raycastInfo.hitCube != null && !IsCentreGridPosition((Int3)base.raycastInfo.hitCube.gridPos))
			{
				MachineCell cellAt = base.machineMap.GetCellAt(GetMirrorGridPos((Int3)base.raycastInfo.hitCube.gridPos));
				if (cellAt != null)
				{
					base.adjacentCube = cellAt.info;
				}
			}
		}

		protected override void SaveGhostState()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			base.rotation = -base.selectedCube.currentRotation + _initialRotation;
			Vector3 forward = Vector3.get_up();
			RaycastHit hit = base.raycastInfo.hit;
			base.cubeUp = hit.get_normal();
			PersistentCubeData cubeData = base.cubeList.CubeTypeDataOf(_lastCube).cubeData;
			Vector3 groundPlacePositionOffset = cubeData.groundPlacePositionOffset;
			if (base.raycastInfo.hitCube == null)
			{
				float groundPlaceHitNormalZ_Rotation = cubeData.groundPlaceHitNormalZ_Rotation;
				Int3 @int = base.machineMap.FindGridLocFromHit(base.raycastInfo.hit, 1);
				base.cubeUp = Quaternion.Euler(Vector3.get_forward() * groundPlaceHitNormalZ_Rotation) * base.cubeUp;
				if (groundPlaceHitNormalZ_Rotation == 90f && @int.x > GetCentreLine())
				{
					base.cubeUp = -base.cubeUp;
				}
			}
			if (Mathf.Abs(Vector3.Dot(base.cubeUp, Vector3.get_right())) > 0.5f)
			{
				base.cubeUp = -base.cubeUp;
			}
			Vector3 up = base.cubeUp;
			Vector3 val = CalculateOrientationAndOffset(ref forward, ref up, cubeData);
			float num = _initialZOffset;
			Vector3 cubeUp = base.cubeUp;
			Int3 int2 = new Int3(0f, 0f, num * cubeUp.x);
			base.hitGridPos += int2;
			base.adjacentGridPos += int2;
			_ghostPosition = GridScaleUtility.GridToWorld(base.hitGridPos, TargetType.Player);
			_ghostOrientation = Quaternion.LookRotation(forward, up);
			if (base.raycastInfo.hitCube == null)
			{
				Int3 hitGridPos = base.hitGridPos;
				if (hitGridPos.x > GetCentreLine())
				{
					groundPlacePositionOffset.x = 0f - groundPlacePositionOffset.x;
				}
				else
				{
					float z = groundPlacePositionOffset.z;
					float num2 = _initialZOffset;
					Vector3 cubeUp2 = base.cubeUp;
					groundPlacePositionOffset.z = z - num2 * cubeUp2.x;
				}
				_ghostPosition += groundPlacePositionOffset * 0.2f;
			}
			ref Vector3 ghostPosition = ref _ghostPosition;
			ghostPosition.x -= val.y * 0.2f;
			ref Vector3 ghostPosition2 = ref _ghostPosition;
			float y = ghostPosition2.y;
			float num3 = val.x * 0.2f;
			Vector3 cubeUp3 = base.cubeUp;
			ghostPosition2.y = y + num3 * cubeUp3.y;
			ref Vector3 ghostPosition3 = ref _ghostPosition;
			ghostPosition3.z -= val.z * 0.2f;
			base.finalGridPos = GridScaleUtility.WorldToGrid(_ghostPosition, TargetType.Player);
		}
	}
}
