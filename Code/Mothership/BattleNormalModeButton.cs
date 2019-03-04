using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal class BattleNormalModeButton : MonoBehaviour
	{
		private EnterPlanetButtonStateManager _buttonStateManager;

		[SerializeField]
		private GameObject lockGameObject;

		[SerializeField]
		private GameObject[] objectsToDisableOnLock;

		[SerializeField]
		private UILabel lockInfoLabel;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal DesiredGameMode desiredGameMode
		{
			private get;
			set;
		}

		public event Action<string> OnButtonPressed = delegate
		{
		};

		public BattleNormalModeButton()
			: this()
		{
		}

		public void SetButtonState(GameModeAvailabilityState state)
		{
			if (this.get_gameObject().get_activeSelf())
			{
				_buttonStateManager.SetState(state);
			}
		}

		private void Awake()
		{
			_buttonStateManager = new EnterPlanetButtonStateManager(this.GetComponents<UIButton>(), lockGameObject, objectsToDisableOnLock, lockInfoLabel);
			_buttonStateManager.AddLockState(GameModeAvailabilityState.PlayerLevelTooLow, "strUnlockedAtLevelFive");
			_buttonStateManager.AddLockState(GameModeAvailabilityState.CPUTooHighLocked, "strCantPlayCpuTooHigh");
		}

		private void OnClick()
		{
			desiredGameMode.DesiredMode = LobbyType.QuickPlay;
			guiInputController.ToggleScreen(GuiScreens.BattleCountdown);
		}
	}
}
