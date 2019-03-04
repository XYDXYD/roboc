using Simulation.Hardware.Weapons;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class CubeCursor : MonoBehaviour, IInitialize
	{
		public Color CanPlaceColour = Color.get_green();

		public Color CanNotPlaceColour = Color.get_red();

		private Color _currentColor;

		private ICubeCaster _cubeCaster;

		private MeshRenderer _meshRenderer;

		private bool _frameworkInitialized;

		private bool _isPaintToolEnabled;

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal GhostCubeController ghostCubeController
		{
			private get;
			set;
		}

		[Inject]
		internal CubeRaycastInfo raycastInfo
		{
			private get;
			set;
		}

		[Inject]
		internal CurrentToolMode toolMode
		{
			private get;
			set;
		}

		public CubeCursor()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)


		private void Start()
		{
			_frameworkInitialized = true;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_meshRenderer = this.GetComponentInChildren<MeshRenderer>();
			ghostCubeController.cubeCursor = this.get_gameObject();
			ghostCubeController.OnGhostCubeInitialized += HandleOnGhostCubeInitialized;
			toolMode.OnToolModeChanged += HandleOnToolModeChanged;
		}

		private void HandleOnToolModeChanged(CurrentToolMode.ToolMode mode)
		{
			_isPaintToolEnabled = (mode == CurrentToolMode.ToolMode.Paint);
		}

		private void HandleOnGhostCubeInitialized(GhostCube ghostCube)
		{
			_cubeCaster = ghostCube.cubeCaster;
		}

		private void ProcessIndicator()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			if (!_cubeCaster.outSideTheGrid && _cubeCaster.axisAlligned)
			{
				RaycastHit hit = raycastInfo.hit;
				if (hit.get_collider() != null && !_isPaintToolEnabled && _cubeCaster.isAdjacentSuitableCube)
				{
					_meshRenderer.set_enabled(true);
					Byte3 pos = (Byte3)machineMap.FindGridLocFromHit(raycastInfo.hit, 1);
					this.get_transform().set_position(GridScaleUtility.GridToWorld(pos, TargetType.Player));
					Quaternion rotation = default(Quaternion);
					Vector3 up = Vector3.get_up();
					RaycastHit hit2 = raycastInfo.hit;
					rotation.SetFromToRotation(up, hit2.get_normal());
					this.get_transform().set_rotation(rotation);
					return;
				}
			}
			_meshRenderer.set_enabled(false);
		}

		private void ProcessColor()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (!_cubeCaster.changingCube)
			{
				if (_cubeCaster.canPlace)
				{
					_currentColor = CanPlaceColour;
				}
				else
				{
					_currentColor = CanNotPlaceColour;
				}
			}
			_meshRenderer.get_material().SetColor("_Color", _currentColor);
			_meshRenderer.get_material().SetColor("_Emission", _currentColor);
		}

		private void LateUpdate()
		{
			if (_cubeCaster != null && _frameworkInitialized)
			{
				ProcessIndicator();
				ProcessColor();
			}
		}
	}
}
