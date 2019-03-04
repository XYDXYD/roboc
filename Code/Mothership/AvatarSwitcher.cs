using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class AvatarSwitcher : MonoBehaviour, IInitialize
	{
		public GameObject PaintAvatar;

		public GameObject BuildAvatar;

		public GameObject NoToolAvatar;

		public CubePainter cubePainter;

		public CubeLauncher cubeLauncher;

		public CubeRemover cubeRemover;

		public float weaponSwapTiming = 0.2f;

		public Vector3 weaponUpPosition;

		public Vector3 weaponDownPosition;

		private const float DISTANCE_TO_ANIMATE = 2f;

		private bool _switching;

		private GameObject _currentWeapon;

		private GameObject _nextWeapon;

		private CurrentToolMode.ToolMode _desiredToolMode;

		private CurrentToolMode.ToolMode _currentShownToolMode;

		private float _timeSwitching;

		[Inject]
		internal CurrentToolMode toolMode
		{
			get;
			private set;
		}

		[Inject]
		internal BuildInputLock buildInputLock
		{
			get;
			private set;
		}

		public AvatarSwitcher()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			toolMode.OnToolModeChanged += HandleOnToolModeChanged;
			weaponDownPosition = BuildAvatar.get_transform().get_localPosition() - new Vector3(0f, 2f, 0f);
			weaponUpPosition = BuildAvatar.get_transform().get_localPosition();
			_timeSwitching = 0f;
			BuildAvatar.SetActive(true);
			PaintAvatar.SetActive(true);
			NoToolAvatar = new GameObject("EmptyTool");
			SetBuildModeToolAtStart();
		}

		private void SetBuildModeToolAtStart()
		{
			_currentWeapon = BuildAvatar;
			_currentShownToolMode = CurrentToolMode.ToolMode.Build;
			AssignWeaponPositions(_currentWeapon, 1f);
			_switching = false;
			PaintAvatar.SetActive(false);
			BuildAvatar.SetActive(true);
		}

		private void HandleOnToolModeChanged(CurrentToolMode.ToolMode mode)
		{
			_desiredToolMode = mode;
		}

		private void Update()
		{
			if (_switching)
			{
				_timeSwitching += Time.get_deltaTime();
				float num = _timeSwitching / weaponSwapTiming;
				if (num >= 1f)
				{
					AssignWeaponPositions(_currentWeapon, 0f);
					AssignWeaponPositions(_nextWeapon, 1f);
					_currentWeapon = _nextWeapon;
					_nextWeapon = null;
					_switching = false;
					LockBuildModeInput(locked: false);
				}
				else
				{
					AssignWeaponPositions(_currentWeapon, 1f - num);
					AssignWeaponPositions(_nextWeapon, num);
				}
			}
			if (!_switching && _desiredToolMode != _currentShownToolMode)
			{
				_switching = true;
				_currentWeapon = GetGameObjectForBuildMode(_currentShownToolMode);
				_nextWeapon = GetGameObjectForBuildMode(_desiredToolMode);
				AssignWeaponPositions(_currentWeapon, 1f);
				AssignWeaponPositions(_nextWeapon, 0f);
				_currentShownToolMode = _desiredToolMode;
				LockBuildModeInput(locked: true);
				_timeSwitching = 0f;
			}
		}

		private void AssignWeaponPositions(GameObject weapon, float percentageRaised)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			weapon.get_transform().set_localPosition(Vector3.Lerp(weaponDownPosition, weaponUpPosition, percentageRaised));
			if (percentageRaised == 0f)
			{
				weapon.SetActive(false);
			}
			else if (!weapon.get_activeSelf())
			{
				weapon.SetActive(true);
			}
		}

		private GameObject GetGameObjectForBuildMode(CurrentToolMode.ToolMode mode)
		{
			switch (mode)
			{
			case CurrentToolMode.ToolMode.Build:
				return BuildAvatar;
			case CurrentToolMode.ToolMode.Paint:
				return PaintAvatar;
			case CurrentToolMode.ToolMode.NoTool:
				return NoToolAvatar;
			default:
				return null;
			}
		}

		private void LockBuildModeInput(bool locked)
		{
			buildInputLock.SetLocked(locked);
		}
	}
}
