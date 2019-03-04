using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class GhostCube : MonoBehaviour
	{
		public Material transMaterial;

		private GhostCubeColorUpdater _cubeColorUpdater;

		private Int3 _previousKnownGridPos = Int3.zero;

		private bool _initialized;

		private bool _redifyCube = true;

		private float _cubeScale = 1f;

		private int _positionOrOrientationChanging;

		private int _previousKnownRotation;

		protected CubeTypeID _lastCube = CubeTypeID.StandardCubeID;

		protected GameObject _goCube;

		protected GhostCubeCollisionCheck _colliderCheck;

		protected Quaternion _ghostOrientation;

		protected Vector3 _ghostPosition;

		protected bool _canShow;

		protected int _centreLineOffset;

		protected int _fullLineWidthOffset;

		[Inject]
		internal ICubeHolder selectedCube
		{
			get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
		{
			get;
			set;
		}

		[Inject]
		internal IEditorCubeFactory cubeFactory
		{
			get;
			private set;
		}

		[Inject]
		internal IGUIInputControllerMothership inputController
		{
			get;
			set;
		}

		[Inject]
		internal IGhostCubeVisibilityChecker ghostCubeVisibilityChecker
		{
			get;
			set;
		}

		[Inject]
		internal IMachineBuilder machineBuilder
		{
			get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			get;
			set;
		}

		[Inject]
		internal CubeRaycastInfo raycastInfo
		{
			get;
			set;
		}

		[Inject]
		internal GhostCubeController ghostCubeController
		{
			get;
			private set;
		}

		[Inject]
		internal MachineColorUpdater colorUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal MachineMover machineMover
		{
			private get;
			set;
		}

		[Inject]
		internal MirrorMode mirrorMode
		{
			get;
			set;
		}

		internal CubeCaster cubeCaster
		{
			get;
			set;
		}

		internal InstantiatedCube adjacentCube
		{
			get;
			set;
		}

		internal CubeTypeID currentCube
		{
			get
			{
				return _lastCube;
			}
			set
			{
				_lastCube = value;
			}
		}

		internal Int3 adjacentGridPos
		{
			get;
			set;
		}

		internal Int3 finalGridPos
		{
			get;
			set;
		}

		internal Int3 hitGridPos
		{
			get;
			set;
		}

		internal Vector3 cubeUp
		{
			get;
			set;
		}

		internal int rotation
		{
			get;
			set;
		}

		public GhostCube()
			: this()
		{
		}

		public int GetCentreLine()
		{
			Byte3 @byte = machineMap.GridSize();
			return (int)@byte.x / 2 + _centreLineOffset;
		}

		private void Awake()
		{
			cubeCaster = new CubeCaster();
		}

		private void Start()
		{
			_cubeScale = GridScaleUtility.WorldScale(1f, TargetType.Player);
			selectedCube.OnColorChanged += ChangeColor;
			selectedCube.OnCubeSelectedChanged += CreateGhostCube;
			machineBuilder.OnPlaceCube += CubeBeingPlaced;
			machineMover.OnMachineMoved += HandleOnMachineMoved;
			_lastCube = selectedCube.selectedCubeID;
			CreateGhostCube(_lastCube);
			Initialize();
			_initialized = true;
			_redifyCube = true;
			OnEnable();
		}

		public Int3 GetMirrorGridPos(Int3 cubeGridPos)
		{
			int centreLine = GetCentreLine();
			int num = centreLine - cubeGridPos.x;
			int num2 = cubeGridPos.x = centreLine + num + _fullLineWidthOffset;
			return cubeGridPos;
		}

		private void ChangeColor(PaletteColor color)
		{
			_cubeColorUpdater.ApplyColor(color);
		}

		protected virtual void Initialize()
		{
			cubeCaster.requiresMirrorMode = false;
			ghostCubeController.GhostCubeInitialized(this);
			mirrorMode.OnMirrorModeChanged += HandleOnMirrorModeChanged;
			mirrorMode.OnMirrorLineMoved += HandleOnMirrorLineMoved;
			mirrorMode.CurrentLinePosition(out _centreLineOffset, out _fullLineWidthOffset);
		}

		private void HandleOnMirrorModeChanged(bool enabled)
		{
			if (!enabled)
			{
				_colliderCheck.ResetGhost();
			}
		}

		protected virtual void HandleOnMirrorLineMoved(int centreLineOffset, int fullLineWidthOffset)
		{
			_centreLineOffset = centreLineOffset;
			_fullLineWidthOffset = fullLineWidthOffset;
		}

		private void HandleOnInfoUpdated()
		{
			adjacentCube = null;
			if (!cubeCaster.changingCube && _goCube != null)
			{
				ProcessGhostCube();
			}
		}

		private void HandleOnMachineMoved(Int3 displacement)
		{
			finalGridPos += displacement;
			hitGridPos += displacement;
			adjacentGridPos += displacement;
			_positionOrOrientationChanging = 2;
			cubeCaster.cubeIntersectionsUnreliable = true;
		}

		protected bool CubeNormalIsAxisAlligned(RaycastHit hit)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < 3; i++)
			{
				Vector3 normal = hit.get_normal();
				float num = Mathf.Abs(normal.get_Item(i));
				Vector3 point = hit.get_point();
				float num2 = Mathf.Abs(point.get_Item(i) % _cubeScale);
				if (num > 0.995f && (num2 < 0.01f || num2 > 0.19f))
				{
					Debug.DrawRay(hit.get_point(), hit.get_normal(), Color.get_green());
					return true;
				}
			}
			Debug.DrawRay(hit.get_point(), hit.get_normal(), Color.get_red());
			return false;
		}

		private void OnEnable()
		{
			if (_initialized)
			{
				ShowGhostCube(enabled: true);
				raycastInfo.OnInfoUpdated += HandleOnInfoUpdated;
			}
		}

		private void OnDisable()
		{
			if (_initialized)
			{
				ShowGhostCube(enabled: false);
				raycastInfo.OnInfoUpdated -= HandleOnInfoUpdated;
			}
		}

		protected void ShowGhostCube(bool enabled)
		{
			if (_goCube != null)
			{
				_goCube.SetActive(enabled);
			}
		}

		private void OnDestroy()
		{
			selectedCube.OnCubeSelectedChanged -= CreateGhostCube;
			machineBuilder.OnPlaceCube -= CubeBeingPlaced;
			machineMover.OnMachineMoved -= HandleOnMachineMoved;
		}

		protected virtual void CreateGhostCube(CubeTypeID cube)
		{
			DestroyCube();
			_lastCube = cube;
			BuildCube(cube);
			ghostCubeController.GhostCubeChanged(_goCube);
			cubeCaster.changingCube = true;
		}

		private void CubeBeingPlaced(InstantiatedCube cube)
		{
			cubeCaster.changingCube = true;
		}

		protected void DestroyCube()
		{
			if (_goCube != null)
			{
				Object.Destroy(_goCube);
			}
			_goCube = null;
		}

		protected void BuildCube(CubeTypeID cube)
		{
			if (_goCube == null)
			{
				_goCube = cubeFactory.BuildDummyCube(cube, onGrid: true);
				InitializeCube();
			}
		}

		private void InitializeCube()
		{
			_colliderCheck = _goCube.AddComponent<GhostCubeCollisionCheck>();
			_colliderCheck.cubeCaster = cubeCaster;
			_colliderCheck.machineMap = machineMap;
			_colliderCheck.colorUpdater = colorUpdater;
			_cubeColorUpdater = _goCube.AddComponent<GhostCubeColorUpdater>();
			_cubeColorUpdater.fadeShader = transMaterial.get_shader();
			_cubeColorUpdater.initialColor = selectedCube.currentColor;
		}

		protected bool IsCentreGridPosition(Int3 cubeGridPos)
		{
			if (_fullLineWidthOffset != 0)
			{
				return false;
			}
			int centreLine = GetCentreLine();
			return centreLine == cubeGridPos.x;
		}

		protected virtual void ComputeGridPos(RaycastHit hit)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			hitGridPos = machineMap.FindGridLocFromHit(hit, 1);
			if (hitGridPos != _previousKnownGridPos)
			{
				cubeCaster.cubeIntersectionsUnreliable = true;
				_positionOrOrientationChanging = 2;
			}
			_previousKnownGridPos = hitGridPos;
			adjacentGridPos = machineMap.FindGridLocFromHit(hit, -1);
			if (IsCentreGridPosition(hitGridPos))
			{
				_colliderCheck.ResetGhost();
			}
		}

		protected virtual void UpdateGhostInfo()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			RaycastHit hit = raycastInfo.hit;
			bool flag = machineMap.IsPosValid(this.hitGridPos);
			bool flag2 = CubeNormalIsAxisAlligned(raycastInfo.hit);
			bool flag3 = machineMap.IsCellTaken(this.hitGridPos);
			Vector3 val = hit.get_normal();
			float groundPlaceHitNormalZ_Rotation = cubeList.CubeTypeDataOf(_lastCube).cubeData.groundPlaceHitNormalZ_Rotation;
			if (raycastInfo.hitCube == null && groundPlaceHitNormalZ_Rotation != 0f)
			{
				val = Quaternion.Euler(0f, 0f, groundPlaceHitNormalZ_Rotation) * val;
				if (groundPlaceHitNormalZ_Rotation <= 90f)
				{
					Int3 hitGridPos = this.hitGridPos;
					if (hitGridPos.x > GetCentreLine())
					{
						val = -val;
					}
				}
			}
			bool flag4 = machineBuilder.CheckIfCubeSideIsValid(adjacentGridPos, val, selectedCube.selectedCubeID);
			_canShow = (hit.get_collider() != null && flag && flag2 && !flag3 && flag4 && ghostCubeVisibilityChecker.CheckVisibility(this.hitGridPos.ToVector3(), _ghostOrientation, selectedCube.selectedCubeID));
			adjacentCube = raycastInfo.hitCube;
			bool pBuilderCanPlaceCubeAtGridPos = false;
			if (flag2)
			{
				pBuilderCanPlaceCubeAtGridPos = machineBuilder.CheckIfCanPlaceCube(this.hitGridPos, cubeCaster);
			}
			cubeCaster.UpdateCaster(pBuilderCanPlaceCubeAtGridPos, flag2, !flag, flag4);
		}

		private void ProcessGhostCube()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!(_goCube != null))
			{
				return;
			}
			ComputeGridPos(raycastInfo.hit);
			SaveGhostState();
			UpdateGhostInfo();
			if (_canShow)
			{
				ShowGhostCube(enabled: true);
				if (_positionOrOrientationChanging == 0)
				{
					_redifyCube = !cubeCaster.canPlace;
				}
				_cubeColorUpdater.Redify(_redifyCube);
				UpdateGhostTransform();
			}
			else
			{
				ShowGhostCube(enabled: false);
			}
		}

		protected virtual void SaveGhostState()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			rotation = selectedCube.currentRotation;
			if (rotation != _previousKnownRotation)
			{
				cubeCaster.cubeIntersectionsUnreliable = true;
				_positionOrOrientationChanging = 2;
			}
			_previousKnownRotation = rotation;
			_ghostPosition = GridScaleUtility.GridToWorld(this.hitGridPos, TargetType.Player);
			RaycastHit hit = raycastInfo.hit;
			this.cubeUp = hit.get_normal();
			Vector3 forward = Vector3.get_up();
			PersistentCubeData cubeData = cubeList.CubeTypeDataOf(_lastCube).cubeData;
			Vector3 groundPlacePositionOffset = cubeData.groundPlacePositionOffset;
			if (raycastInfo.hitCube == null)
			{
				float groundPlaceHitNormalZ_Rotation = cubeData.groundPlaceHitNormalZ_Rotation;
				this.cubeUp = Quaternion.Euler(Vector3.get_forward() * groundPlaceHitNormalZ_Rotation) * this.cubeUp;
				if (groundPlaceHitNormalZ_Rotation == 90f)
				{
					Int3 hitGridPos = this.hitGridPos;
					if (hitGridPos.x > GetCentreLine())
					{
						this.cubeUp = -this.cubeUp;
					}
				}
			}
			Vector3 up = this.cubeUp;
			Vector3 val = CalculateOrientationAndOffset(ref forward, ref up, cubeData);
			_ghostOrientation = Quaternion.LookRotation(forward, up);
			if (raycastInfo.hitCube == null)
			{
				Int3 hitGridPos2 = this.hitGridPos;
				if (hitGridPos2.x > GetCentreLine())
				{
					groundPlacePositionOffset.x = 0f - groundPlacePositionOffset.x;
					float z = groundPlacePositionOffset.z;
					float num = cubeData.initialMirrorZOffset;
					Vector3 cubeUp = this.cubeUp;
					groundPlacePositionOffset.z = z + num * cubeUp.x;
				}
				_ghostPosition += groundPlacePositionOffset * 0.2f;
			}
			ref Vector3 ghostPosition = ref _ghostPosition;
			ghostPosition.x -= val.y * 0.2f;
			ref Vector3 ghostPosition2 = ref _ghostPosition;
			float y = ghostPosition2.y;
			float num2 = val.x * 0.2f;
			Vector3 cubeUp2 = this.cubeUp;
			ghostPosition2.y = y + num2 * cubeUp2.y;
			ref Vector3 ghostPosition3 = ref _ghostPosition;
			ghostPosition3.z -= val.z * 0.2f;
			finalGridPos = GridScaleUtility.WorldToGrid(_ghostPosition, TargetType.Player);
		}

		protected Vector3 CalculateOrientationAndOffset(ref Vector3 forward, ref Vector3 up, PersistentCubeData cubeData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = -Vector3.get_up();
			Vector3 result = Vector3.get_zero();
			if (Mathf.Abs(Vector3.Dot(cubeUp, Vector3.get_up())) > 0.9f)
			{
				List<ConnectionPoint> attachablePoints = cubeData.attachablePoints;
				List<ConnectionPoint>.Enumerator enumerator = attachablePoints.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ConnectionPoint current = enumerator.Current;
					Vector3 direction = current.direction;
					if (Mathf.Abs(Vector3.Dot(cubeUp, direction)) < 0.1f)
					{
						val = direction;
						result = current.offset;
						break;
					}
				}
			}
			if (Mathf.Abs(Vector3.Dot(cubeUp, forward)) > 0.5f)
			{
				forward = Vector3.get_forward();
			}
			if (Mathf.Abs(Vector3.Dot(val, -Vector3.get_up())) < 0.1f)
			{
				up = val;
				forward = -cubeUp;
			}
			return result;
		}

		public Quaternion GetFinalCubeRotation()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _ghostOrientation;
		}

		private void UpdateGhostTransform()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = _goCube.get_transform();
			transform.set_position(_ghostPosition);
			transform.set_rotation(_ghostOrientation);
			transform.RotateAround(transform.get_position(), transform.get_up(), (float)rotation);
		}

		private void Update()
		{
			cubeCaster.changingCube = false;
		}

		private void LateUpdate()
		{
			if (_positionOrOrientationChanging > 0)
			{
				_positionOrOrientationChanging--;
				if (_positionOrOrientationChanging == 0)
				{
					cubeCaster.cubeIntersectionsUnreliable = false;
				}
			}
		}
	}
}
